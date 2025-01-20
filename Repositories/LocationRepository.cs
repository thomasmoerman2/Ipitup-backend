namespace Ipitup.Repositories;

public interface ILocationRepository
{
    Task<bool> AddLocationAsync(Location location);
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task<Location?> GetLocationByIdAsync(int id);
}

public class LocationRepository : ILocationRepository
{
    private readonly string _connectionString;

    public LocationRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("SQLConnectionString")
                            ?? throw new InvalidOperationException("Database connection string is not set.");
    }

    public async Task<bool> AddLocationAsync(Location location)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO Location (locationName, locationCountry) VALUES (@name, @country)", connection);
                command.Parameters.AddWithValue("@name", location.LocationName);
                command.Parameters.AddWithValue("@country", location.LocationCountry);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding location", ex);
        }
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        var locations = new List<Location>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Location", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    locations.Add(new Location
                    {
                        LocationId = reader.GetInt32(reader.GetOrdinal("locationId")),
                        LocationName = reader.GetString(reader.GetOrdinal("locationName")),
                        LocationCountry = reader.GetString(reader.GetOrdinal("locationCountry"))
                    });
                }
            }
        }

        return locations;
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Location WHERE locationId = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Location
                    {
                        LocationId = reader.GetInt32(reader.GetOrdinal("locationId")),
                        LocationName = reader.GetString(reader.GetOrdinal("locationName")),
                        LocationCountry = reader.GetString(reader.GetOrdinal("locationCountry"))
                    };
                }
            }
        }
        return null;
    }
}
