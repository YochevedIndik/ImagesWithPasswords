using System;
using System.Data.SqlClient;

namespace ImagesWithPasswords.Data
{
    public class ImagesRepository
    {
        private string _connectionString;
        public ImagesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(Images image)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images(ImagePath, Password, Views) " +
                              "VALUES(@imagePath, @password, @views) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@imagePath", image.ImagePath);
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@views", image.Views);
            
            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();
            
        }
        public Images GetImageById(int id) 
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            return new Images()
            {
                Id = id,
                ImagePath = (string)reader["ImagePath"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };
      }
        public void UpdateViews(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views = ISNULL(Views, 1) + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
    }
}
