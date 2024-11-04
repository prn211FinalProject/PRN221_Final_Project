using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryCustomerSite.Pages.Books
{
    public class WishListModel : PageModel
    {
        private readonly LMS_PRN221Context _context;
        public WishListModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        public IList<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

        public async Task<RedirectToPageResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Books/SignUp");
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.Bids)
                .ThenInclude(b => b.BookTitle)
                .Where(w => w.Uid == userId)
                .ToListAsync();

            if (wishlist != null)
            {
                BookCopies = wishlist.SelectMany(w => w.Bids).ToList();
            }
            return RedirectToPage("/Books/WishList");
        }
        public async Task<IActionResult> OnPostRemoveFromWishlistAsync(int id)
        {
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
                return NotFound();
            }

            var bookCopy = wishlist.Bids.FirstOrDefault(bc => bc.BookTitleId == id);
            if (bookCopy == null)
            {
                return NotFound();
            }

            wishlist.Bids.Remove(bookCopy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa thành công sách khỏi Wishlist";
            return RedirectToPage(new { id = id });
        }
    }
}