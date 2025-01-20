namespace Ipitup.Services;

public interface ILocationService
{
    Task<bool> AddLocationAsync(Location location);
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task<Location?> GetLocationByIdAsync(int id);
}

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<bool> AddLocationAsync(Location location)
    {
        if (string.IsNullOrWhiteSpace(location.LocationName) || string.IsNullOrWhiteSpace(location.LocationCountry))
        {
            throw new ArgumentException("Invalid location data");
        }

        return await _locationRepository.AddLocationAsync(location);
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        return await _locationRepository.GetAllLocationsAsync();
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        return await _locationRepository.GetLocationByIdAsync(id);
    }
}
