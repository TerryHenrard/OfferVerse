using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;

namespace OfferVerse.DAL
{
    public class ServiceProvidedDAL : IServiceProvidedDAL
    {
        private readonly string connectionString;

        public ServiceProvidedDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ServiceProvided GetServiceProvidedInfo(int sId)
        {
            ServiceProvided sp;

            try
            {
                using(SqlConnection connection  = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT servicePId, title, description, categoryId, userId " +
                        " FROM ServicesProvided WHERE servicePId = @sId",
                        connection                       
                        );

                    cmd.Parameters.AddWithValue("@sId", sId);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int serviceId = reader.GetInt32("servicePId");
                            string title = reader.GetString("Title");
                            string description = reader.GetString("Description");
                            int catId = reader.GetInt32("categoryId");
                            int userId = reader.GetInt32("userId");

                            sp = new(serviceId, title, description, catId, userId);
                        }
                        else
                        {
                            sp = new();
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                throw new Exception("An SQL error has occurred : " + e.Message);
            }
            catch(Exception e) 
            {
                throw new Exception("Error while getting the service infos : " + e.Message);
            }

            return sp;
        }

        public bool ApplyServiceProvidedChanges(ServiceProvided sp, int id)
        {
            bool success = false;

            try
            {
                using(SqlConnection connection = new( connectionString))
                {
                    SqlCommand cmd = new(
                        "UPDATE ServicesProvided " +
                        " SET title = @title, description = @description, categoryId = @catId " +
                        " WHERE servicePId = @sId", connection
                        );

                    cmd.Parameters.AddWithValue("@title", sp.Title);
                    cmd.Parameters.AddWithValue("@description", sp.Description);
                    cmd.Parameters.AddWithValue("@catId", sp.Category.CategoryId);
                    cmd.Parameters.AddWithValue("@sId", id);

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
                throw new Exception("Error while updating a service provided : " + e.Message);
            }

            return success;
        }

        public List<ServiceProvided> GetServicesProvided(int pageNb, int servicesPerPage)
        {
            List<ServiceProvided> servicesP = new();

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    string query =
                      @"SELECT *
                        FROM ServicesProvided sp 
                        INNER JOIN Categories c ON sp.categoryId = c.categoryId
                        ORDER BY 
                            sp.priority DESC, 
                            CASE 
                                WHEN sp.priority = 1 THEN sp.datePriority 
                            END ASC,
                            CASE 
                                WHEN sp.priority = 0 THEN NEWID() 
                            END
                        OFFSET (@pageNb - 1) * @servicePerPage ROWS 
                        FETCH NEXT @servicePerPage ROWS ONLY";
                    SqlCommand cmd = new(query, connection);

                    cmd.Parameters.AddWithValue("@servicePerPage", servicesPerPage);
                    cmd.Parameters.AddWithValue("@pageNb", pageNb);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            /******Service provided*/
                            int servicePId = reader.GetInt32("servicePId");
                            string title = reader.GetString("title");
                            string description = reader.GetString("description");
                            bool priority = reader.GetBoolean("priority");
                            DateTime? datePriority = !reader.IsDBNull("datePriority") ? reader.GetDateTime("datePriority") : null;
                            int userId = reader.GetInt32("userId");

                            /******Category******/
                            int categoryId = reader.GetInt32("categoryId");
                            string categoryName = reader.GetString("name");
                            string imagePath = reader.GetString("imagePath");

                            ServiceProvided sp = new(servicePId, title, description, priority, datePriority, userId, categoryId, categoryName, imagePath);
                            GetCommentariesForService(sp);
                            servicesP.Add(sp);
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

            return servicesP;
        }

        public int GetNumberOfPages(int servicesPerPage)
        {
            double nbPages;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new("SELECT CEILING(COUNT(servicePId) / CAST(@nbServicesPerPage AS FLOAT)) nbPages FROM ServicesProvided;", connection);

                    cmd.Parameters.AddWithValue("@nbServicesPerPage", servicesPerPage);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        nbPages = reader.Read() ? reader.GetDouble("nbPages") : 0;
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

            return (int)nbPages;
        }

        private bool GetCommentariesForService(ServiceProvided sp)
        {
            bool succes = false;
            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    string query =
                        @"SELECT com.commentaryId, com.content, com.rating, com.commentaryDate, u.userId, u.firstName, u.lastName
                        FROM Commentaries com INNER JOIN Users u
                        ON com.userId = u.userId
                        WHERE servicePId = @ServicePId";

                    SqlCommand command = new(query, connection);
                    command.Parameters.AddWithValue("@ServicePId", sp.ServiceProvidedId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int commentaryId = reader.GetInt32("commentaryId");
                        string content = reader.GetString("content");
                        int rating = reader.GetInt32("rating");
                        DateTime commentaryDate = reader.GetDateTime("commentaryDate");
                        int userId = reader.GetInt32("userId");
                        string firstName = reader.GetString("firstName");
                        string lastName = reader.GetString("lastName");

                        Commentary com = new(commentaryId, content, rating, commentaryDate, userId, firstName, lastName);
                        sp.AddCommentary(com);
                    }
                    succes = true;
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

            return succes;
        }

        public ServiceProvided GetServiceProvided(int servicePId)
        {
            ServiceProvided sp;

            try
            {
                using (SqlConnection connection = new(connectionString))
                {
                    string query =
                        @"SELECT 
	                        sp.servicePId spservicePId,
	                        sp.title sptitle,
	                        sp.description spdescription,
	                        sp.priority sppriority,
	                        sp.datePriority spdatePriority,
	                        u.userId uuserid,
	                        u.phoneNumber uphoneNumber,
	                        u.postCode, 
	                        u.streetName ustreetName,
	                        u.streetNumber ustreetNumber,
	                        u.city ucity,
	                        u.email uemail,
	                        u.firstName ufirstName,
	                        u.lastName ulastName,
	                        c.categoryId ccategoryId,
	                        c.imagePath cimagePath,
	                        c.name cname
                        FROM ServicesProvided sp 
                        INNER JOIN Users u ON sp.userId = u.userId
                        INNER JOIN Categories c ON sp.categoryId = c.categoryId
                        WHERE servicePId = @servicePId";

                    SqlCommand command = new(query, connection);
                    command.Parameters.AddWithValue("@servicePId", servicePId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int spservicePId = reader.GetInt32("spservicePId");
                        string sptitle = reader.GetString("sptitle");
                        string spdescription = reader.GetString("spdescription");
                        bool sppriority = reader.GetBoolean("sppriority");
                        DateTime? spdatePriority = !reader.IsDBNull("spdatePriority") ? reader.GetDateTime("spdatePriority") : null;

                        int uuserid = reader.GetInt32("uuserid");
                        string uphoneNumber = reader.GetString("uphoneNumber");
                        string postCode = reader.GetString("postCode");
                        string ustreetName = reader.GetString("ustreetName");
                        string ustreetNumber = reader.GetString("ustreetNumber");
                        string ucity = reader.GetString("ucity");
                        string uemail = reader.GetString("uemail");
                        string ufirstName = reader.GetString("ufirstName");
                        string ulastName = reader.GetString("ulastName");

                        int ccategoryId = reader.GetInt32("ccategoryId");
                        string cimagePath = reader.GetString("cimagePath");
                        string cname = reader.GetString("cname");

                        sp = new(
                            servicePId, 
                            sptitle, 
                            spdescription, 
                            sppriority, 
                            spdatePriority, 
                            uuserid, 
                            uphoneNumber, 
                            postCode, 
                            ustreetName, 
                            ustreetNumber, 
                            ucity, 
                            uemail, 
                            ufirstName, 
                            ulastName, 
                            ccategoryId, 
                            cname, 
                            cimagePath);  ;

                        GetCommentariesForService(sp);
                    }
                    else
                    {
                        sp = new();
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
    }
}
