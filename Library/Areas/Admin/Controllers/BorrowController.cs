using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Library.Areas.Admin.Controllers
{
    [Authorize(Roles="Admin,Staff")]
    [Area("Admin")]
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Borrow
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Borrow.Include(b => b.Book).Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Borrow/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // GET: Admin/Borrow/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id");
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id");
            return View();
        }

        // POST: Admin/Borrow/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,UserId,BorrowDate,ReturnDate,Status")] Borrow borrow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(borrow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", null, borrow.BookId);
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Email", borrow.UserId);
            return View(borrow);
        }

        // GET: Admin/Borrow/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow.FindAsync(id);
            if (borrow == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", null, borrow.BookId);
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Email", borrow.UserId);
            return View(borrow);
        }

        // POST: Admin/Borrow/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,UserId,BorrowDate,ReturnDate,Status")] Borrow borrow)
        {
            if (id != borrow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Pobierz oryginalny stan wypo¿yczenia z bazy danych
                    var existingBorrow = await _context.Borrow
                        .AsNoTracking() // Unikamy œledzenia zmian na tym obiekcie
                        .FirstOrDefaultAsync(b => b.Id == borrow.Id);

                    if (existingBorrow == null)
                    {
                        return NotFound();
                    }

                    // Jeœli zmieniono status na "Returned", zwiêksz stan magazynowy ksi¹¿ki
                    if (existingBorrow.Status != BorrowStatus.Returned && borrow.Status == BorrowStatus.Returned)
                    {
                        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == borrow.BookId);
                        if (book != null)
                        {
                            book.Stock++; // Zwiêkszamy iloœæ dostêpnych egzemplarzy
                            _context.Update(book);
                        }
                    }

                    _context.Update(borrow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowExists(borrow.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", null, borrow.BookId);
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Email", borrow.UserId);
            return View(borrow);
        }

        // GET: Admin/Borrow/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // POST: Admin/Borrow/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrow = await _context.Borrow.FindAsync(id);
            if (borrow != null)
            {
                _context.Borrow.Remove(borrow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowExists(int id)
        {
            return _context.Borrow.Any(e => e.Id == id);
        }
    }
}
