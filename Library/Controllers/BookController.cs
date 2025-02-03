using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Library.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? categoryId, string? query)
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var books = _context.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();
           

            if (categoryId.HasValue && categoryId > 0)
            {
                books = books.Where(b => b.CategoryId == categoryId);
                ViewBag.SelectedCategory = categories
                                                .Where(c => c.Id == categoryId)
                                                .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(query))
            {
                return View(await books.ToListAsync());
                //return PartialView("_SearchResults", await books.ToListAsync());
            }

            ViewBag.query = query;

            var results = SearchBooks(query, books);


            return View(results.ToList());
            //return PartialView("_SearchResults", await results.ToListAsync());
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Files).FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        public async Task<IActionResult> Search(string? query, int? categoryId)
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var books = _context.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();


            if (categoryId.HasValue && categoryId > 0)
            {
                books = books.Where(b => b.CategoryId == categoryId);
                ViewBag.SelectedCategory = categories
                                                .Where(c => c.Id == categoryId)
                                                .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(query))
            {
                return View(await books.ToListAsync());
                //return PartialView("_SearchResults", await books.ToListAsync());
            }

            ViewBag.query = query;

            var results = SearchBooks(query, books);


            return View(results.ToList());
            //return PartialView("_SearchResults", await results.ToListAsync());

        }

        public async Task<IActionResult> Search2(string? query, int? categoryId)
        {
            var categories = _context.Categories.ToList();

            var books = _context.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                books = books.Where(b => b.CategoryId == categoryId);
                ViewBag.SelectedCategory = categories
                                                .Where(c => c.Id == categoryId)
                                                .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(query))
            {
                return PartialView("_SearchResults", await books.ToListAsync()); // Zwróć tylko fragment widoku
            }

            var results = SearchBooks(query, books);

            return PartialView("_SearchResults", await results.ToListAsync()); // Zwróć tylko fragment widoku

        }

        private IQueryable<Book> SearchBooks(string query, IQueryable<Book> _books)
        {
            // Rozdzielenie zapytania na słowa kluczowe
            var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Inicjalizacja listy do dynamicznego budowania zapytań
            var books = _books;

            bool isAnd = true; // domyślnie łączymy za pomocą AND
            bool negateNext = false;

            foreach (var token in tokens)
            {
                if (token.ToUpper() == "AND")
                {
                    isAnd = true;
                    continue;
                }
                else if (token.ToUpper() == "OR")
                {
                    isAnd = false;
                    continue;
                }
                else if (token.ToUpper() == "NOT")
                {
                    negateNext = true;
                    continue;
                }

                // Filtrowanie na podstawie słowa kluczowego
                var keyword = token.ToLower();
                if (isAnd)
                {
                    if (negateNext)
                    {
                        books = books.Where(b => !(
                            b.Title.ToLower().Contains(keyword) ||
                            b.Author.FirstName.ToLower().Contains(keyword) ||
                            b.Author.LastName.ToLower().Contains(keyword) ||
                            b.ISBN.ToLower().Contains(keyword)
                        ));
                        negateNext = false;
                    }
                    else
                    {
                        books = books.Where(b =>
                            b.Title.ToLower().Contains(keyword) ||
                            b.Author.FirstName.ToLower().Contains(keyword) ||
                            b.Author.LastName.ToLower().Contains(keyword) ||
                            b.ISBN.ToLower().Contains(keyword)
                        );
                    }
                }
                else // OR
                {
                    if (negateNext)
                    {
                        books = books.Concat(
                            _context.Books.Include(b => b.Author).Include(b => b.Category).Where(b => !(
                                b.Title.ToLower().Contains(keyword) ||
                                b.Author.FirstName.ToLower().Contains(keyword) ||
                                b.Author.LastName.ToLower().Contains(keyword) ||
                                b.ISBN.ToLower().Contains(keyword)
                            ))
                        ).Distinct();
                        negateNext = false;
                    }
                    else
                    {
                        books = books.Concat(
                            _context.Books.Include(b => b.Author).Include(b => b.Category).Where(b =>
                                b.Title.ToLower().Contains(keyword) ||
                                b.Author.FirstName.ToLower().Contains(keyword) ||
                                b.Author.LastName.ToLower().Contains(keyword) ||
                                b.ISBN.ToLower().Contains(keyword)
                            )
                        ).Distinct();
                    }
                }
            }

            return books.AsQueryable();
        }
       
    }
}


