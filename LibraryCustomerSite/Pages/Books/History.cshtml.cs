using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryCustomerSite.Pages.Books
{
    public class HistoryModel : PageModel
    {
        private readonly LMS_PRN221Context _context;
        public HistoryModel(LMS_PRN221Context context)
        {
            _context = context;
        }
		public List<BorrowInformation> BorrowInformation { get; set; } = new List<BorrowInformation>();
        public string UserName { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
		{
			var userId = HttpContext.Session.GetInt32("UserId");
            UserName = HttpContext.Session.GetString("UserName");
            if (userId == null || userId != id)
			{
				return RedirectToPage("/Books/SignUp");
			}

			BorrowInformation = await _context.BorrowInformations
				.Include(b => b.Bids)
                .ThenInclude(bc => bc.BookTitle)
                 .Where(b => b.Uid == userId)
                .ToListAsync();

			return Page();
		}
	}
}
