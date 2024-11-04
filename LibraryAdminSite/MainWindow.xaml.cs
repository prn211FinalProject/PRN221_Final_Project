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
        }



        private void btnUserManage_Click(object sender, RoutedEventArgs e)
        {
            var userManageControl = new UserManage();

            // Load the UserManage control into the ContentControl
            MainContentControl.Content = userManageControl;
        }

        private void btnBookManage_Click(object sender, RoutedEventArgs e)
        {
            var bookManager = new BookManagement();

            // Load the UserManage control into the ContentControl
            MainContentControl.Content = bookManager;
        }

        private void btnCateManage_Click(object sender, RoutedEventArgs e)
        {
            var cateManager = new CategoryManagement();

            // Load the UserManage control into the ContentControl
            MainContentControl.Content = cateManager;
        }

        private void btnNXBManage_Click(object sender, RoutedEventArgs e)
        {
            var NXBManager = new NXBManagement();

            // Load the UserManage control into the ContentControl
            MainContentControl.Content = NXBManager;
        }

        private void btnBlogManage_Click(object sender, RoutedEventArgs e)
        {
            var BlogManager = new BlogManager();

            // Load the UserManage control into the ContentControl
            MainContentControl.Content = BlogManager;
        }

        private void btnBookThongKe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnThongKeSachMuon_Click(object sender, RoutedEventArgs e)
        {
            var thongkeSachCate = new ThongKeSachMuon();

            // Load the UserManage control into the ContentControl
            MainContentControl.Content = thongkeSachCate;
        }
    }
}
