using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace LibraryCustomerSite.Pages.Books
{
    public class ScheduleReturnModel : PageModel
    {
        private readonly LMS_PRN221Context _context;
        public ScheduleReturnModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public DateTime ReturnDate { get; set; }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var borrowInfo = await _context.BorrowInformations.FindAsync(id);
            if (borrowInfo != null)
            {
                borrowInfo.ReturnDate = ReturnDate;
                borrowInfo.Status = 6; // Thay đổi trạng thái thành 6
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/Books/History", new { id = borrowInfo.Uid });
        }
    }
}