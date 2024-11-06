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
                    if (user.RoleId == 1)
                    {
                        if (user.Password == pass)
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                            return;
                        }
                    }
                }

                    MessageBox.Show("Email hoặc mật khẩu không chính xác. Vui lòng thử lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
