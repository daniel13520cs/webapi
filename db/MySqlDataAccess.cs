using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class MySqlDataAccess
{
    IConfiguration _configuration;
    private readonly string? _connectionString;

    public MySqlDataAccess() {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json") // Assuming your JSON configuration is in appsettings.json file
            .Build();
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public MySqlDataAccess(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string? GetSessionToken(string username)
    {
        string sessionToken = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString)) {
            connection?.Open();
            
        string query = "SELECT * FROM shopping_session WHERE username = @Username";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Username", username);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                // Assuming you want to retrieve the session token from the result
                if (reader.Read())
                {
                    sessionToken = reader["session_token"].ToString(); // Adjust the column name accordingly
                }
            }
        }
        }

        return sessionToken;
    }

    public void CreateSessionToken(string username, string token)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString)) {
            connection?.Open();
            string query = $"INSERT INTO shopping_session (username, session_token, created_at, modified_at) VALUES (@username, @sessionToken, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@sessionToken", token);
                
                // Execute the query
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Session added successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to add session.");
                }
            }
        }
    }
}
