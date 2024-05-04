using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
using System.Data.SqlClient;

namespace OfferVerse.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly string connectionString;

        public UserDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public User GetUserInfo(int userId)
        {
            User user;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT * FROM OfferVerse.dbo.Users " +
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

                            user = new(memberId, email, password, firstName, lastName, phoneNumber, postCode, streetName, streetNumber, city);
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
                throw new Exception("Error while getting user info, please contact an administrator.\nException: " + ex.Message);
            }

            return user;
        }

        public List<ServiceProvided> GetAllServicesProvided(int memberId)
        {
            List<ServiceProvided> sp = new List<ServiceProvided>();

            try
            {
                using(SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT * FROM OfferVerse.dbo.ServicesProvided WHERE userId = @memberId",
                        connection
                        );

                    cmd.Parameters.AddWithValue("@memberId", memberId);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int servicePId = reader.GetInt32("servicePId");
                            string title = reader.GetString("title");
                            string description = reader.GetString("description");
                            bool priority = reader.GetBoolean("priority");
                            DateTime datePriority = reader.GetDateTime("datePriority");

                            ServiceProvided s = new ServiceProvided(servicePId, title, description, priority, datePriority);
                            sp.Add(s);
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL error occured : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("Error while getting the list of provided services : " + e.Message);
            }

            return sp;
        }
    }
}
