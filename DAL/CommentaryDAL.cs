using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Data.SqlClient;

namespace OfferVerse.DAL
{
    public class CommentaryDAL : ICommentaryDAL
    {
        private readonly string connectionString;

        public CommentaryDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool InsertCommentary(string content, int rating, int servicePId)
        {
            bool success = false;

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new(
                    @"INSERT INTO Commentaries(content, rating, commentaryDate, userId, servicePId)
                      VALUES(@content, @rating, GETDATE(), @userId, @servicePId);",
                    connection);
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@userId", 4); // à remplacer avec la valeur de l'utilisateur connecté
                cmd.Parameters.AddWithValue("@servicePId", servicePId); 
                
                connection.Open();
                int res = cmd.ExecuteNonQuery();
                success = res > 0;
            }

            return success;
        }
    }
}
