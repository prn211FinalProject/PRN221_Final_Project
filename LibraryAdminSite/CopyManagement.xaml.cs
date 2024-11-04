using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Shapes;

namespace LibraryAdminSite
{
    /// <summary>
    /// Interaction logic for CopyManagement.xaml
    /// </summary>
    public partial class CopyManagement : Window
    {
        public CopyManagement(int bookId)
        {
            InitializeComponent();
            LoadCopy(bookId);
            LoadStatus();
        }
        public int bookID;
        private void LoadStatus()
        {
            var statuses = new List<string> { "Có Thể Mượn", "Đang Cho Mượn" };
            cbxStatus.ItemsSource = statuses;
        }
        private void LoadCopy(int bookId)
        {
            var copybooks = LMS_PRN221Context.Ins.BookCopies.Include(x=>x.BookTitle).Where(bc => bc.BookTitleId == bookId).Select(x => new
            {
                Id = x.Id,
                Name = x.BookTitle.Bname,
                Status = (x.Status ?? false) ? "Có Thể Mượn" : "Đang Cho Mượn",
                Note = x.Note
            }).ToList();
            bookID = bookId;
            lvDisplay.ItemsSource = copybooks;
        }
     

        private void lvDisplay_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var selectedCopyBook = lvDisplay.SelectedItem as dynamic;
            if (selectedCopyBook != null)
            {
                cbxStatus.SelectedValue = selectedCopyBook.Status;
            }
        }

        private void cbxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedCopyBook = lvDisplay.SelectedItem as dynamic;
            if (selectedCopyBook == null)
            {
                MessageBox.Show("Vui lòng chọn một thể loại để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string bookIDCopy = selectedCopyBook.Id;
            string Note = txbNote.Text.Trim();

            var CopyBooktoUpdate = LMS_PRN221Context.Ins.BookCopies.Find(bookIDCopy);
            if (CopyBooktoUpdate != null)
            {
                CopyBooktoUpdate.Note = Note;
                CopyBooktoUpdate.Status = cbxStatus.SelectedValue.ToString() == "Có Thể Mượn";

                LMS_PRN221Context.Ins.SaveChanges();
                MessageBox.Show("Cập Nhật Thành Công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                LoadCopy(bookID);
            }
            else
            {
                MessageBox.Show("Không tìm thấy thể loại để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedCopyBook = lvDisplay.SelectedItem as dynamic;
            if (selectedCopyBook == null)
            {
                MessageBox.Show("Vui lòng chọn một thể loại để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string copyBookId = selectedCopyBook.Id;
            string Note = txbNote.Text.Trim();
            var CopyBooktoUpdate = LMS_PRN221Context.Ins.BookCopies.Find(copyBookId);

            if (CopyBooktoUpdate.Status == false)
            {
                MessageBox.Show("Sách Đang Được Cho Mượn , Không Thể Xóa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                LMS_PRN221Context.Ins.BookCopies.Remove(CopyBooktoUpdate);
                LMS_PRN221Context.Ins.SaveChanges();
                MessageBox.Show("Xóa Thành Công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                LoadCopy(bookID);

            }
        }
    }
}
