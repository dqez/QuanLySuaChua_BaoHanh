using Microsoft.AspNetCore.Mvc;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Controllers
{
    public class RoutingController : Controller
    {
        private readonly MapService _mapService;

        public RoutingController(MapService mapService)
        {
            _mapService = mapService;
        }

        public async Task<IActionResult> GetRoute()
        {
            try
            {
                var start = await _mapService.GeocodeAsync("02 Thanh Sơn, Thanh Bình, Hải Châu, Đà Nẵng");
                var end = await _mapService.GeocodeAsync("22 Thanh Sơn, Thanh Bình, Hải Châu, Đà Nẵng");
                var route = await _mapService.GetRouteAsync(start, end);

                return Ok(new
                {
                    Start = start,
                    End = end,
                    Distance = $"{route.Distance:0.0} km",
                    Duration = $"{route.Duration:0} phút"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    public record RouteRequest(string StartAddress, string EndAddress);
}
