﻿using Microsoft.AspNetCore.Identity;
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
                throw new Exception("Error while getting user info, please contact an administrator.\nException: " + ex.Message);
            }

            return user;
        }

        public List<ServiceProvided> GetAllServicesProvided(int memberId)
        {
            List<ServiceProvided> sp = new List<ServiceProvided>();

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT * FROM ServicesProvided WHERE userId = @memberId",
                        connection
                        );

                    cmd.Parameters.AddWithValue("@memberId", memberId);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int servicePId = reader.GetInt32("servicePId");
                            string title = reader.GetString("title");
                            string description = reader.GetString("description");
                            bool priority = reader.GetBoolean("priority");
                            DateTime? datePriority = reader.IsDBNull("datePriority") ? null : reader.GetDateTime("datePriority");
                            int userId = reader.GetInt32("userId");

                            ServiceProvided s = new ServiceProvided
                                (servicePId, title, description, priority, datePriority, userId);

                            sp.Add(s);
                        }
                    }
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

            return sp;
        }

        public bool ApplyProfileChanges(User userProfil)
        {
            bool success = false;
            bool editPassword = userProfil.EditPassword;
            string query =
                "UPDATE Users " +
                "SET phoneNumber = @phoneNumber, postCode = @postCode, streetName = @streetName, " +
                "streetNumber = @streetNumber, city = @city, email = @email, firstName = @firstName, lastName = @lastName";

            if (editPassword) 
                query += ", password = @password";

            query += " WHERE userId = @userId";

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(query, connection);
                    cmd.Parameters.AddWithValue("@phoneNumber", userProfil.PhoneNumber);
                    cmd.Parameters.AddWithValue("@postCode", userProfil.PostCode);
                    cmd.Parameters.AddWithValue("@streetName", userProfil.StreetName);
                    cmd.Parameters.AddWithValue("@streetNumber", userProfil.StreetNumber);
                    cmd.Parameters.AddWithValue("@city", userProfil.City);
                    cmd.Parameters.AddWithValue("@email", userProfil.Email);
                    cmd.Parameters.AddWithValue("@firstName", userProfil.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", userProfil.LastName);
                    cmd.Parameters.AddWithValue("@userId", userProfil.MemberId); //TODO: replace 1 with the id of the authenticated user in the session

                    if (editPassword)
                        cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.EnhancedHashPassword(userProfil.Password, 13));

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

        public List<ServiceDemanded> GetTransactions(int memberId, bool inProgress = false)
        {
            List<ServiceDemanded> ServicesD = new();

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
                                WHERE u.userId = @userId AND sd.endService ";
                query += inProgress ? "IS NULL" : "IS NOT NULL";
                query += " ORDER BY sd.startService DESC";

                SqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@userId", memberId);

                if (inProgress)
                    cmd.Parameters.AddWithValue("@sd_uid", memberId);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int serviceDId = reader.GetInt32("ServiceDId");
                        DateTime? startService = !reader.IsDBNull("Start date") ? reader.GetDateTime("Start date") : null;
                        DateTime? endService = !reader.IsDBNull("End date") ? reader.GetDateTime("End date") : null;
                        int DId = reader.GetInt32("Demander Id");
                        string DFirstName = reader.GetString("Demander first name");
                        string DLastName = reader.GetString("Demander last name");
                        int PId = reader.GetInt32("Provider Id");
                        string PFirstName = reader.GetString("Provider first name");
                        string PLastName = reader.GetString("Provider last name");
                        int SPId = reader.GetInt32("Service provided Id");
                        string title = reader.GetString("Title");
                        string description = reader.GetString("Description");
                        int? hours = !reader.IsDBNull("Amount of the transaction") ? reader.GetInt32("Amount of the transaction") : null;

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

        public bool DeleteServiceProvided(int sId)
        {
            bool success = false;
            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "DELETE FROM Images WHERE servicePId = @sId " +
                        "DELETE FROM Commentaries WHERE servicePId = @sId " +
                        "DELETE FROM ServicesDemanded WHERE serviceProvidedId = @sId " +
                        "DELETE FROM Favorites WHERE servicePId = @sId " +
                        "DELETE FROM ServicesProvided WHERE servicePId = @sId"
                        ,
                        connection
                        );

                    cmd.Parameters.AddWithValue("@sId", sId);
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
                throw new Exception("Error while deleting a service : " + e.Message);
            }

            return success;
        }
        public bool AddServiceProvided(ServiceProvided service, int uId)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "INSERT INTO ServicesProvided (title, description, userId, priority, categoryId)" +
                        "VALUES(@title, @description, @userId, @priority, @categoryId)", connection
                        );

                    cmd.Parameters.AddWithValue("@title", service.Title);
                    cmd.Parameters.AddWithValue("@description", service.Description);
                    cmd.Parameters.AddWithValue("@userId", uId);
                    cmd.Parameters.AddWithValue("@priority", service.Priority);
                    cmd.Parameters.AddWithValue("@categoryId", service.Category.CategoryId);
                    //TODO : add images to a service

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

        public bool PromoteServiceProvided(int sId)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "UPDATE ServicesProvided " +
                        " SET priority = 1, datePriority = @datePriority WHERE servicePId = @sId",
                        connection
                        );

                    cmd.Parameters.AddWithValue("@datePriority", DateTime.Now);
                    cmd.Parameters.AddWithValue("@sId", sId);

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e)
            {
                throw new Exception("An SQL error occurred : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("Error while promoting a service : " + e.Message);
            }

            return success;
        }

        public bool CheckCredits(int uId)
        {
            bool success = false;

            try
            {
                using(SqlConnection connection = new(connectionString)) 
                {
                    SqlCommand cmd = new(
                        "SELECT timeCredits FROM Users " +
                        " WHERE userId = @uId", connection
                        );

                    cmd.Parameters.AddWithValue("@uId", uId);

                    connection.Open();

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int credits = reader.GetInt32("timeCredits");

                            success = credits >= 5;
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL error occurred : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("Error while checking the credits : " + e.Message);
            }

            return success;
        }

        public bool DebitUser(int uId)
        {
            bool success = false;

            try
            {
                using( SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "UPDATE Users " +
                        "SET timeCredits = timeCredits - 5 ", connection
                        );

                    connection.Open();

                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL error occurred : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("An error occurred while debiting the user : " + e.Message);
            }

            return success;
        }

        public bool DebitUser(int memberId, int? nbHours)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        @"UPDATE Users 
                          SET timeCredits = timeCredits - @timeCredits
                          WHERE userId = @serviceDId",
                        connection);

                    cmd.Parameters.AddWithValue("@timeCredits", nbHours);
                    cmd.Parameters.AddWithValue("@serviceDId", memberId);

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

        public bool CreditUser(int memberId, int? nbHours)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        @"UPDATE Users 
                          SET timeCredits = timeCredits + @timeCredits
                          WHERE userId = @servicePId",
                        connection);

                    cmd.Parameters.AddWithValue("@timeCredits", nbHours);
                    cmd.Parameters.AddWithValue("@servicePId", memberId);

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

        public int CheckLogin(string mail, string password)
        {
            int userId = 0; // 0 means he's not connected

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT userId, password " +
                        "FROM Users " +
                        "WHERE email = @mail", connection);
                    cmd.Parameters.AddWithValue("@mail", mail);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashedPasswordFromDB = reader.GetString(reader.GetOrdinal("password"));
                            if (BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPasswordFromDB))
                            {
                                userId = reader.GetInt32(reader.GetOrdinal("userId"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return -1;
            }

            return userId;
        }

        public bool IsAdmin(int id)
        {
            bool isAdmin = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT isAdmin FROM Users WHERE userId = @userId", connection);
                    cmd.Parameters.AddWithValue("@userId", id);

                    connection.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        isAdmin = Convert.ToBoolean(result);
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return isAdmin;
        }



        public bool AskForAService(int sProvidedId, int sDemanderId, int sProviderId)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new(
                        "INSERT INTO ServicesDemanded (serviceProvidedId, serviceDemander_userId, serviceProvider_userId) " +
                        " VALUES (@sProvidedId, @sDemanderId, @sProviderId)", connection
                        );

                    cmd.Parameters.AddWithValue("@sProvidedId", sProvidedId);
                    cmd.Parameters.AddWithValue("@sProviderId", sProviderId);
                    cmd.Parameters.AddWithValue("@sDemanderId", sDemanderId);

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e)
            {
                throw new Exception("An SQL error has occurred : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("Error while asking for a service : " + e.Message);
            }

            return success;
        }

        public bool CheckIfServiceDemanded(int uId, int spId)
        {
            bool success = false;

            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT serviceDId FROM ServicesDemanded " +
                        " WHERE endService IS NULL AND serviceProvidedId = @spId AND serviceDemander_userId = @uId",
                        connection
                        );

                    cmd.Parameters.AddWithValue("@spId", spId);
                    cmd.Parameters.AddWithValue("@uId", uId);

                    connection.Open();
                    
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            success = true;
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL exception has occurred : " + e.Message);
            }
            catch( Exception e )
            {
                throw new Exception("Error while checking if service si demanded : " + e.Message);
            }

            return success;
        }

        public List<ServiceProvided> GetFavorites(int userId)
        {
            List<ServiceProvided> favorites = new();
            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    string query =
                        @"SELECT sp.*
                          FROM Favorites fav 
                          INNER JOIN Users u ON fav.userId = u.userId
                          INNER JOIN ServicesProvided sp ON sp.servicePId = fav.servicePId
                          WHERE u.userId = @userId";

                    SqlCommand command = new(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int servicePId = reader.GetInt32("servicePId");
                        string title = reader.GetString("title");
                        string description = reader.GetString("description");
                        bool priority = reader.GetBoolean("priority");
                        DateTime? datePriority = !reader.IsDBNull("datePriority") ? reader.GetDateTime("datePriority") : null;
                        int categoryId = reader.GetInt32("categoryId");

                        ServiceProvided sp = new(servicePId, title, description, priority, datePriority, categoryId);
                        favorites.Add(sp);
                    }
                    
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

            return favorites;
        }

        public int Register(string firstName, string lastName, string mail, string city, string postCode, string streetName, string streetNumber, string password, string confirmPassword, string phoneNumber)
        {
            int userId;
            if (password != confirmPassword)
            {
                return -2;  // means password isn't the same
            }

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Users (firstName, lastName, email, city, postCode, streetName, streetNumber, password, phoneNumber) " +
                        "VALUES (@firstName, @lastName, @mail, @city, @postCode, @streetName, @streetNumber, @hashedPassword, @phoneNumber);" +
                        "SELECT SCOPE_IDENTITY();", connection);

                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@mail", mail);
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.Parameters.AddWithValue("@postCode", postCode);
                    cmd.Parameters.AddWithValue("@streetName", streetName);
                    cmd.Parameters.AddWithValue("@streetNumber", streetNumber);
                    cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

                    connection.Open();

                    // get the id to connect the user after the creation
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return -1;
            }

            return userId;
        }


        public bool AcceptDemand(int demandId)
        {
            bool success = false;

            try
            {
                using(SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "UPDATE ServicesDemanded " +
                        " SET startService = @start " +
                        " WHERE serviceDId = @dID ", connection
                        );

                    cmd.Parameters.AddWithValue("@start", DateTime.Now);
                    cmd.Parameters.AddWithValue("@dId", demandId);

                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL error has occurred : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("An error has occurred while accepting the demand : " + e.Message);
            }

            return success;
        }

        public bool RefuseDemand(int demandId)
        {
            bool success = false;

            try
            {
                using(SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "DELETE FROM ServicesDemanded " +
                        " WHERE serviceDId = @sId ", connection
                        );

                    cmd.Parameters.AddWithValue("@sId", demandId);
                    
                    connection.Open();
                    success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException e)
            {
                throw new Exception("An SQL error has occurred : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("Error while refusing the demand : " + e.Message);
            }
            
            return success;
        }
    }
}
