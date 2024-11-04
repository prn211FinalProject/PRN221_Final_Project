using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                Status = x.Status == false ? "Đã mượn" : "Chưa mượn",
                Phone = x.UidNavigation.Phone,
                bookCopy = x.Bids
            }).ToList();
            lvDisplay.ItemsSource = borrow;
        }
        private ObservableCollection<object> bookDisplayList = new ObservableCollection<object>();
        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                rdbBorrowed.IsChecked = status.Equals("Đã mượn");
                rdbNotBorrow.IsChecked = status.Equals("Chưa mượn");
                if (status.Equals("Đã mượn"))
                {
                    rdbNotBorrow.IsEnabled = false;
                    rdbBorrowed.IsEnabled = false;
                    stackChangeStatus.Visibility = Visibility.Hidden;
                }
                else
                {
                    stackChangeStatus.Visibility = Visibility.Visible;
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
                    Status = x.Status == true ? "Đang sử dụng" : "Có thể sử dụng",
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

            var selectedItem = lvDisplay.SelectedItem;
            if (selectedItem != null)
            {
                var borrowId = (int)selectedItem.GetType().GetProperty("Id").GetValue(selectedItem);
                var borrow = LMS_PRN221Context.Ins.BorrowInformations.FirstOrDefault(x => x.Oid == borrowId);
                borrow.Status = rdbNotBorrow.IsChecked;
                borrow.BorrowDate = DateTime.Now;
                LMS_PRN221Context.Ins.SaveChanges();
                LoadBorrowInfor();
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
                query = query.Where(x=> x.BorrowDate >= dpFromDate.SelectedDate.Value
                    || x.DueDate >= dpFromDate.SelectedDate.Value
                    || x.CheckoutDate >= dpFromDate.SelectedDate.Value);
            }
            if (dpToDate.SelectedDate.HasValue)
            {
                query = query.Where(x => x.BorrowDate <= dpToDate.SelectedDate.Value
                    || x.DueDate <= dpToDate.SelectedDate.Value
                    || x.CheckoutDate <= dpToDate.SelectedDate.Value);
            }
            if(cbxFilterStatus.SelectedIndex != null)
            {
                if(!cbxFilterStatus.SelectedItem.Equals("Tất cả"))
                {
                    var selectedStatus = cbxFilterStatus.SelectedItem.Equals("Chưa mượn") ? true : false;
                    query = query.Where(x => x.Status == selectedStatus);
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
                Status = x.Status == false ? "Đã mượn" : "Chưa mượn",
                Phone = x.UidNavigation.Phone,
                bookCopy = x.Bids
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
