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
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Policy;
using System.Xml.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace LibraryAdminSite
{
    /// <summary>
    /// Interaction logic for BookManagement.xaml
    /// </summary>
    public partial class BookManagement : UserControl
    {

        private List<string> selectedGenre = new List<string>(); 
        private List<string> selectedNXB = new List<string>(); 
        private CollectionViewSource _bookCollectionViewSource;
        public BookManagement()
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

            NXB.Insert(0, "All");
            cbxNXBFilter.ItemsSource = NXB;

            cbxNXBFilter.SelectedIndex = 0;

        }
        private void LoadBook()
        {
            var books = LMS_PRN221Context.Ins.BookTitles.Include(x => x.Publisher).Include(x => x.CidNavigation).Where(x => x.Hide == false).Where(x => x.CidNavigation.Status == true).Select(x => new
            {
                Id = x.Id,
                ISBN = x.Isbn,
                Name = x.Bname,
                Genre = x.CidNavigation.Cname,
                Publisher = x.Publisher.Pname,
                Author = x.Author,
                PublishDate = x.PublishDate,
                Image = x.Image,
                Quantity = LMS_PRN221Context.Ins.BookCopies.Count(bc => bc.BookTitleId == x.Id),
                UnitStock = x.UnitInStock,
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
                imgBookCover.Source = ConvertToImage(selectedBook.Image);
                cbxStatus.SelectedValue = selectedBook.Status ? "Còn hàng" : "Hết hàng"; 
            }
        }

        private string imagePath;
        public static BitmapImage ConvertToImage(string relativeImagePath)
        {
            BitmapImage image = new BitmapImage();
            string projectDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", @"..", @"..", "BookImage");
            string fullImagePath = System.IO.Path.Combine(projectDirectory, relativeImagePath);
            if (File.Exists(fullImagePath)) 
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fullImagePath, UriKind.Absolute); 
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
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; 
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName; 
                                                     
                imgBookCover.Source = new BitmapImage(new Uri(imagePath));
            }
        }


     

        private void OpenAddForm(object sender, RoutedEventArgs e)
        {
            AddNewBook addNewBookWindow = new AddNewBook();
            addNewBookWindow.BookAdded += LoadBook;
            addNewBookWindow.Show(); 
        }

        private void LoadGenres()
        {
            var genres = LMS_PRN221Context.Ins.Categories.Where(x => x.Status == true).Select(x => x.Cname).ToList();
            cbxGenre.ItemsSource = genres;
            genres.Insert(0, "All");

            cbxGenreFilter.ItemsSource = genres;
            cbxGenreFilter.SelectedIndex = 0;

        }



        private void UpdateBook(object sender, RoutedEventArgs e)
        {
            BookTitle book = getBook();
            if (book != null)
            {
                var existingBook = LMS_PRN221Context.Ins.BookTitles.Find(book.Id);
                if (existingBook != null)
                {
                    existingBook.Bname = book.Bname;
                    existingBook.Author = book.Author;
                    existingBook.Cid = book.Cid;
                    existingBook.PublisherId = book.PublisherId;
                    existingBook.PublishDate = book.PublishDate;
                    existingBook.Status = book.Status;

                    // Kiểm tra nếu có đường dẫn ảnh mới
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        existingBook.Image = imagePath;
                    }

                    int newQuantity = book.Quantity; // Lấy số lượng mới từ đối tượng book
                    int currentQuantity = existingBook.Quantity; // Lấy số lượng hiện tại từ đối tượng existingBook

                    // Cập nhật số lượng sách trong cơ sở dữ liệu
                    existingBook.Quantity = newQuantity; 

                    // Kiểm tra xem số lượng mới có lớn hơn số lượng hiện có không
                    if (newQuantity > currentQuantity)
                    {
                        int quantityToAdd = newQuantity - currentQuantity; // Tính số bản sao cần thêm
                        MessageBox.Show("Quantity to add = " + quantityToAdd);
                        for (int i = 1; i <= quantityToAdd; i++)
                        {
                            // Tạo ID cho bản sao, sử dụng số lượng hiện có của bản sao để tạo ID duy nhất
                            string copyId = $"CP{existingBook.Id}-{(LMS_PRN221Context.Ins.BookCopies.Count(bc => bc.BookTitleId == existingBook.Id) + i):D3}"; // Tạo ID cho bản sao
                            BookCopy newCopy = new BookCopy
                            {
                                Id = copyId,
                                BookTitleId = existingBook.Id, // Liên kết bản sao với sách
                                Status = true,
                                Note = "New"
                            };
                            LMS_PRN221Context.Ins.BookCopies.Add(newCopy);
                        }
                    }
                    else if (newQuantity < currentQuantity)
                    {
                        MessageBox.Show("Số lượng mới không thể nhỏ hơn số lượng hiện tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Ngăn không cho tiếp tục cập nhật
                    }

                    LMS_PRN221Context.Ins.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu
                    MessageBox.Show("Cập nhật thành công!");
                    LoadBook(); // Tải lại danh sách sách
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




        private void FilterBook()
        {
            var query = LMS_PRN221Context.Ins.BookTitles.Include(x => x.CidNavigation).Include(x => x.Publisher).Where(x => x.Hide == false).Where(x => x.CidNavigation.Status == true).AsQueryable();

          

           
            if (cbxGenreFilter.SelectedValue != null && cbxGenreFilter.SelectedValue.ToString() != "All")
            {
                string selectedDepartment = cbxGenreFilter.SelectedValue.ToString();
                query = query.Where(x => x.CidNavigation.Cname == selectedDepartment);
            }

            if (cbxNXBFilter.SelectedValue != null && cbxNXBFilter.SelectedValue.ToString() != "All")
            {
                string selectedNXB = cbxNXBFilter.SelectedValue.ToString();
                query = query.Where(x => x.Publisher.Pname == selectedNXB);
            }

            if (dpFromDate.SelectedDate.HasValue)
            {
                DateTime fromDate = dpFromDate.SelectedDate.Value;
                query = query.Where(x => x.PublishDate >= fromDate);
            }
            if (dpToDate.SelectedDate.HasValue)
            {
                DateTime toDate = dpToDate.SelectedDate.Value;
                query = query.Where(x => x.PublishDate <= toDate);
            }

            string searchTerm = txbSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Bname.Contains(searchTerm) || x.Author.Contains(searchTerm));
            }
            if (decimal.TryParse(txbMinPrice.Text, out decimal minPrice))
            {
                query = query.Where(x => x.Price >= minPrice);
            }
            if (decimal.TryParse(txbMaxPrice.Text, out decimal maxPrice))
            {
                query = query.Where(x => x.Price <= maxPrice);
            }

            if (cmbSortByPrice.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Tag.ToString() == "asc")
                {
                    query = query.OrderBy(x => x.Price);
                }
                else if (selectedItem.Tag.ToString() == "desc")
                {
                    query = query.OrderByDescending(x => x.Price);
                }
            }
            if (cmbSortByQuantity.SelectedItem is ComboBoxItem selectedItem2)
            {
                if (selectedItem2.Tag.ToString() == "asc")
                {
                    query = query.OrderBy(x => x.Quantity);
                }
                else if (selectedItem2.Tag.ToString() == "desc")
                {
                    query = query.OrderByDescending(x => x.Quantity);
                }
            }

            var filteredStudents = query.Select(x => new
            {
                Id = x.Id,
                Name = x.Bname,
                ISBN = x.Isbn,
                Genre = x.CidNavigation.Cname,
                Publisher = x.Publisher.Pname,
                Author = x.Author,
                PublishDate = x.PublishDate.Value.ToString("dd/MM/yyyy"),
                Image = x.Image,
                Quantity = LMS_PRN221Context.Ins.BookCopies.Count(bc => bc.BookTitleId == x.Id),
                Status = x.Status,
                UnitStock = x.UnitInStock,
                Price = x.Price,
            }).ToList();

            lvDisplay.ItemsSource = filteredStudents;
        }


        private BookTitle getBook()
        {
            try
            {
                int id = int.Parse(txbID.Text); 
                string name = txbBName.Text; 
                string author = txbAuthor.Text; 
                DateTime? publishDate = dpkPdate.SelectedDate; 
                int? genreID = LMS_PRN221Context.Ins.Categories
                    .FirstOrDefault(x => x.Cname.Equals(cbxGenre.SelectedItem.ToString()))?.Id;
                int quantity = int.Parse(txbQuantity.Text); 
                int? pID = LMS_PRN221Context.Ins.Publishers
                    .FirstOrDefault(x => x.Pname.Equals(cbxNXB.SelectedItem.ToString()))?.Id;
                bool status = cbxStatus.SelectedValue.ToString() == "Còn hàng"; 
                return new BookTitle()
                {
                    Id = id,
                    Bname = name,
                    Author = author,
                    PublishDate = publishDate ?? DateTime.Now, 
                    PublisherId = pID,
                    Cid = genreID,
                    Quantity = quantity,
                    Status = status
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; 
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
			BookTitle book = getBook();
            if (book != null)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa cuốn sách : " + book.Bname,
                                              "Xác nhận xóa",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var existingBook = LMS_PRN221Context.Ins.BookTitles
                        .Include(b => b.BookCopies) 
                        .FirstOrDefault(b => b.Id == book.Id); 

                    if (existingBook != null)
                    {
                        bool hasNotAvailableCopies = existingBook.BookCopies.Any(bc => bc.Status == false);
                        if (hasNotAvailableCopies)
                        {
                            MessageBox.Show("Không thể xóa sách vì còn bản sao đang cho mượn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            existingBook.Hide = true; 
                            LMS_PRN221Context.Ins.SaveChanges();
                            MessageBox.Show("Xóa sách thành công!");
                            LoadBook();
                        }
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


        private void cbxGenreFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBook();
        }

        private void cbxNXBFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBook();

        }

        private void dpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBook();

        }

        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterBook();

        }

        private void txbMinPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterBook();

        }

        private void cmbSortByPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBook();

        }

        private void cmbSortByQuantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBook();

        }

        private void btnViewCopy_Click(object sender, RoutedEventArgs e)
        {
			BookTitle book = getBook();
            if (book != null)
            {
                var bookCopyWindow = new CopyManagement(book.Id);
                bookCopyWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách để xem các bản sao.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
