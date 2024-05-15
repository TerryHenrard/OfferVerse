using Microsoft.AspNetCore.Mvc.Filters;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
using System.Data.SqlClient;

namespace OfferVerse.DAL
{
    public class ServiceProvidedDAL : IServicesProvidedDAL
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
                using(SqlConnection connection  = new SqlConnection(connectionString))
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

                            sp = new ServiceProvided(serviceId, title, description, catId, userId);
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
                using( SqlConnection connection = new SqlConnection( connectionString))
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
    }
}
