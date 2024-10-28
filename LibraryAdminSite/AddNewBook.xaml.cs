using LibraryAdminSite.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LibraryAdminSite
{
    public partial class AddNewBook : Window
    {
        public AddNewBook()
        {
            InitializeComponent();
            LoadGenres();
        }

        private void LoadGenres()
        {
            // Giả sử bạn có một danh sách thể loại từ cơ sở dữ liệu
            using (var context = LMS_PRN221Context.Ins)
            {
                var genres = context.Categories.Select(x => x.Cname).ToList();
                cmbGenre.ItemsSource = genres;
            }
        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Chọn Ảnh Bìa"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                // Hiển thị ảnh đã chọn
                imgBookCover.Source = new BitmapImage(new Uri(selectedImagePath));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Logic để thêm sách mới vào cơ sở dữ liệu
            // ...
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
