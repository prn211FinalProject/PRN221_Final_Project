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
    /// Interaction logic for LoadAdmin.xaml
    /// </summary>
    public partial class LoadAdmin : Window
    {
        public LoadAdmin()
        {
            InitializeComponent();
        }

        private void btnBookManage_Click(object sender, RoutedEventArgs e)
        {
            var librarian = new quanLyLibrarian();

            // Load the UserManage control into the ContentControl
           AdminContentControl.Content = librarian;
        }
    }
}
