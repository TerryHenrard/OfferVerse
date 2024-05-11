using Microsoft.AspNetCore.Identity;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection.PortableExecutable;

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

        public bool ApplyProfileChanges(User userProfil)
        {
            bool success = false;
            bool editPassword = userProfil.EditPassword;
            string query =
                "UPDATE OfferVerse.dbo.Users " +
                "SET phoneNumber = @phoneNumber, postCode = @postCode, streetName = @streetName, " +
                "streetNumber = @streetNumber, city = @city, email = @email, firstName = @firstName, lastName = @lastName";

            if (editPassword) 
                query += ", password = @password";

            query += " WHERE userId = @userId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@phoneNumber", userProfil.PhoneNumber);
                    cmd.Parameters.AddWithValue("@postCode", userProfil.PostCode);
                    cmd.Parameters.AddWithValue("@streetName", userProfil.StreetName);
                    cmd.Parameters.AddWithValue("@streetNumber", userProfil.StreetNumber);
                    cmd.Parameters.AddWithValue("@city", userProfil.City);
                    cmd.Parameters.AddWithValue("@email", userProfil.Email);
                    cmd.Parameters.AddWithValue("@firstName", userProfil.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", userProfil.LastName);
                    cmd.Parameters.AddWithValue("@userId", 1); //TODO: replace 1 with the id of the authenticated user in the session

                    if (editPassword)
                    {
                        cmd.Parameters.AddWithValue(
                            "@password", 
                            BCrypt.Net.BCrypt.EnhancedHashPassword(userProfil.Password, 13));
                    }

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e)
            {
                throw new Exception("An SQL error occured : " + e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Error while getting the list of provided services : " + e.Message);
            }

            return success;
        }

        public List<ServiceDemanded> GetTransactions(int memberId, bool inProgress)
        {
            List<ServiceDemanded> ServicesD = new List<ServiceDemanded>();

            using (SqlConnection connection = new(connectionString))
            {
                string query = @"SELECT
                                    sd.serviceDId [ServiceDId],
                                    sd.startService [Start date],
                                    sd.endService [End date],
                                    UDemander.userId [Demander Id],
                                    UDemander.firstName [Demander first name],
                                    UDemander.lastName [Demander last name],
                                    UProvider.userId [Provider Id],
                                    UProvider.firstName [Provider first name],
                                    UProvider.lastName [Provider last name],
                                    sp.servicePId [Service provided Id],
                                    sp.title [Title],
                                    sp.description [Description],
                                    sd.nbHours [Amount of the transaction]
                                FROM Users u
                                INNER JOIN ServicesDemanded sd ON u.userId = sd.serviceProvider_userId OR u.userId = sd.serviceDemander_userId  
                                INNER JOIN ServicesProvided sp ON sd.serviceProvidedId = sp.servicePId
                                INNER JOIN Users UDemander ON UDemander.userId = sd.serviceDemander_userId 
                                INNER JOIN Users UProvider ON UProvider.userId = sd.serviceProvider_userId 
                                WHERE u.userId = @userId AND sd.endService";
                if (inProgress)
                    query += " IS NULL AND sd.serviceDemander_userId = @sd_uid";
                else
                    query += " IS NOT NULL";

                query += " ORDER BY sd.startService DESC";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@userId", memberId);

                if (inProgress)
                    cmd.Parameters.AddWithValue("@sd_uid", memberId);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int serviceDId = reader.GetInt32("ServiceDId");
                        DateTime startService = reader.GetDateTime("Start date");

                        DateTime? endService = null;
                        if (!reader.IsDBNull("End date"))
                        {
                            endService = reader.GetDateTime("End date");
                        }

                        int DId = reader.GetInt32("Demander Id");
                        string DFirstName = reader.GetString("Demander first name");
                        string DLastName = reader.GetString("Demander last name");
                        int PId = reader.GetInt32("Provider Id");
                        string PFirstName = reader.GetString("Provider first name");
                        string PLastName = reader.GetString("Provider last name");
                        int SPId = reader.GetInt32("Service provided Id");
                        string title = reader.GetString("Title");
                        string description = reader.GetString("Description");

                        int? hours = null;
                        if (!reader.IsDBNull("Amount of the transaction"))
                        {
                            hours = reader.GetInt32("Amount of the transaction");
                        }

                        ServicesD.Add(new(serviceDId, 
                                          startService, 
                                          endService, 
                                          hours, 
                                          PId, 
                                          PFirstName, 
                                          PLastName, 
                                          DId, 
                                          DFirstName, 
                                          DLastName, 
                                          SPId, 
                                          title, 
                                          description));
                    }
                }
            }
            return ServicesD;
        }

        public ServiceDemanded GetInProgressTransaction(int serviceId)
        {
            ServiceDemanded serviceDemanded;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        @"SELECT
                        sd.serviceDId [ServiceDId],
                        sd.startService [Start date],
                        UProvider.userId [Provider Id],
                        UProvider.firstName [Provider first name],
                        UProvider.lastName [Provider last name],
                        sp.servicePId [Service provided Id],
                        sp.title [Title],
                        sp.description [Description]
                    FROM ServicesDemanded sd 
                    INNER JOIN ServicesProvided sp ON sd.serviceProvidedId = sp.servicePId
                    INNER JOIN Users UProvider ON UProvider.userId = sd.serviceProvider_userId 
                    WHERE serviceDId = @serviceDId;"
                        , connection);
                    cmd.Parameters.AddWithValue("@serviceDId", serviceId);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int serviceDId = reader.GetInt32("ServiceDId");
                            DateTime startService = reader.GetDateTime("Start date");
                            int PId = reader.GetInt32("Provider Id");
                            string PFirstName = reader.GetString("Provider first name");
                            string PLastName = reader.GetString("Provider last name");
                            int SPId = reader.GetInt32("Service provided Id");
                            string title = reader.GetString("Title");
                            string description = reader.GetString("Description");

                            serviceDemanded = new(serviceDId, startService, PId, PFirstName, PLastName, SPId, title, description);
                        }
                        else
                        {
                            serviceDemanded = new();
                        }

                    }
                }
            }
            catch (SqlException ex) 
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex) 
            {
                throw new Exception (ex.Message);   
            }
            
            return serviceDemanded;
        }
    }
}
