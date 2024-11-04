using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryCustomerSite.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly LMS_PRN221Context _context;

        public DetailsModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public BookTitle Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book = await _context.BookTitles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Book == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToWishlistAsync(int id)
        {
            // Kiểm tra xem BookTitle có tồn tại không
            var bookTitle = await _context.BookTitles
                .FirstOrDefaultAsync(bt => bt.Id == id);

            if (bookTitle == null)
            {
                return NotFound();
            }

            // Tìm BookCopy với BookTitleId tương ứng
            var bookCopy = await _context.BookCopies
                .Include(bc => bc.BookTitle)
                .FirstOrDefaultAsync(bc => bc.BookTitleId == id);

            if (bookCopy == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.Bids)
                .FirstOrDefaultAsync(w => w.Uid == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    Uid = userId.Value,
                    Bids = new List<BookCopy>()
                };
                _context.Wishlists.Add(wishlist);
            }

            // Kiểm tra xem sách đã tồn tại trong wishlist chưa
            if (wishlist.Bids.Any(bc => bc.Id == bookCopy.Id))
            {
                TempData["ErrorMessage"] = "Cuốn sách này đã ở trong wishlist";
                return RedirectToPage(new { id = id });
            }

            wishlist.Bids.Add(bookCopy);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sách được thêm vào wishlist thành công";
            return RedirectToPage(new { id = id });
        }
        
    }
}