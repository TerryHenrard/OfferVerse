using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
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

        public List<Report> GetReports()
        {
            List<Report> reports = new List<Report>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query =
                        @"SELECT reportId, title, description, reporter_userId, reported_userId
                FROM Reports";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int reportId = reader.GetInt32("reportId");
                        string title = reader.GetString("title");
                        string description = reader.GetString("description");
                        int reporterUserId = reader.GetInt32("reporter_userId");
                        int reportedUserId = reader.GetInt32("reported_userId");

                        User reporter = GetUserInfo(reporterUserId);
                        User reported = GetUserInfo(reportedUserId);

                        Report report = new (reportId, title, description, reporter, reported);
                        reports.Add(report);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception("ERROR SQL" + e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("ERROR DATABASE" + e.Message);
            }

            return reports;
        }

        public User GetUserInfo(int userId)
        {
            User user;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT * FROM Users " +
                        "WHERE userId = @userId",
                        connection);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int memberId = reader.GetInt32("userId");
                            string email = reader.GetString("email");
                            string password = reader.GetString("password");
                            string firstName = reader.GetString("firstName");
                            string lastName = reader.GetString("lastName");
                            string phoneNumber = reader.GetString("phoneNumber");
                            string postCode = reader.GetString("postCode");
                            string streetName = reader.GetString("streetName");
                            string streetNumber = reader.GetString("streetNumber");
                            string city = reader.GetString("city");
                            int timeCredits = reader.GetInt32("timeCredits");

                            user = new(memberId, email, password, firstName, lastName, phoneNumber, postCode, streetNumber, streetName, city, timeCredits);
                        }
                        else
                        {
                            user = new();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An SQL error occurred.\nSqlException: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting user info.\nException: " + ex.Message);
            }

            return user;
        }

        public bool SanctionUser(int userId)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new (connectionString))
                {
                    string query = @"UPDATE Users 
                                 SET timeCredits = timeCredits - @credits 
                                 WHERE userId = @userId";

                    SqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@credits", 50);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An SQL error occurred.\nSqlException: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sanctioning user.\nException: " + ex.Message);
            }
            return success;
        }

        public bool DeleteReport(int reportId)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"DELETE FROM Reports WHERE reportId = @reportId";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@reportId", reportId);

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0; 
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An SQL error occurred.\nSqlException: " + ex.Message);
                throw new Exception("Error while deleting the report, please contact an administrator.\nException: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting the report, please contact an administrator.\nException: " + ex.Message);
            }

            return success;
        }
    }
}
