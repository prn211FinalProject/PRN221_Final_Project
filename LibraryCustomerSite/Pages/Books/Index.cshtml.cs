using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LibraryCustomerSite.Models;
using Microsoft.Data.SqlClient;

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
        public IList<Category> Categories { get; set; } = new List<Category>();
        public async Task OnGetAsync(List<int> categoryIds, string sortOrder)
        {
            IQueryable<Book> bookQuery = _context.Books
                .Where(b => b.Hide == false);

            if (categoryIds != null && categoryIds.Any())
            {
                // Lọc sách theo các category được chọn
                bookQuery = bookQuery.Where(b => categoryIds.Contains(b.Cid ?? 0));
            }
            switch (sortOrder)
            {
                case "name":
                    bookQuery = bookQuery.OrderBy(b => b.Bname);
                    break;
                case "price":
                    bookQuery = bookQuery.OrderBy(b => b.Price);
                    break;
                case "quantity":
                    bookQuery = bookQuery.OrderBy(b => b.Quantity);
                    break;
                case "status":
                    bookQuery = bookQuery.OrderBy(b => b.Status);
                    break;
                default:
                    bookQuery = bookQuery.OrderBy(b => b.Bname); // Mặc định sắp xếp theo tên
                    break;
            }
            Book = await bookQuery.ToListAsync();
            Categories = await _context.Categories
                .Where(c => c.Status == true)
                .ToListAsync();
        }

    }
}
