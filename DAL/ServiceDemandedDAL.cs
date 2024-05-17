using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
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

        public List<ServiceDemanded> GetServicesDemanded(int uId)
        {
            List<ServiceDemanded> requests = new List<ServiceDemanded>();

            try
            {
                using(SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT sd.serviceDId, sd.startService, sd.endService, sd.nbHours, sd.serviceProvidedId, " +
                        " sp.title, sd.serviceDemander_userId, d.lastName AS 'dLN', d.firstName AS 'dFN', " +
                        " sd.serviceProvider_userId, " +
                        " u.lastName AS 'uLN', u.firstName AS 'uFN' FROM ServicesDemanded sd" +
                        " INNER JOIN Users u ON u.userId = sd.serviceProvider_userId " +
                        " INNER JOIN ServicesProvided sp ON sp.servicePId = sd.serviceProvidedId " +
                        " INNER JOIN Users d ON d.userId = sd.serviceDemander_userId " +
                        " WHERE sd.serviceProvider_userId = @uId AND sd.startService IS NULL ", connection
                        );

                    cmd.Parameters.AddWithValue("@uId", uId);

                    connection.Open();

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sId = reader.GetInt32("serviceDId");
                            DateTime? start = !reader.IsDBNull("startService") ? reader.GetDateTime("startService") : null;
                            DateTime? end = !reader.IsDBNull("endService") ? reader.GetDateTime("endService") : null;
                            int? nbHours = !reader.IsDBNull("nbHours") ? reader.GetInt32("nbHours") : null;
                            int sProvidedId = reader.GetInt32("serviceProvidedId");
                            string spTitle = reader.GetString("title");
                            int sDemanderId = reader.GetInt32("serviceDemander_userId");
                            string demanderLN = reader.GetString("dLN");
                            string demanderFN = reader.GetString("dFN");
                            int sProviderId = reader.GetInt32("serviceProvider_userId");
                            string providerLN = reader.GetString("uLN");
                            string providerFN = reader.GetString("uFN");

                            ServiceDemanded sd = new(sId, start, end, nbHours, 
                                sProviderId, providerFN, providerLN, sDemanderId, demanderFN, demanderLN,
                                sProvidedId, spTitle, "nothing");

                            requests.Add(sd);
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL error occurred : " + e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("An error has occurred while getting the demands : " + e.Message);
            }

            return requests;
        }
    }
}
