using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data.SqlClient;

namespace OfferVerse.DAL
{
    public class ServiceDemandedDAL : IServiceDemandedDAL
    {
        private readonly string connectionString;

        public ServiceDemandedDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool FinalizeService(int serviceDId, int? nbHours)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        @"UPDATE ServicesDemanded 
                      SET endService = GETDATE(),
                          nbHours = @nbHours
                      WHERE serviceDId = @serviceDId",
                        connection);

                    cmd.Parameters.AddWithValue("@nbHours", nbHours);
                    cmd.Parameters.AddWithValue("@serviceDId", serviceDId);

                    connection.Open();
                    int res = cmd.ExecuteNonQuery();
                    success = res > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return success;
        }
    }
}
