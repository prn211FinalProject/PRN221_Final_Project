using LibraryCustomerSite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCustomerSite.Pages.Books
{
    public class BorrowModel : PageModel
    {
        private readonly LMS_PRN221Context _context;
        public BorrowModel(LMS_PRN221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public BorrowInformation BorrowInformation { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string BookName { get; set; }

        public IActionResult OnGet(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Books/SignUp");
            }

            UserName = HttpContext.Session.GetString("UserName");
            Email = HttpContext.Session.GetString("UserEmail");

            var book = _context.BookTitles.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            BorrowInformation = new BorrowInformation
            {
                Uid = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14), // Giả sử thời hạn mượn sách là 14 ngày
                Status = true,
            };

            BookName = book.Bname;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Books/SignUp");
            }

            var book = _context.BookTitles.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            if (book.Quantity <= 0)
            {
                ModelState.AddModelError(string.Empty, "Sách này hiện không có sẵn.");
                return Page();
            }

            BorrowInformation.Uid = userId;
            BorrowInformation.Status = true; // Đặt giá trị mặc định cho Status

            // Lấy ID lớn nhất hiện có trong bảng BookCopies
            var lastBookCopy = _context.BookCopies.OrderByDescending(bc => bc.Id).FirstOrDefault();
            int nextIdNumber = 1;
            if (lastBookCopy != null)
            {
                // Lấy phần số từ ID cuối cùng và tăng lên 1
                string lastId = lastBookCopy.Id.Substring(2); // Bỏ "BC"
                nextIdNumber = int.Parse(lastId) + 1;
            }
            string newId = "BC" + nextIdNumber.ToString("D3"); // Định dạng thành "BC" + số với 3 chữ số

            // Tạo một bản sao của sách và lưu vào cơ sở dữ liệu để tạo Id
            var bookCopy = new BookCopy
            {
                Id = newId,
                BookTitleId = book.Id,
                Status = 1,
                Note = "OK"
            };
            _context.BookCopies.Add(bookCopy);
            _context.SaveChanges();

            // Thêm bản sao sách vào thông tin mượn
            BorrowInformation.Bids.Add(bookCopy);
            _context.BorrowInformations.Add(BorrowInformation);

            // Giảm số lượng sách
            book.Quantity -= 1;
            _context.BookTitles.Update(book);

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Mượn sách thành công!";
            return RedirectToPage("Index");
        }
    }
    }