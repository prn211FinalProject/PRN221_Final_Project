using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
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
        private void UpdateBook(object sender, RoutedEventArgs e)
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

        }

        private void btnBorrowManage_Click(object sender, RoutedEventArgs e)
        {
            var BorrowManageControl = new BorrowManage();
            MainContentControl.Content = BorrowManageControl;
        }
    }
}
