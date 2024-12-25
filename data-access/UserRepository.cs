using Microsoft.Data.Sqlite;
using System.Windows.Forms;
using password_manager_project.encryption;

public class UserRepository
{
    private readonly string connectionString;

    public UserRepository()
    {
        string dbPath = @"C:\Users\User\Documents\code\C#Class\password-manager-project\pswmanager.db";
        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    public async Task<User> GetUserByUsername(string username)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM user WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        MasterPassword = reader.GetString(2)
                    };
                }
                return null;
            }
        }
    }

    public async Task CreateUser(User user)
    {
        try
        {
            string hashedPassword = PasswordHasher.HashPassword(user.MasterPassword);
            
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = 
                    "INSERT INTO user (username, master_password) VALUES (@username, @master_password)";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@master_password", hashedPassword);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database error in CreateUser: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

    public User? GetUser(string username)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM user WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            MasterPassword = reader.GetString(2)
                        };
                    }
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database error in GetUser: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    public void AddUser(User user)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Users (Username, MasterPassword)
            VALUES ($username, $masterPassword)";

        command.Parameters.AddWithValue("$username", user.Username);
        command.Parameters.AddWithValue("$masterPassword", user.MasterPassword);

        command.ExecuteNonQuery();
    }
}
