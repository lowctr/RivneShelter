using System.Diagnostics;
using RivneSheltersProject.Data;
using Microsoft.AspNetCore.Mvc;
using RivneSheltersProject.Models;
using Microsoft.EntityFrameworkCore;

namespace RivneSheltersProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
    {
        return await _context.Locations.ToListAsync();
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Rivne()
    {
        return View();
    }
    public IActionResult Admin()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    public IActionResult Edit()
    {
        return View();
    }
    public IActionResult Terms()
    {
        return View();
    }
    public IActionResult About()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}