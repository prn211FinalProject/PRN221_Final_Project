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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newBook = new BookTitle
            {
                Bname = txtBookName.Text,
                Author = txtAuthor.Text,
                PublishDate = dpPublishDate.SelectedDate,
                Cid = (cmbGenre.SelectedItem as Category)?.Id,
                PublisherId = (cbxNXB.SelectedItem as Publisher)?.Id,
                Status = true,
                Hide = false,
                Quantity = int.Parse(txtQuantity.Text),
                UnitInStock = int.Parse(txtQuantity.Text),
                Price = decimal.Parse(txtPrice.Text)
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