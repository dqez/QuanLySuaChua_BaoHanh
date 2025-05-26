using Newtonsoft.Json;

namespace QuanLySuaChua_BaoHanh.Services
{
    public class MapService
    {
        private readonly HttpClient _httpClient;

        public MapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourAppName");
        }

        public async Task<Location> GeocodeAsync(string address)
        {
            var response = await _httpClient.GetAsync(
                $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Geocoding failed");

            var content = await response.Content.ReadAsStringAsync();
            var results = JsonConvert.DeserializeObject<List<NominatimResult>>(content);

            return results?.FirstOrDefault() is NominatimResult result
                ? new Location(result.Lat, result.Lon, result.DisplayName)
                : throw new Exception("Location not found");
        }

        public async Task<RouteResult> GetRouteAsync(Location start, Location end)
        {
            var response = await _httpClient.GetAsync(
                $"https://router.project-osrm.org/route/v1/driving/{start.Longitude},{start.Latitude};{end.Longitude},{end.Latitude}?overview=full");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Routing failed");

            var result = await response.Content.ReadFromJsonAsync<OsrmRouteResponse>();

            return new RouteResult(
                Distance: result.Routes[0].Distance / 1000, // km
                Duration: result.Routes[0].Duration / 60,   // minutes
                Geometry: result.Routes[0].Geometry
            );
        }
    }
    public record NominatimResult(
    [property: JsonProperty("lat")] double Lat,
    [property: JsonProperty("lon")] double Lon,
    [property: JsonProperty("display_name")] string DisplayName);

    public record Location(double Latitude, double Longitude, string DisplayName = "");

    public record OsrmRouteResponse(
        [property: JsonProperty("routes")] Route[] Routes,
        [property: JsonProperty("code")] string Code);

    public record Route(
        [property: JsonProperty("distance")] double Distance,
        [property: JsonProperty("duration")] double Duration,
        [property: JsonProperty("geometry")] string Geometry);

    public record RouteResult(double Distance, double Duration, string Geometry);
}
