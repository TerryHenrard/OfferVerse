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

        public bool InsertCommentary(Commentary commentary)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Commentaries(content, rating, commentaryDate, userId, servicePId)
                      VALUES(@content, @rating, @commentaryDate, @userId, @servicePId);",
                    connection);
                
                connection.Open();
                int res = cmd.ExecuteNonQuery();
                success = res > 0;
            }

            return success;
        }
    }
}
