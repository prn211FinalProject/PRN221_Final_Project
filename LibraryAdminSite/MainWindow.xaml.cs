using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LibraryAdminSite
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBook();
        }

        private void LoadBook()
        {
            var books = LMS_PRN221Context.Ins.Books.Include(x => x.Publisher).Include(x => x.CidNavigation).Select(x => new
            {
                Id = x.Id,
                Name = x.Bname,
                Genre = x.CidNavigation.Cname,
                Publisher = x.Publisher.Pname,
                Author = x.Author,
                PublishDate = x.PublishDate.Value.ToString("dd/MM/yyyy"),
                Image = x.Image // Assuming this is a relative path to the image
            }).ToList();

            lvDisplay.ItemsSource = books;
        }
        private void LoadUser()
        {
            var users = LMS_PRN221Context.Ins.Users.Select(x => new
            {
                Id = x.Uid,
                FullName = x.FullName,
                Phone = x.Phone,
                Email = x.Email,
                Role = x.Role,
            }).ToList();
            lvDisplay.ItemsSource = users;
        }
        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = lvDisplay.SelectedItem as dynamic;

            if (selectedBook != null)
            {
                // Hiển thị thông tin sách
                imgBookCover.Source = ConvertToImage(selectedBook.Image);
            }
        }

        public static BitmapImage ConvertToImage(string relativeImagePath)
        {
            BitmapImage image = new BitmapImage();
            // Đường dẫn đến thư mục chứa ảnh
            string projectDirectory = @"D:\Dai Hoc FBT\Ki_7_Fall2024\PRN221\PRN221_Final_Project.git\LibraryAdminSite\BookImage";
            string fullImagePath = Path.Combine(projectDirectory, relativeImagePath); // Chuyển đổi đường dẫn tương đối sang tuyệt đối
            if (File.Exists(fullImagePath)) // Kiểm tra xem file ảnh có tồn tại không
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fullImagePath, UriKind.Absolute); // Sử dụng đường dẫn tuyệt đối
                image.EndInit();
            }
            else
            {
                // Xử lý trường hợp ảnh không tồn tại (tùy chọn: đặt ảnh mặc định)
                MessageBox.Show($"Ảnh không tìm thấy: {fullImagePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return image;
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            // Tạo một hộp thoại chọn tệp
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"; // Chỉ cho phép chọn tệp hình ảnh
            openFileDialog.Title = "Chọn Ảnh";

            if (openFileDialog.ShowDialog() == true)
            {
                // Lấy đường dẫn tệp đã chọn
                string selectedFilePath = openFileDialog.FileName;

                // Lấy sách được chọn
                var selectedBook = lvDisplay.SelectedItem as dynamic;

                if (selectedBook != null)
                {
                    // Cập nhật hình ảnh trong cơ sở dữ liệu
                    UpdateBookImage(selectedBook.Id, selectedFilePath);

                    // Hiển thị hình ảnh mới
                    imgBookCover.Source = ConvertToImage(selectedFilePath); // Cập nhật hình ảnh ngay lập tức
                }
            }
        }


        private void UpdateBookImage(int bookId, string imagePath)
        {
            // Cập nhật hình ảnh trong cơ sở dữ liệu (cần có phương thức này trong model của bạn)
            var book = LMS_PRN221Context.Ins.Books.FirstOrDefault(b => b.Id == bookId);

            if (book != null)
            {
                book.Image = imagePath; // Cập nhật đường dẫn hình ảnh
                LMS_PRN221Context.Ins.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddNewBook addBookWindow = new AddNewBook
            {
                Owner = this // Thiết lập cửa sổ chính là chủ sở hữu
            };
            addBookWindow.ShowDialog();
        }

        private void btnUserManage_Click(object sender, RoutedEventArgs e)
        {
            //UserManage userManage = new UserManage();
            //userManage.ShowDialog();
            //this.Hide();
            txtTitle.Text = "Quản lý người dùng";
            txtSearch.Text = "Nhập tên người dùng...";
            LoadUser();
        }

        private void btnBookManage_Click(object sender, RoutedEventArgs e)
        {
            txtTitle.Text = "Quản lý sách";
            txtSearch.Text = "Nhập tên sách...";
            LoadBook();
        }
    }
}
