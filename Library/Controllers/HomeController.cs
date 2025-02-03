using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Library.Data;

namespace Library.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var messages = _context.Messages.OrderByDescending(m => m.Timestamp).Take(6).ToList();

        // Pobierz ostatnie 5 ksi¹¿ek
        var latestBooks = _context.Books
            .Include(b => b.Author)
            .OrderByDescending(b => b.Id)  
            .Take(5)  
            .ToList();

        ViewBag.latestBooks = latestBooks;
        //return View(latestBooks);

        return View(messages);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}