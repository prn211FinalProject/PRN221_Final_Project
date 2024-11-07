using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryAdminSite
{
    /// <summary>
    /// Interaction logic for ViewBorrowHandle.xaml
    /// </summary>
    public partial class ViewBorrowHandle : Window
    {
        public ViewBorrowHandle(int id )
        {
            InitializeComponent();
            allBorrow(id);
        }

        public void allBorrow(int id)
        {
            // Truy vấn dữ liệu từ các bảng LibrarianOrder, BorrowInformation, User và BookCopy
            var borrowHandle = LMS_PRN221Context.Ins.LibrarianOrders
                .Include(x => x.BorrowOrder) // Bao gồm thông tin đơn mượn
                .ThenInclude(x => x.UidNavigation) // Bao gồm thông tin người dùng (User)
                .ThenInclude(x => x.BorrowInformations) // Bao gồm các thông tin mượn sách của người dùng
                .ThenInclude(x => x.Bids) // Bao gồm các bản sao sách (BookCopy)
                .Where(x => x.LibrarianId == id) // Lọc theo LibrarianId
                .Select(x => new
                {
                    Id = x.Id, // Lấy Id của LibrarianOrder
                    UserName = x.BorrowOrder.UidNavigation.FullName, // Lấy tên người dùng từ BorrowOrder (User)
                    BookName = string.Join(", ", x.BorrowOrder.Bids.Select(b => b.BookTitle.Bname)), // Lấy tên sách từ BookCopy, nối các tên sách nếu có nhiều
                    BorrowDate = x.BorrowOrder.BorrowDate // Ngày mượn từ BorrowInformation
                    
                })
                .ToList();

            // Gán kết quả vào ListView (hoặc bất kỳ phần tử giao diện nào bạn muốn)
            lvDisplay.ItemsSource = borrowHandle;
        }


    }
}
