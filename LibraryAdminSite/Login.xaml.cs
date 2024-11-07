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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtEmail != null && txtPass != null)
            {
                string email = txtEmail.Text;
                string pass = txtPass.Password;
                var user = LMS_PRN221Context.Ins.Users.FirstOrDefault(x => x.Email.Equals(email));

                if (user != null)
                {
                    if (user.RoleId == 1) // Kiểm tra quyền admin
                    {
                        if (user.Password == pass) // Kiểm tra mật khẩu
                        {
                            // Lưu thông tin người dùng vào Application.Current.Properties
                            Application.Current.Properties["LibrarianId"] = user.Uid;
                            Application.Current.Properties["FullName"] = user.FullName;
                            Application.Current.Properties["Email"] = user.Email;

                            // Mở cửa sổ chính sau khi đăng nhập thành công
                            LoadAdmin mainWindow = new LoadAdmin();
                            mainWindow.Show();
                            this.Close(); // Đóng cửa sổ đăng nhập
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Email hoặc mật khẩu không chính xác. Vui lòng thử lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Email hoặc mật khẩu không chính xác. Vui lòng thử lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

    }
}
