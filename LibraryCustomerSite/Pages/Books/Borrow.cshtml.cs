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
            if (book.UnitInStock <= 0)
            {
                TempData["ErrorMessage"] = "Sách này hiện không có sẵn.";
                return RedirectToPage("/Books/Details", new { id = id });
            }
            BorrowInformation = new BorrowInformation
            {
                Uid = userId,
                BorrowDate = DateTime.Now,
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7), // Giả sử thời hạn mượn sách là 7 ngày
                Status = 2,
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

            if (book.UnitInStock <= 0)
            {
                ModelState.AddModelError(string.Empty, "Sách này hiện không có sẵn.");
                return Page();
            }

            BorrowInformation.Uid = userId;
            BorrowInformation.Status = 2; 

            // Lấy ID lớn nhất hiện có trong bảng BookCopies
            //var lastBookCopy = _context.BookCopies.OrderByDescending(bc => bc.Id).FirstOrDefault();
            //int nextIdNumber = 1;
            //if (lastBookCopy != null)
            //{
            //    // Lấy phần số từ ID cuối cùng và tăng lên 1
            //    string lastId = lastBookCopy.Id.Substring(2); // Bỏ "BC"
            //    nextIdNumber = int.Parse(lastId) + 1;
            //}
            string newId = "BC"+book.Id; // Định dạng thành "BC" + số với 3 chữ số

            // Tạo một bản sao của sách và lưu vào cơ sở dữ liệu để tạo Id
            //var bookCopy = new BookCopy
            //{
            //    Id = newId,
            //    BookTitleId = book.Id,
            //    Status = true,
            //    Note = "OK"
            //};
            var bookCopy = _context.BookCopies.Where(x=>x.Status == true).FirstOrDefault(x => x.Id.Contains(newId));
            if (bookCopy != null)
            {
                bookCopy.Status = true;
            }

            // Thêm bản sao sách vào thông tin mượn
            BorrowInformation.Bids.Add(bookCopy);
            _context.BorrowInformations.Add(BorrowInformation);

            // Giảm số lượng sách
            book.UnitInStock -= 1;
            _context.BookTitles.Update(book);

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Mượn sách thành công!";
            return RedirectToPage("Index");
        }
    }
    }