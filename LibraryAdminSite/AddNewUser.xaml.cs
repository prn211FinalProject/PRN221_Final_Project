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
using System.Windows.Shapes;

namespace LibraryAdminSite
{
    /// <summary>
    /// Interaction logic for AddNewUser.xaml
    /// </summary>
    public partial class AddNewUser : Window
    {
        public AddNewUser()
        {
            InitializeComponent();
        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkUser = LMS_PRN221Context.Ins.Users.FirstOrDefault(x=>x.Email == txtEmail.Text);
                if (checkUser == null)
                {
                    Role role = LMS_PRN221Context.Ins.Roles.FirstOrDefault(x => x.Name == cbxRole.Text);
                    User user = new User
                    {
                        FullName = txtName.Text,
                        Email = txtEmail.Text,
                        Gender = rdbMale.IsChecked,
                        Phone = txtPhone.Text,
                        RoleId = role.Id,
                        Password = pwdBox.Password
                    };
                    LMS_PRN221Context.Ins.Add(user);
                    LMS_PRN221Context.Ins.SaveChanges();
                    UserManage userManage = new UserManage();
                    userManage.LoadUser();
                    MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MessageBox.Show("Thêm người dùng không thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRole();
        }
        private void LoadRole()
        {
            var role = LMS_PRN221Context.Ins.Roles.Select(x => x.Name).ToList();
            cbxRole.ItemsSource = role;
        }

        private void cbxRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbxRole.SelectedItem;
            if (selectedItem != null)
            {

                if (selectedItem.ToString() == "Admin")
                {
                    lblPassword.Visibility = Visibility.Visible;
                    pwdBox.Visibility = Visibility.Visible;
                }
                else
                {
                    lblPassword.Visibility = Visibility.Collapsed;
                    pwdBox.Visibility = Visibility.Collapsed;
                    pwdBox.Password = "123456";
                }
            }
        }
    }
}
