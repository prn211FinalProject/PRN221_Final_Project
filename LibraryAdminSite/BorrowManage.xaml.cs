using LibraryAdminSite.Models;
using LibraryAdminSite.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LibraryAdminSite
{
    /// <summary>
    /// Interaction logic for BorrowManage.xaml
    /// </summary>
    public partial class BorrowManage : UserControl
    {
        public BorrowManage()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBorrowInfor();
            cbxFilterStatus.Items.Add("Tất cả");
            cbxFilterStatus.Items.Add("Đã mượn");
            cbxFilterStatus.Items.Add("Chưa mượn");
            cbxFilterStatus.Items.Add("Đã trả");
            cbxFilterStatus.Items.Add("Trễ hạn");
            cbxFilterStatus.Items.Add("Đã hủy");
            LoadcboChangeStatus();
            cbxFilterStatus.SelectedIndex = 0;
        }
        private void LoadBorrowInfor()
        {
            var borrow = LMS_PRN221Context.Ins.BorrowInformations.Include(x => x.UidNavigation).Select(x => new
            {
                Id = x.Oid,
                Uid = x.Uid,
                FullName = x.UidNavigation.FullName,
                Email = x.UidNavigation.Email,
                CheckoutDate = x.CheckoutDate,
                BorrowDate = x.BorrowDate,
                DueDate = x.DueDate,
                ReturnDate = x.ReturnDate,
                Status = x.Status == 1 ? "Đã mượn" : (x.Status == 2 ? "Chưa mượn" : (x.Status == 3 ? "Đã trả" : (x.Status == 4 ? "Trễ hạn" : "Đã hủy"))),
                Phone = x.UidNavigation.Phone,
                Punish = x.PenaltyAmount,
                bookCopy = x.Bids,
                IsOverDue = x.DueDate < DateTime.Now ? "true" : "false",
                //IsCancel = x.CheckoutDate.HasValue && (DateTime.Now - x.CheckoutDate).Days >=7 ? "true" : "false",
            }).ToList();
            lvDisplay.ItemsSource = borrow;
        }
        private ObservableCollection<object> bookDisplayList = new ObservableCollection<object>();
        private void LoadcboChangeStatus()
        {
            cbxChangeStatus.Items.Clear();
            cbxChangeStatus.Items.Add("Đã mượn");
            cbxChangeStatus.Items.Add("Chưa mượn");
            cbxChangeStatus.Items.Add("Đã trả");
            cbxChangeStatus.Items.Add("Trễ hạn");
            cbxChangeStatus.Items.Add("Đã hủy");
        }
        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadcboChangeStatus();
            bookDisplayList.Clear();
            var selectedItem = lvDisplay.SelectedItem;
            if (selectedItem != null)
            {
                var bookCopies = (ICollection<BookCopy>)selectedItem.GetType().GetProperty("bookCopy").GetValue(selectedItem);
                List<string> bids = bookCopies.Select(bookCopy => bookCopy.Id).ToList();
                foreach (var bid in bids)
                {
                    LoadBook(bid);
                }
                var status = (string)selectedItem.GetType().GetProperty("Status").GetValue(selectedItem);
                switch (status)
                {
                    case "Đã mượn":
                        cbxChangeStatus.Items.Remove("Chưa mượn");
                        cbxChangeStatus.Items.Remove("Đã hủy");
                        cbxChangeStatus.Text = status;
                        break;
                    case "Chưa mượn":
                        cbxChangeStatus.Items.Remove("Đã trả");
                        cbxChangeStatus.Items.Remove("Trễ hạn");
                        cbxChangeStatus.Text = status;
                        break;
                    case "Đã trả":
                        cbxChangeStatus.Items.Remove("Chưa mượn");
                        cbxChangeStatus.Items.Remove("Trễ hạn");
                        cbxChangeStatus.Items.Remove("Đã mượn");
                        cbxChangeStatus.Items.Remove("Đã hủy");
                        cbxChangeStatus.Text = status;
                        break;
                    case "Trễ hạn":
                        cbxChangeStatus.Items.Remove("Chưa mượn");
                        cbxChangeStatus.Items.Remove("Đã mượn");
                        cbxChangeStatus.Items.Remove("Đã hủy");
                        cbxChangeStatus.Text = status;
                        break;
                    case "Đã hủy":
                        cbxChangeStatus.Items.Remove("Chưa mượn");
                        cbxChangeStatus.Items.Remove("Đã trả");
                        cbxChangeStatus.Items.Remove("Trễ hạn");
                        cbxChangeStatus.Items.Remove("Đã mượn");
                        cbxChangeStatus.Text = status;
                        break;
                }
                var oId = (int)selectedItem.GetType().GetProperty("Id").GetValue(selectedItem);
                var punish = LMS_PRN221Context.Ins.Penalties.FirstOrDefault(x => x.Oid == oId);
                if (punish != null)
                {

                    txtReason.Text = punish.Reason;
                }
            }
        }

        private void LoadBook(string Bid)
        {
            var book = LMS_PRN221Context.Ins.BookCopies.Include(x => x.BookTitle).Where(x => x.Id == Bid)
                .Select(x => new
                {
                    Bid = x.Id,
                    Name = x.BookTitle.Bname,
                    Status = x.Status == false ? "Đang sử dụng" : "Có thể sử dụng",
                    Note = x.Note
                }).ToList();

            if (lvDisplayBook.ItemsSource == null)
            {
                lvDisplayBook.ItemsSource = bookDisplayList;
            }

            foreach (var item in book)
            {
                bookDisplayList.Add(item);
            }
        }

        private void lvDisplayBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var selectedItem = lvDisplay.SelectedItem;
                if (selectedItem != null)
                {
                    var borrowId = (int)selectedItem.GetType().GetProperty("Id").GetValue(selectedItem);
                    var borrow = LMS_PRN221Context.Ins.BorrowInformations.Include(x => x.Bids).FirstOrDefault(x => x.Oid == borrowId);
                    var statusUpdate = cbxChangeStatus.SelectedItem;
                    switch (statusUpdate)
                    {
                        case "Đã mượn":
                            if (borrow.Status != 1)
                            {
                                borrow.BorrowDate = DateTime.Now;
                                borrow.DueDate = DateTime.Now.AddDays(7);
                                borrow.Status = 1;

                                var librarianOrder = new LibrarianOrder
                                {
                                    LibrarianId = (int)Application.Current.Properties["LibrarianId"], // Dùng LibrarianId từ lớp CurrentUser
                                BorrowOrderId = borrowId,
                                    AssignmentDate = DateTime.Now,
                                    Status = true // Hoặc giá trị khác tùy thuộc vào logic của bạn
                                };
                                LMS_PRN221Context.Ins.LibrarianOrders.Add(librarianOrder);
                                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            break;
                        case "Đã trả":
                            if (borrow.Status != 2)
                            {
                                var book = LMS_PRN221Context.Ins.BorrowInformations.Include(b => b.Bids)
                                 .ThenInclude(copy => copy.BookTitle)
                                 .FirstOrDefault(x => x.Oid == borrowId);
                                foreach (var bid in borrow.Bids)
                                {
                                    bid.Status = false;
                                }
                                borrow.ReturnDate = DateTime.Now;
                                borrow.Status = 3;
                                var bookCopyId = borrow.Bids.Select(copy => copy.Id).First(); ;
                                //var bookCopy = book.Bids.Any(x => x.Id.Equals());
                                var bookCopy = LMS_PRN221Context.Ins.BookCopies.FirstOrDefault(x => x.Id.Equals(bookCopyId));
                                bookCopy.Status = true;
                                var bookTitle = LMS_PRN221Context.Ins.BookTitles.FirstOrDefault(x => x.Id.Equals(borrow.Bids.Select(copy => copy.BookTitleId).First()));
                                bookTitle.UnitInStock++;
                                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            break;
                        case "Trễ hạn":
                            if (borrow.Status != 4)
                            {
                                var user = LMS_PRN221Context.Ins.Users.FirstOrDefault(x => x.Uid == borrow.Uid);
                                if (user != null)
                                {
                                    //var book = LMS_PRN221Context.Ins.BorrowInformations.Include(b => b.Bids)
                                    // .ThenInclude(copy => copy.BookTitle)
                                    // .FirstOrDefault(x => x.Oid == borrowId);
                                    var bookNames = borrow.Bids.Select(copy => copy.BookTitle.Bname);
                                    SendEmailService sendEmail = new SendEmailService();
                                    sendEmail.SendEmail(user.Email, "Thông báo trễ hạn trả sách", "Thư viện xin thông báo bạn đã quá hạn trả sách." +
                                        "Bạn vui lòng đến thư viện để trả lại sách: " + bookNames);
                                    MessageBox.Show("Thông báo đã được gửi đến người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                borrow.Status = 4;
                            }
                            break;
                        case "Đã hủy":
                            if (borrow.Status != 5)
                            {
                                var user = LMS_PRN221Context.Ins.Users.FirstOrDefault(x => x.Uid == borrow.Uid);
                                if (user != null)
                                {
                                    var book = LMS_PRN221Context.Ins.BorrowInformations.Include(b => b.Bids)
                                 .ThenInclude(copy => copy.BookTitle)
                                 .FirstOrDefault(x => x.Oid == borrowId);
                                    foreach (var bid in borrow.Bids)
                                    {
                                        bid.Status = false;
                                    }
                                    borrow.ReturnDate = DateTime.Now;
                                    borrow.Status = 3;
                                    var bookCopyId = borrow.Bids.Select(copy => copy.Id).First(); ;
                                    //var bookCopy = book.Bids.Any(x => x.Id.Equals());
                                    var bookCopy = LMS_PRN221Context.Ins.BookCopies.FirstOrDefault(x => x.Id.Equals(bookCopyId));
                                    bookCopy.Status = true;
                                    var bookTitle = LMS_PRN221Context.Ins.BookTitles.FirstOrDefault(x => x.Id.Equals(borrow.Bids.Select(copy => copy.BookTitleId).First()));
                                    bookTitle.UnitInStock++;
                                    var bookNames = borrow.Bids.Select(copy => copy.BookTitle.Bname);
                                    SendEmailService sendEmail = new SendEmailService();
                                    sendEmail.SendEmail(user.Email, "Thông báo quá hạn mượn sách", "Thư viện xin thông báo đơn sách bạn đăng ký mượn đã bị hủy vì quá hạn." +
                                        "Để mượn sách bạn vui lòng tạo đơn khác");
                                    MessageBox.Show("Thông báo đã được gửi đến người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                borrow.Status = 5;
                            }
                            break;
                    }
                    LMS_PRN221Context.Ins.SaveChanges();
                    LoadBorrowInfor();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void Filter()
        {
            var query = LMS_PRN221Context.Ins.BorrowInformations.Include(x => x.UidNavigation).AsQueryable();
            string searchTerm = txbSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.UidNavigation.FullName.Contains(searchTerm) || x.UidNavigation.Email.Contains(searchTerm));
            }
            if (searchTerm.Equals(""))
            {
                LoadBorrowInfor();
            }
            if (dpFromDate.SelectedDate.HasValue)
            {
                query = query.Where(x => x.BorrowDate >= dpFromDate.SelectedDate.Value
                    || x.DueDate >= dpFromDate.SelectedDate.Value
                    || x.CheckoutDate >= dpFromDate.SelectedDate.Value);
            }
            if (dpToDate.SelectedDate.HasValue)
            {
                query = query.Where(x => x.BorrowDate <= dpToDate.SelectedDate.Value
                    || x.DueDate <= dpToDate.SelectedDate.Value
                    || x.CheckoutDate <= dpToDate.SelectedDate.Value);
            }
            if (cbxFilterStatus.SelectedIndex != null)
            {
                var selectedStatus = cbxFilterStatus.SelectedItem;
                if (selectedStatus.Equals("Tất cả"))
                {
                    query = query;
                }
                if (selectedStatus.Equals("Đã mượn"))
                {
                    query = query.Where(x => x.Status == 1);
                }
                if (selectedStatus.Equals("Chưa mượn"))
                {
                    query = query.Where(x => x.Status == 2);
                }
                if (selectedStatus.Equals("Đã trả"))
                {
                    query = query.Where(x => x.Status == 3);
                }
                if (selectedStatus.Equals("Trễ hạn"))
                {
                    query = query.Where(x => x.Status == 4);
                }
                if (selectedStatus.Equals("Đã hủy"))
                {
                    query = query.Where(x => x.Status == 5);
                }
            }
            var query1 = query.Select(x => new
            {
                Id = x.Oid,
                Uid = x.Uid,
                FullName = x.UidNavigation.FullName,
                Email = x.UidNavigation.Email,
                CheckoutDate = x.CheckoutDate,
                BorrowDate = x.BorrowDate,
                DueDate = x.DueDate,
                ReturnDate = x.ReturnDate,
                Status = x.Status == 1 ? "Đã mượn" : (x.Status == 2 ? "Chưa mượn" : (x.Status == 3 ? "Đã trả" : (x.Status == 4 ? "Trễ hạn" : "Đã hủy"))),
                Phone = x.UidNavigation.Phone,
                Punish = x.PenaltyAmount,
                bookCopy = x.Bids,
                IsOverDue = x.DueDate < DateTime.Now ? "true" : "false",

            }).ToList();
            lvDisplay.ItemsSource = query1;
        }
        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void dpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            Filter();
        }

        private void dpToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cbxFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }
    }
}
