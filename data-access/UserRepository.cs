using Microsoft.Data.Sqlite;

public class UserRepository
{
    private readonly string connectionString;

    public UserRepository()
    {
        string dbPath = Path.Combine(Application.StartupPath, "pswmanager.db");
        connectionString = $"Data Source={dbPath}";
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
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = 
                "INSERT INTO user (username, master_password) VALUES (@username, @password)";
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@password", user.MasterPassword);
            await command.ExecuteNonQueryAsync();
        }
    }
}
