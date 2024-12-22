using Microsoft.Data.Sqlite;

public class GrainRepository
{
    private readonly string connectionString;

    public GrainRepository()
    {
        string dbPath = @"C:\Users\User\Documents\code\C#Class\password-manager-project\pswmanager.db";
        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    public async Task<List<Grain>> GetGrainsByUserId(int userId)
    {
        var grains = new List<Grain>();
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM grains WHERE user_id = @userId";
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    grains.Add(new Grain
                    {
                        Id = reader.GetInt32(0),
                        ServiceName = reader.GetString(1),
                        Password = reader.GetString(2),
                        UserId = reader.GetInt32(3),
                        CreatedAt = reader.GetDateTime(4),
                        UpdatedAt = reader.GetDateTime(5)
                    });
                }
            }
        }
        return grains;
    }

    public async Task CreateGrain(Grain grain)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = 
                "INSERT INTO grains (service_name, password, user_id) VALUES (@service, @password, @userId)";
            command.Parameters.AddWithValue("@service", grain.ServiceName);
            command.Parameters.AddWithValue("@password", grain.Password);
            command.Parameters.AddWithValue("@userId", grain.UserId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task UpdateGrain(Grain grain)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = 
                "UPDATE grains SET service_name = @service, password = @password WHERE id = @id";
            command.Parameters.AddWithValue("@service", grain.ServiceName);
            command.Parameters.AddWithValue("@password", grain.Password);
            command.Parameters.AddWithValue("@id", grain.Id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteGrain(int id)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM grains WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<bool> AddGrain(string serviceName, string password, int userId)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO grains (service_name, password, user_id) 
            VALUES (@service, @password, @userId)";
        
        command.Parameters.AddWithValue("@service", serviceName);
        command.Parameters.AddWithValue("@password", password);
        command.Parameters.AddWithValue("@userId", userId);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteGrain(string serviceName, int userId)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM grains 
            WHERE service_name = @service 
            AND user_id = @userId";
        
        command.Parameters.AddWithValue("@service", serviceName);
        command.Parameters.AddWithValue("@userId", userId);
        
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<List<Grain>> SearchGrains(string serviceName, int userId)
    {
        var grains = new List<Grain>();
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT * FROM grains 
            WHERE service_name LIKE @search 
            AND user_id = @userId";
        
        command.Parameters.AddWithValue("@search", $"%{serviceName}%");
        command.Parameters.AddWithValue("@userId", userId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            grains.Add(new Grain
            {
                Id = reader.GetInt32(0),
                ServiceName = reader.GetString(1),
                Password = reader.GetString(2),
                UserId = reader.GetInt32(3)
            });
        }
        
        return grains;
    }

    public async Task<List<Grain>> GetAllGrains(int userId)
    {
        var grains = new List<Grain>();
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM grains WHERE user_id = @userId";
        command.Parameters.AddWithValue("@userId", userId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            grains.Add(new Grain
            {
                Id = reader.GetInt32(0),
                ServiceName = reader.GetString(1),
                Password = reader.GetString(2),
                UserId = reader.GetInt32(3)
            });
        }
        
        return grains;
    }
}