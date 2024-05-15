using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data.SqlClient;

namespace OfferVerse.DAL
{
    public class ReportDAL : IReportDAL
    {
        private readonly string connectionString;

        public ReportDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public bool InsertReport(Report report)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    string query =
                        @"INSERT INTO Reports(title, description, reporter_userId, reported_userId)
                          VALUES(@title, @description, @reporter_userId, @reported_userId)";
                    SqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@title", report.Title);
                    cmd.Parameters.AddWithValue("@description", report.Description);
                    cmd.Parameters.AddWithValue("@reporter_userId", report.Reporter.MemberId);
                    cmd.Parameters.AddWithValue("@reported_userId", report.Reported.MemberId);

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e)
            {
                throw new Exception("An SQL error occurred : " + e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating a service : " + e.Message);
            }

            return success;
        }
    }
}
