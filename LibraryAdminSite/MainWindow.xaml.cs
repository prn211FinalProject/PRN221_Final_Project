using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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
            LoadGenres();
            LoadStatus();
            LoadNXB();
        }
        private void LoadStatus()
        {
            var statuses = new List<string> { "Còn hàng", "Hết hàng" };
            cbxStatus.ItemsSource = statuses;
        }
        private void LoadNXB()
        {
            var NXB = LMS_PRN221Context.Ins.Publishers.Select(x => x.Pname).ToList();
            cbxNXB.ItemsSource = NXB;

        }
        private void LoadBook()
        {
            var books = LMS_PRN221Context.Ins.Books.Include(x => x.Publisher).Include(x => x.CidNavigation).Where(x => x.Hide == false) .Select(x => new
            {
                Id = x.Id,
                Name = x.Bname,
                Genre = x.CidNavigation.Cname,
                Publisher = x.Publisher.Pname,
                Author = x.Author,
                PublishDate = x.PublishDate.Value.ToString("dd/MM/yyyy"),
                Image = x.Image ,// Assuming this is a relative path to the image
                Quantity = x.Quantity,
                Status = x.Status,
                Price = x.Price
            }).ToList();

            lvDisplay.ItemsSource = books;
        }

        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = lvDisplay.SelectedItem as dynamic;

            if (selectedBook != null)
            {
                // Hiển thị thông tin sách
                imgBookCover.Source = ConvertToImage(selectedBook.Image);
                cbxStatus.SelectedValue = selectedBook.Status ? "Còn hàng" : "Hết hàng"; // Cập nhật trạng thái
            }
        }

        private string imagePath;
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
                MessageBox.Show($"Ảnh không tìm thấy: {fullImagePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return image;
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; // Bộ lọc cho loại file hình ảnh
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName; // Lưu đường dẫn tạm thời
                                                     // Hiển thị ảnh trong giao diện người dùng (nếu cần)
                imgBookCover.Source = new BitmapImage(new Uri(imagePath));
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
                Owner = this 
            };
            addBookWindow.Closed += (s, args) => LoadBook();
            addBookWindow.ShowDialog();
        }

        private void LoadGenres()
        {
            var genres = LMS_PRN221Context.Ins.Categories.Select(x => x.Cname).ToList();
            cbxGenre.ItemsSource = genres;

        }



        private void UpdateBook(object sender, RoutedEventArgs e)
        {
            Book book = getBook(); 
            if (book != null)
            {
                var existingBook = LMS_PRN221Context.Ins.Books.Find(book.Id);
                if (existingBook != null)
                {
                    existingBook.Bname = book.Bname;
                    existingBook.Author = book.Author;
                    existingBook.Cid = book.Cid;
                    existingBook.PublisherId = book.PublisherId;

                    existingBook.PublishDate = book.PublishDate;
                    existingBook.Quantity = book.Quantity;
                    existingBook.Status = book.Status;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        existingBook.Image = imagePath; // Lưu đường dẫn ảnh
                    }
                    LMS_PRN221Context.Ins.SaveChanges();
                    MessageBox.Show("Cập nhật thành công!");
                    LoadBook(); 
                }
                else
                {
                    MessageBox.Show("Sách không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng điền thông tin hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private Book getBook()
        {
            try
            {
                int id = int.Parse(txbID.Text); // Tên điều khiển là txbID
                string name = txbBName.Text; // Tên điều khiển là txbBName
                string author = txbAuthor.Text; // Tên điều khiển là txbAuthor
                DateTime? publishDate = dpkPdate.SelectedDate; // Tên điều khiển là dpkPdate
                int? genreID = LMS_PRN221Context.Ins.Categories
                    .FirstOrDefault(x => x.Cname.Equals(cbxGenre.SelectedItem.ToString()))?.Id;
                int quantity = int.Parse(txbQuantity.Text); // Tên điều khiển là txbQuantity
                int? pID = LMS_PRN221Context.Ins.Publishers
                    .FirstOrDefault(x => x.Pname.Equals(cbxNXB.SelectedItem.ToString()))?.Id;
                bool status = cbxStatus.SelectedValue.ToString() == "Còn hàng"; // Tên điều khiển là cbxStatus
                return new Book()
                {
                    Id = id,
                    Bname = name,
                    Author = author,
                    PublishDate = publishDate ?? DateTime.Now, // Sử dụng giá trị hiện tại nếu không có ngày xuất bản
                    PublisherId = pID,
                    Cid = genreID,
                    Quantity = quantity,
                    Status = status
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; // Trả về null nếu có lỗi
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            Book book = getBook();
            if (book != null)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa cuốn sách : "+book.Bname,
                                             "Xác nhận xóa",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var existingBook = LMS_PRN221Context.Ins.Books.Find(book.Id);
                    if (existingBook != null)
                    {
                        existingBook.Hide = true; // Đổi thuộc tính Hide thành true
                        LMS_PRN221Context.Ins.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                        MessageBox.Show("Xóa sách thành công!");
                        LoadBook(); // Cập nhật lại danh sách sách
                    }
                    else
                    {
                        MessageBox.Show("Sách không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Xóa đã bị hủy.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sách để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
