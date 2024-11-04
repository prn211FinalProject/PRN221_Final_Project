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
    public class DetailsModel : PageModel
    {
        private readonly LibraryCustomerSite.Models.LibraryManagementContext _context;

        public DetailsModel(LibraryCustomerSite.Models.LibraryManagementContext context)
        {
            _context = context;
        }

      public Book Book { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Book = await _context.Books.FindAsync(id);

            if (Book == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
