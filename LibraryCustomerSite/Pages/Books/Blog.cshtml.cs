using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibraryCustomerSite.Models;

namespace LibraryCustomerSite.Pages.Books
{
    public class BlogModel : PageModel
    {
        private readonly LMS_PRN221Context _context;

        public BlogModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        public List<Blog> Blog { get; set; }

        public void OnGet()
        {
            Blog = _context.Blogs.ToList();
        }
    }
}
