//using LibraryCustomerSite.Models;
using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryCustomerSite.Pages.Books
{
    public class SignUpModel : PageModel
    {
        private readonly LMS_PRN221Context _context;

        public SignUpModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; private set; }

        [BindProperty]
        public User NewUser { get; set; } = new User();

        public void OnGet()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (userEmail != null)
            {
                Response.Redirect("/Books/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string FormAction)
        {
            if (FormAction == "Login")
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email); // Lưu email vào session (đã đăng nhập thành công)
                    HttpContext.Session.SetString("UserName", user.FullName); // Lưu UserName vào session (đã đăng nhập thành công) 
                    HttpContext.Session.SetInt32("UserId", user.Uid); // Lưu UserId vào session (đã đăng nhập thành công)
                    return RedirectToPage("/Books/Index");
                }
                else
                {
                    Message = "Invalid email or password.";
                    return Page();
                }
            }
            else if (FormAction == "SignUp")
            {
                if (_context.Users.Any(u => u.Email == NewUser.Email))
                {
                    Message = "Email already exists.";
                    return Page();
                }
                NewUser.Status = true; // Activate user
                NewUser.RoleId = 1; // Đặt RoleId mặc định
                _context.Users.Add(NewUser);
                await _context.SaveChangesAsync();
                Message = "Sign up successful! You can now login.";
                return Page();
            }

            return Page();
        }
        
    }
}
