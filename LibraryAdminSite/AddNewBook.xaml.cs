using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LibraryAdminSite
{
    public partial class AddNewBook : Window
    {
        private LMS_PRN221Context _context;
        private string _selectedImagePath; 
        public event Action BookAdded;
        public AddNewBook()
        {
            InitializeComponent();
            _context = new LMS_PRN221Context();
            LoadGenres();
            LoadNXB();
        }

        private void LoadNXB()
        {
            var publishers = _context.Publishers.ToList();
            cbxNXB.ItemsSource = publishers;
            cbxNXB.DisplayMemberPath = "Pname"; 
            cbxNXB.SelectedValuePath = "Id"; 
        }

        private void LoadGenres()
        {
            var genres = _context.Categories.ToList();
            cmbGenre.ItemsSource = genres;
            cmbGenre.DisplayMemberPath = "Cname"; 
            cmbGenre.SelectedValuePath = "Id"; 
        }
        private string imagePath;
        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; 
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName; 
                                                   
                imgBookCover.Source = new BitmapImage(new Uri(imagePath));
            }
        }
        private string GenerateISBN(string publisherCode, string uniqueBookCode)
        {
            // Tiền tố ISBN-13 là "978"
            string isbnWithoutChecksum = "978" + publisherCode + uniqueBookCode;

            // Làm sạch chuỗi để đảm bảo chỉ có số, loại bỏ ký tự không hợp lệ (nếu có)
            isbnWithoutChecksum = new string(isbnWithoutChecksum.Where(char.IsDigit).ToArray());

            // Bổ sung số 0 nếu chuỗi có chiều dài nhỏ hơn 12 (do ISBN cần 12 số trước khi tính checksum)
            while (isbnWithoutChecksum.Length < 12)
            {
                isbnWithoutChecksum = "0" + isbnWithoutChecksum;  // Thêm số 0 vào đầu chuỗi
            }

            // Đảm bảo tổng chiều dài của ISBN-13 là 12 chữ số (trước khi tính checksum)
            if (isbnWithoutChecksum.Length != 12)
            {
                throw new FormatException("Mã ISBN phải có 12 chữ số trước khi tính checksum.");
            }

            // Tính checksum cho ISBN
            int sum = 0;
            for (int i = 0; i < isbnWithoutChecksum.Length; i++)
            {
                int digit = int.Parse(isbnWithoutChecksum[i].ToString());  // Chuyển đổi ký tự thành số nguyên
                if (i % 2 == 0)
                {
                    sum += digit;  // Vị trí lẻ, nhân với 1
                }
                else
                {
                    sum += digit * 3;  // Vị trí chẵn, nhân với 3
                }
            }

            int remainder = sum % 10;
            int checksum = (10 - remainder) % 10;

            // Trả về ISBN-13 hợp lệ với mã kiểm tra (13 chữ số)
            string isbnWithChecksum = isbnWithoutChecksum + checksum.ToString();
            return isbnWithChecksum;
        }







        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var publisher = cbxNXB.SelectedItem as Publisher;  // Lấy nhà xuất bản từ ComboBox
            if (publisher == null)
            {
                MessageBox.Show("Chưa chọn nhà xuất bản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo ISBN tự động
            string publisherCode = publisher.PublisherCode;  // Mã nhà xuất bản
            string bookUniqueCode = Guid.NewGuid().ToString("N").Substring(0, 5);  // Sử dụng 5 ký tự ngẫu nhiên cho mã sách

            // Tạo ISBN 13 chữ số
            string isbn = GenerateISBN(publisherCode, bookUniqueCode);  // Sử dụng hàm GenerateISBN để tạo mã ISBN chuẩn

            var newBook = new BookTitle
            {
                Bname = txtBookName.Text,
                Author = txtAuthor.Text,
                PublishDate = dpPublishDate.SelectedDate,
                Cid = (cmbGenre.SelectedItem as Category)?.Id,
                PublisherId = publisher.Id,  // Lưu PublisherId (để tham chiếu đến nhà xuất bản)
                Status = true,
                Hide = false,
                Quantity = int.Parse(txtQuantity.Text),
                UnitInStock = int.Parse(txtQuantity.Text),
                Price = decimal.Parse(txtPrice.Text),
                Isbn = isbn  // Gán ISBN tự động
            };

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    string projectDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", @"..", @"..", "BookImage");

                    if (!Directory.Exists(projectDirectory))
                    {
                        Directory.CreateDirectory(projectDirectory);
                    }

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagePath)}";
                    string destPath = Path.Combine(projectDirectory, fileName);

                    File.Copy(imagePath, destPath, true);

                    newBook.Image = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}");
                    return;
                }
            }

            _context.BookTitles.Add(newBook);
            _context.SaveChanges();

            for (int i = 1; i <= newBook.Quantity; i++)
            {
                string copyId = $"BC{newBook.Id}-{i:D3}";
                BookCopy newCopy = new BookCopy
                {
                    Id = copyId,
                    BookTitleId = newBook.Id,
                    Status = true,
                    Note = "New"
                };
                _context.BookCopies.Add(newCopy);
            }

            _context.SaveChanges();
            MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            BookAdded?.Invoke();
            this.Close();
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
