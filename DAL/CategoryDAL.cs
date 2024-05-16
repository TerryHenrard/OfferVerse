using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data;
using System.Data.SqlClient;

namespace OfferVerse.DAL
{
    public class CategoryDAL : ICategoryDAL
    {
        private readonly string connectionString;

        public CategoryDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            try 
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand cmd = new(
                        "SELECT * FROM Categories", connection
                        );

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int catId = reader.GetInt32("categoryId");
                            string catName = reader.GetString("Name");
                            string imagePath = reader.GetString("ImagePath");
                            categories.Add(new(catId, catName, imagePath));
                        }
                    }
                   
                }
            }
            catch (SqlException e)
            {
                throw new Exception("An SQL exception occured : " + e.Message);
            }
            catch(Exception e)
            {
                throw new Exception("Error while getting the categories : " + e.Message);
            }

            return categories;
        }
    }
}
