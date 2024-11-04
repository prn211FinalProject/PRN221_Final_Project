using LibraryAdminSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibraryAdminSite
{
    public partial class BlogManager : UserControl
    {
        private int? selectetedBlogId = null;

        public BlogManager()
        {
            InitializeComponent();
            LoadStatus();
            LoadBlog();
        }

        private void LoadStatus()
        {
            var statuses = new List<string> { "Hiển Thị", "Ẩn Đi" };
            cbxStatus.ItemsSource = statuses;
        }

        private void LoadBlog()
        {
            var books = LMS_PRN221Context.Ins.Blogs.Select(x => new
            {
                Id = x.Bid,
                Tittle = x.Title,
                Content = x.Content,
                PDate = x.CreatedDate,
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
                selectetedBlogId = selectedCategory.Id;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectetedBlogId == null)
            {
                MessageBox.Show("Vui lòng chọn thông báo để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var blogToUpdate = LMS_PRN221Context.Ins.Blogs.FirstOrDefault(x => x.Bid == selectetedBlogId);
            if (blogToUpdate != null)
            {
                blogToUpdate.Title = txbTittle.Text.Trim();
                blogToUpdate.Content = txbContent.Text.Trim();
                blogToUpdate.CreatedDate = dpkPdate.SelectedDate ?? blogToUpdate.CreatedDate;
                blogToUpdate.Status = cbxStatus.SelectedValue?.ToString() == "Hiển Thị";

                LMS_PRN221Context.Ins.SaveChanges();

                LoadBlog();

                MessageBox.Show("Cập nhật bài viết thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy bài viết để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddNewBook_Click(object sender, RoutedEventArgs e)
        {
            string title = txbTittle.Text.Trim();
            string content = txbContent.Text.Trim();
            DateTime? createdDate = dpkPdate.SelectedDate;

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content) && createdDate.HasValue)
            {
                var newBlog = new Blog
                {
                    Title = title,
                    Content = content,
                    CreatedDate = createdDate.Value,
                    Status = cbxStatus.SelectedItem?.ToString() == "Hiển Thị"
                };

                LMS_PRN221Context.Ins.Blogs.Add(newBlog);
                LMS_PRN221Context.Ins.SaveChanges();
                LoadBlog();
                MessageBox.Show("Thêm mới bài viết thành công!");
            }
            else
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
            }
        }
    }
}
