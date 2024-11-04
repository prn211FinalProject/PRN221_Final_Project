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
    /// Interaction logic for CategoryManagement.xaml
    /// </summary>
    public partial class CategoryManagement : UserControl
    {
        public CategoryManagement()
        {
            InitializeComponent();
            LoadCategory();
            LoadStatus();

        }
        private void LoadStatus()
        {
            var statuses = new List<string> { "Hiển Thị", "Ẩn Đi" };
            cbxStatus.ItemsSource = statuses;
        }
        private void LoadCategory()
        {
            var books = LMS_PRN221Context.Ins.Categories.Select(x => new
            {
                Id = x.Id,
                Name = x.Cname,
                Status = (x.Status ?? false) ? "Hiển Thị" : "Ẩn Đi",
            }).ToList();

            lvDisplay.ItemsSource = books;
        }

        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = lvDisplay.SelectedItem as dynamic;

            if (selectedCategory != null)
            {
                cbxStatus.SelectedValue = selectedCategory.Status; 
            }
        }


        private void btnAddNewBook_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = txbBName.Text.Trim();

            bool categoryExists = LMS_PRN221Context.Ins.Categories
                .Any(x => x.Cname.ToLower() == categoryName.ToLower());

            if (categoryExists)
            {
                MessageBox.Show("Thể loại này đã tồn tại trong cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            var newCategory = new Category
            {
                Cname = categoryName,
                Status = cbxStatus.SelectedValue.ToString() == "Hiển Thị" 
            };

            LMS_PRN221Context.Ins.Categories.Add(newCategory);
            LMS_PRN221Context.Ins.SaveChanges();

            LoadCategory();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            var selectedCategory = lvDisplay.SelectedItem as dynamic;

            if (selectedCategory == null)
            {
                MessageBox.Show("Vui lòng chọn một thể loại để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int categoryId = selectedCategory.Id;
            string categoryName = txbBName.Text.Trim();

            if (LMS_PRN221Context.Ins.Categories
                .Any(x => x.Cname.ToLower() == categoryName.ToLower() && x.Id != categoryId))
            {
                MessageBox.Show("Thể loại này đã tồn tại trong cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            var categoryToUpdate = LMS_PRN221Context.Ins.Categories.Find(categoryId);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Cname = categoryName; 
                categoryToUpdate.Status = cbxStatus.SelectedValue.ToString() == "Hiển Thị"; 

                LMS_PRN221Context.Ins.SaveChanges();
                MessageBox.Show("Cập Nhật Thành Công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                LoadCategory();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thể loại để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
    }
}
