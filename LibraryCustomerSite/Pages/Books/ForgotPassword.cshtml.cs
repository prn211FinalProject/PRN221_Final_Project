using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCustomerSite.Pages.Books
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly LMS_PRN221Context _context;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public ForgotPasswordModel(LMS_PRN221Context context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại");
                return Page();
            }

            // Tạo mã đặt lại mật khẩu
            var code = GeneratePasswordResetToken();
            // Lưu mã đặt lại mật khẩu vào cơ sở dữ liệu
            await SavePasswordResetToken(user.Uid, code);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string link = Url.Page("/Books/ResetPassword", null, new { code = code }, Request.Scheme);

            // Gửi email
            await _emailSender.SendEmailAsync(Email, "Reset Password", $"Please reset your password by <a href='{link}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "We've Emailed the link");
            return Page();
        }

        public void OnGet()
        {
        }

        private string GeneratePasswordResetToken()
        {
            // Tạo mã đặt lại mật khẩu (ví dụ: sử dụng GUID)
            return Guid.NewGuid().ToString();
        }

        private async Task SavePasswordResetToken(int userId, string token)
        {
            // Lưu mã đặt lại mật khẩu vào cơ sở dữ liệu
            var passwordResetToken = new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1) // Mã hết hạn sau 1 giờ
            };
            _context.PasswordResetTokens.Add(passwordResetToken);
            await _context.SaveChangesAsync();
        }
    }
}