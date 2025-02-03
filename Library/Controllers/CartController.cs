using Library.Data;
using Library.Helpers;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Wyświetlanie koszyka
        public IActionResult Index()
        {
            var cart = CartHelper.GetCart(HttpContext.Session);
            return View(cart);
        }

        // Dodanie książki do koszyka
        public IActionResult AddToCart(int bookId)
        {
            var book = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.Id == bookId);
            if (book != null && book.Stock > 0)
            {
                var item = new CartItem
                {
                    BookId = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Cover = book.Cover, 
                    AuthorName = book.Author?.ToString() ?? "Nieznany autor"
                };
                book.Stock--;
                _context.SaveChanges();
                CartHelper.AddToCart(HttpContext.Session, item);
                return Json(new { success = true }); // Zwracamy JSON dla AJAX
            }
            //return RedirectToAction("Index", "Cart");
            //return Redirect(Request.Headers["Referer"].ToString()); // Wraca na poprzednią stronę
            return Json(new { success = false, Message = "Brak egzemplarzy" });
        }

        // Usunięcie książki z koszyka
        public IActionResult RemoveFromCart(int bookId)
        {
            var book = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.Id == bookId);
            book.Stock++;
            _context.SaveChanges();
            CartHelper.RemoveFromCart(HttpContext.Session, bookId);
            return RedirectToAction("Index", "Cart");
        }

        // Wypożyczenie książek
        [HttpPost]
        public async Task<IActionResult> BorrowBooks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home"); 
            }

            var cart = CartHelper.GetCart(HttpContext.Session);

            if (!cart.Any())
            {
                TempData["Error"] = "Koszyk jest pusty!";
                return RedirectToAction("Index");
            }

            foreach (var item in cart)
            {
                var book = _context.Books.FirstOrDefault(b => b.Id == item.BookId);
                if (book == null || book.Stock < 0)
                {
                    TempData["Error"] = $"Brak książki {item.Title} w magazynie.";
                    return RedirectToAction("Index");
                }

                var borrow = new Borrow
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    BorrowDate = DateTime.Now,
                    ReturnDate = null, 
                    Status = BorrowStatus.Borrowed
                };

                _context.Borrow.Add(borrow);
                //book.Stock--; 
            }

            await _context.SaveChangesAsync();
            CartHelper.ClearCart(HttpContext.Session);

            TempData["Success"] = "Pomyślnie wypożyczono książki!";
            return RedirectToAction("Index");
        }
    }
}
