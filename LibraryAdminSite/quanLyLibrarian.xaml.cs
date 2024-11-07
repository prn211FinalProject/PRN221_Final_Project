using LibraryAdminSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LibraryAdminSite
{
    /// <summary>
    /// Interaction logic for quanLyLibrarian.xaml
    /// </summary>
    public partial class quanLyLibrarian : UserControl
    {
        public quanLyLibrarian()
        {
            InitializeComponent();
            LoadUser();
            LoadStatus();
        }

        private void LoadStatus()
        {
            var statuses = new List<string> { "Hoạt Động", "Khóa" };
            cbxStatus.ItemsSource = statuses;
        }
        public void LoadUser()
        {
            var users = LMS_PRN221Context.Ins.Users.Where(x=>x.RoleId==1002).Select(x => new
            {
                Id = x.Uid,
                FullName = x.FullName,
                Phone = x.Phone,
                Email = x.Email,
                Role = x.Role.Name,
                Status = x.Status,
                 TotalOrdersHandled = LMS_PRN221Context.Ins.LibrarianOrders
                                    .Count(l => l.LibrarianId == x.Uid)
            }).ToList();
            lvDisplay.ItemsSource = users;
        }

        private void btnAddNewBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = lvDisplay.SelectedItem as dynamic;
            if (selectedCategory != null)
            {
                var bookCopyWindow = new ViewBorrowHandle(selectedCategory.Id);
                bookCopyWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách để xem các bản sao.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = lvDisplay.SelectedItem as dynamic;
            var librariantoupdate = LMS_PRN221Context.Ins.Users.Find(selectedCategory.Id);
            if (librariantoupdate != null)
            {
                librariantoupdate.Status = cbxStatus.SelectedValue.ToString() == "Hoạt Động";

                LMS_PRN221Context.Ins.SaveChanges();
                MessageBox.Show("Cập Nhật Thành Công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                LoadUser();
            }
        }

        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = lvDisplay.SelectedItem as dynamic;

            if (selectedCategory != null)
            {
                cbxStatus.SelectedValue = selectedCategory.Status;
            }
        }
    }
}
