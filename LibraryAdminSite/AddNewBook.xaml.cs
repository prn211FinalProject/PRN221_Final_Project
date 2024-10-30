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
        private LMS_PRN221Context _context;
        private string _selectedImagePath; // Đường dẫn ảnh đã chọn

        public AddNewBook()
        {
            InitializeComponent();
            _context = new LMS_PRN221Context();
            LoadGenres();
        }

        private void LoadGenres()
        {
            var genres = _context.Categories.Select(x => x.Cname).ToList();
            cmbGenre.ItemsSource = genres;
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
                _selectedImagePath = openFileDialog.FileName;
                imgBookCover.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lưu ảnh vào thư mục và lấy tên file ảnh
                string imageFileName = SaveImageToDirectory(_selectedImagePath);

                // Tạo đối tượng Book mới và gán các giá trị từ giao diện người dùng
                var newBook = new Book
                {
                    Bname = txtBookName.Text,
                    Author = txtAuthor.Text,
                    PublishDate = dpPublishDate.SelectedDate,
                    Cid = cmbGenre.SelectedIndex + 1, // Điều chỉnh theo ID thực tế nếu có
                    PublisherId = 1, // Gán ID nhà xuất bản mặc định, có thể thay đổi theo dữ liệu của bạn
                    Image = imageFileName, // Lưu tên file ảnh vào cơ sở dữ liệu
                    Status = true, // Giả sử sách mới thêm vào sẽ ở trạng thái khả dụng
                    Quantity = 1,  // Số lượng mặc định là 1, có thể thay đổi nếu cần
                    Price = 100000 // Giá mặc định, điều chỉnh theo nhu cầu
                };

                // Thêm sách vào cơ sở dữ liệu
                _context.Books.Add(newBook);
                _context.SaveChanges();

                MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Đóng cửa sổ sau khi thêm thành công
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string SaveImageToDirectory(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return null;

            string directoryPath = @"D:\Dai Hoc FBT\Ki_7_Fall2024\PRN221\PRN221_Final_Project.git\LibraryAdminSite\BookImage";
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagePath)}"; // Tạo tên file ảnh mới
            string destPath = Path.Combine(directoryPath, fileName);

            File.Copy(imagePath, destPath, true); // Sao chép ảnh vào thư mục lưu trữ
            return fileName; // Chỉ trả về tên file để lưu vào cơ sở dữ liệu
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
