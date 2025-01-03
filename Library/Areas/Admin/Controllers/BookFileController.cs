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

namespace Library.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Area("Admin")]
    public class BookFileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BookFile
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BookFiles.Include(b => b.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/BookFile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookFile = await _context.BookFiles
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookFile == null)
            {
                return NotFound();
            }

            return View(bookFile);
        }

        // GET: Admin/BookFile/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id");
            return View();
        }

        // POST: Admin/BookFile/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileName,Description,FilePath,BookId")] BookFile bookFile,
            IFormFile FilePath)
        {
            if (ModelState.IsValid)
            {
                if (FilePath != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", FilePath.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await FilePath.CopyToAsync(stream);
                    }

                    bookFile.FilePath = "/files/" + FilePath.FileName;
                }

                _context.Add(bookFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books, "Id", null, bookFile.BookId);
            return View(bookFile);
        }

        // GET: Admin/BookFile/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookFile = await _context.BookFiles.FindAsync(id);
            if (bookFile == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", null, bookFile.BookId);
            return View(bookFile);
        }

        // POST: Admin/BookFile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FileName,Description,FilePath,BookId")] BookFile bookFile, IFormFile FilePath)
        {
            if (id != bookFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (FilePath != null)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files",
                            FilePath.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await FilePath.CopyToAsync(stream);
                        }

                        bookFile.FilePath = "/files/" + FilePath.FileName;
                    }

                    _context.Update(bookFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookFileExists(bookFile.Id))
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

            ViewData["BookId"] = new SelectList(_context.Books, "Id", null, bookFile.BookId);
            return View(bookFile);
        }        
        
        // GET: Admin/BookFile/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookFile = await _context.BookFiles
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookFile == null)
            {
                return NotFound();
            }

            return View(bookFile);
        }

        // POST: Admin/BookFile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookFile = await _context.BookFiles.FindAsync(id);
            if (bookFile != null)
            {
                _context.BookFiles.Remove(bookFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookFileExists(int id)
        {
            return _context.BookFiles.Any(e => e.Id == id);
        }
    }
}
