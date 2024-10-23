using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LibraryCustomerSite.Models;

namespace LibraryCustomerSite.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly LibraryCustomerSite.Models.LibraryManagementContext _context;

        public IndexModel(LibraryCustomerSite.Models.LibraryManagementContext context)
        {
            _context = context;
        }

        public IList<Book> Book { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Books != null)
            {
                Book = await _context.Books
                .Include(b => b.CidNavigation).ToListAsync();
            }
        }
    }
}
