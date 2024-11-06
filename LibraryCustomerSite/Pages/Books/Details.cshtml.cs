using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Identity;
using LibraryCustomerSite.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LibraryCustomerSite.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly LMS_PRN221Context _context;
        private readonly IHubContext<ServerHub> hubContext;
        public DetailsModel(LMS_PRN221Context context, IHubContext<ServerHub> hub)
        {
            _context = context;
            hubContext = hub;
        }

        [BindProperty]
        public BookTitle Book { get; set; } = default!;
        [BindProperty]
        public List<Feedback> Feedback { get; set; } = default!;
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
            Feedback = await _context.Feedbacks
             .Where(f => f.Bid == id)
            .Include(f => f.UidNavigation)
            .ToListAsync();
            return Page();
        }
        
        public async Task<IActionResult> OnPostAddFeedbackAsync(int id, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var feedback = new Feedback
            {
                Uid = userId.Value,
                Bid = id,
                Content = content,
                CreatedDate = DateTime.Now,
                Status = true,
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm bình luận thành công";
            hubContext.Clients.All.SendAsync("LoadAll", id);
            return RedirectToPage(new { id = id });
        }
        public async Task<IActionResult> OnPostDeleteFeedbackAsync(int id)
        {
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa bình luận thành công";
            return RedirectToPage(new { id = feedback.Bid });
        }
        
        public async Task<IActionResult> OnPostAddToWishlistAsync(int id)
        {
            // Kiểm tra BookTitle có tồn tại không
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