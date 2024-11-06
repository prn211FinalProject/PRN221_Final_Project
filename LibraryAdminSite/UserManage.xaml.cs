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
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUser();
            LoadRole();
        }
        public void LoadUser()
        {
            var users = LMS_PRN221Context.Ins.Users.Select(x => new
            {
                Id = x.Uid,
                FullName = x.FullName,
                Phone = x.Phone,
                Email = x.Email,
                Role = x.Role.Name,
                Gender = (bool)x.Gender ? "Nam" : "Nữ",
            }).ToList();
            lvDisplay.ItemsSource = users;
        }
        private void LoadRole()
        {
            var role = LMS_PRN221Context.Ins.Roles.Select(x => x.Name).ToList();
            cbxRole.ItemsSource = role;
        }
        private User getUser()
        {
            try
            {
                Role role = LMS_PRN221Context.Ins.Roles.FirstOrDefault(x => x.Name == cbxRole.Text);

                int id = int.Parse(txbID.Text);
                string name = txbBName.Text;
                bool gender = rdbMale.IsChecked == true;
                string phone = txbPhone.Text;
                string email = txbEmail.Text;
                int roleId = role.Id;
                return new User
                {
                    Uid = id,
                    Email = email,
                    RoleId = roleId,
                    FullName = name,
                    Phone = phone,
                    Gender = gender,
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = lvDisplay.SelectedItem; // Giả sử User là model của bạn
            if (selectedItem != null)
            {

                string gender = (string)selectedItem.GetType().GetProperty("Gender").GetValue(selectedItem);
                rdbMale.IsChecked = gender.Equals("Nam") ? true : false;
                rdbFemale.IsChecked = gender.Equals("Nữ") ? true : false;
            }
        }


        private void updateUser(object sender, RoutedEventArgs e)
        {
            var selectedItem = lvDisplay.SelectedItem;
            User user = getUser();
            var x = LMS_PRN221Context.Ins.Users.Find(user.Uid);
            if (x != null)
            {
                x.FullName = user.FullName;
                x.Email = user.Email;
                x.RoleId = user.RoleId;
                x.Phone = user.Phone;
                x.Gender = user.Gender;
                LMS_PRN221Context.Ins.SaveChanges();
                LoadUser();
                var index = lvDisplay.Items.IndexOf(selectedItem);
                if (index >= 0)
                {
                    lvDisplay.SelectedIndex = index; // Đặt lại chỉ số đã chọn
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int uid;
            if (int.TryParse(txbID.Text, out uid))
            {
                User user = LMS_PRN221Context.Ins.Users.FirstOrDefault(x => x.Uid == uid);
                if (user != null)
                {
                    LMS_PRN221Context.Ins.Users.Remove(user);
                    LMS_PRN221Context.Ins.SaveChanges();
                }
                LoadUser();
            }
            else
            {
                MessageBox.Show("Invalid ID format.");
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            AddNewUser addNewUser = new AddNewUser();
            addNewUser.Show();
        }

        private void txtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                var users = LMS_PRN221Context.Ins.Users.Where(x => x.FullName.Contains(searchText) || x.Email.Contains(searchText))
                    .Select(x => new
                    {
                        Id = x.Uid,
                        FullName = x.FullName,
                        Phone = x.Phone,
                        Email = x.Email,
                        Role = x.Role.Name,
                        Gender = (bool)x.Gender ? "Nam" : "Nữ",
                    }).ToList();
                lvDisplay.ItemsSource = users;
            }
            else
            {
                LoadUser();
            }
        }
    }
}
