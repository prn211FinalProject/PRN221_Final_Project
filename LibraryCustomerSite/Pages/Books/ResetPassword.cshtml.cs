using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCustomerSite.Pages.Books
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [BindProperty]
        public string? Code { get; set; }

        private readonly LMS_PRN221Context _context;

        public ResetPasswordModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Ghi lại các lỗi trong ModelState
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        // Ghi lại lỗi (bạn có thể sử dụng logging framework hoặc ghi ra console)
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }

            // Ghi lại thông tin người dùng để kiểm tra
            Console.WriteLine($"User found: {user.Uid}, Email: {user.Email}");

            // Giải mã mã token từ URL
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            Console.WriteLine($"Decoded code: {decodedCode}");

            var passwordResetToken = await _context.PasswordResetTokens
                .SingleOrDefaultAsync(t => t.UserId == user.Uid && t.Token == decodedCode);

            if (passwordResetToken == null)
            {
                // Ghi lại thông tin token để kiểm tra
                Console.WriteLine($"Password reset token not found or expired for UserId: {user.Uid}, Token: {decodedCode}");
                ModelState.AddModelError(string.Empty, "Invalid or expired password reset token.");
                return Page();
            }

            if (passwordResetToken.Expiration < DateTime.UtcNow)
            {
                // Ghi lại thông tin token hết hạn
                Console.WriteLine($"Password reset token expired for UserId: {user.Uid}, Token: {decodedCode}");
                ModelState.AddModelError(string.Empty, "Invalid or expired password reset token.");
                return Page();
            }

            // Băm mật khẩu mới và lưu vào cơ sở dữ liệu
            user.Password = HashPassword(Password);
            _context.Users.Update(user);
            _context.PasswordResetTokens.Remove(passwordResetToken);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}