using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly BHSC_DbContext _context;

        public AddressController(BHSC_DbContext context)
        {
            _context = context;
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.ThanhPhos.Select(c => new { id = c.ThanhPhoId, name = c.TenThanhPho }).ToListAsync();
            return Ok(cities);
        }

        [HttpGet("districts/{cityId}")]
        public async Task<IActionResult> GetDistricts(string cityId)
        {
            var districts = await _context.Quans
                .Where(d => d.ThanhPhoId == cityId)
                .Select(d => new { id = d.QuanId, name = d.TenQuan })
                .ToListAsync();
            return Ok(districts);
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(string districtId)
        {
            var wards = await _context.Phuongs
                .Where(w => w.QuanId == districtId)
                .Select(w => new { id = w.PhuongId, name = w.TenPhuong })
                .ToListAsync();
            return Ok(wards);
        }
    }
}
