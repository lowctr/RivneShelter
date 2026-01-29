using RivneSheltersProject.Data;
using RivneSheltersProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RivneSheltersProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public LocationsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetLocations()
    {
        try
        {
            var locations = await _context.Locations
                .Select(location => new {
                    Id = location.Id,
                    Address = location.Address,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                })
                .ToListAsync();

            return Ok(locations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                error = "Помилка підключення до бази даних",
                details = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Location>> GetLocation(int id)
    {
        try
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound(new { error = "Локацію не знайдено" });
            }

            return location;
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                error = "Помилка підключення до бази даних",
                details = ex.Message
            });
        }
    }

    [HttpGet("GetDistanceMatrix")]
    public async Task<IActionResult> GetDistanceMatrix(string origins, string destinations, string mode)
    {
        var apiKey = "АПІ_КЛЮЧ";
        var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origins}&destinations={destinations}&mode={mode}&key={apiKey}";

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("nearest")]
    public ActionResult<List<object>> GetNearestLocations(double userLatitude, double userLongitude)
    {
        try
        {
            // Haversine formula implemented in LINQ
            const double earthRadiusKm = 6371.0;
            var userLatRad = userLatitude * Math.PI / 180.0;
            var userLonRad = userLongitude * Math.PI / 180.0;

            var locations = _context.Locations
                .AsEnumerable()
                .Select(location => {
                    var latRad = location.Latitude * Math.PI / 180.0;
                    var lonRad = location.Longitude * Math.PI / 180.0;

                    var dLat = latRad - userLatRad;
                    var dLon = lonRad - userLonRad;

                    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                            Math.Cos(userLatRad) * Math.Cos(latRad) *
                            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    var distance = earthRadiusKm * c;

                    return new {
                        Id = location.Id,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Distance = distance
                    };
                })
                .OrderBy(l => l.Distance)
                .Take(5)
                .Select(l => new {
                    Id = l.Id,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                })
                .ToList();

            return Ok(locations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                error = "Помилка пошуку найближчих локацій",
                details = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location updatedLocation)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(updatedLocation.Address))
        {
            location.Address = updatedLocation.Address;
        }
        if (!string.IsNullOrEmpty(updatedLocation.Ownership))
        {
            location.Ownership = updatedLocation.Ownership;
        }
        if (!string.IsNullOrEmpty(updatedLocation.Holder))
        {
            location.Holder = updatedLocation.Holder;
        }
        if (!string.IsNullOrEmpty(updatedLocation.Accessibility))
        {
            location.Accessibility = updatedLocation.Accessibility;
        }
        if (!string.IsNullOrEmpty(updatedLocation.Contact))
        {
            location.Contact = updatedLocation.Contact;
        }
        if (!string.IsNullOrEmpty(updatedLocation.Number))
        {
            location.Number = updatedLocation.Number;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool LocationExists(int id)
    {
        return _context.Locations.Any(e => e.Id == id);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] Location newLocation)
    {
        if (newLocation == null)
        {
            return BadRequest();
        }

        _context.Locations.Add(newLocation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLocation), new { id = newLocation.Id }, newLocation);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
