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
    /// Interaction logic for UserManage.xaml
    /// </summary>
    public partial class UserManage : UserControl
    {
        public UserManage()
        {
            InitializeComponent();
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

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBook(object sender, RoutedEventArgs e)
        {

        }

        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUser();
        }
    }
}
