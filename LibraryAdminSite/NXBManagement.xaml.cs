using LibraryAdminSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace LibraryAdminSite
{
    public partial class NXBManagement : UserControl
    {
        private int? selectedNXBId = null;

        public NXBManagement()
        {
            InitializeComponent();
            LoadNXB();
            LoadStatus();
        }

        private void LoadStatus()
        {
            var statuses = new List<string> { "Hiển Thị", "Ẩn Đi" };
            cbxStatus.ItemsSource = statuses;
        }

        private void LoadNXB()
        {
            var NXB = LMS_PRN221Context.Ins.Publishers.Select(x => new
            {
                Id = x.Id,
                Name = x.Pname,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                Status = (x.Status ?? false) ? "Hiển Thị" : "Ẩn Đi",
            }).ToList();

            lvDisplay.ItemsSource = NXB;
        }

        private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedNXB = lvDisplay.SelectedItem as dynamic;

            if (selectedNXB != null)
            {
                selectedNXBId = selectedNXB.Id;
                txbNXBName.Text = selectedNXB.Name;
                txbAddress.Text = selectedNXB.Address;
                txbPhone.Text = selectedNXB.Phone;
                txbEmail.Text = selectedNXB.Email;
                cbxStatus.SelectedValue = selectedNXB.Status;
            }
        }

        private void AddNXB(object sender, RoutedEventArgs e)
        {
            string name = txbNXBName.Text.Trim();
            string address = txbAddress.Text.Trim();
            string phone = txbPhone.Text.Trim();
            string email = txbEmail.Text.Trim();
            bool status = cbxStatus.SelectedValue.ToString() == "Hiển Thị";

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Vui lòng nhập lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ! Vui lòng nhập lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool nameExists = LMS_PRN221Context.Ins.Publishers.Any(x => x.Pname.ToLower() == name.ToLower());
            bool phoneExists = LMS_PRN221Context.Ins.Publishers.Any(x => x.Phone == phone);
            bool emailExists = LMS_PRN221Context.Ins.Publishers.Any(x => x.Email.ToLower() == email.ToLower());

            if (nameExists || phoneExists || emailExists)
            {
                string message = "Nhà xuất bản đã tồn tại:";
                if (nameExists) message += $"\n- Tên: {name}";
                if (phoneExists) message += $"\n- Số điện thoại: {phone}";
                if (emailExists) message += $"\n- Địa chỉ email: {email}";

                MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            var newNXB = new Publisher
            {
                Pname = name,
                Address = address,
                Phone = phone,
                Email = email,
                Status = status
            };

            LMS_PRN221Context.Ins.Publishers.Add(newNXB);
            LMS_PRN221Context.Ins.SaveChanges();
                MessageBox.Show("Add Thành Công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

            LoadNXB();
            ClearFields();
        }

        private void UpdateNXB(object sender, RoutedEventArgs e)
        {
            if (selectedNXBId == null)
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = txbNXBName.Text.Trim();
            string address = txbAddress.Text.Trim();
            string phone = txbPhone.Text.Trim();
            string email = txbEmail.Text.Trim();
            bool status = cbxStatus.SelectedValue.ToString() == "Hiển Thị";

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Vui lòng nhập lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ! Vui lòng nhập lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nxbToUpdate = LMS_PRN221Context.Ins.Publishers.FirstOrDefault(x => x.Id == selectedNXBId);

            if (nxbToUpdate != null)
            {
                bool nameExists = LMS_PRN221Context.Ins.Publishers.Any(x => x.Pname.ToLower() == name.ToLower() && x.Id != selectedNXBId);
                bool phoneExists = LMS_PRN221Context.Ins.Publishers.Any(x => x.Phone == phone && x.Id != selectedNXBId);
                bool emailExists = LMS_PRN221Context.Ins.Publishers.Any(x => x.Email.ToLower() == email.ToLower() && x.Id != selectedNXBId);

                if (nameExists || phoneExists || emailExists)
                {
                    string message = "Nhà xuất bản đã tồn tại:";
                    if (nameExists) message += $"\n- Tên: {name}";
                    if (phoneExists) message += $"\n- Số điện thoại: {phone}";
                    if (emailExists) message += $"\n- Địa chỉ email: {email}";

                    MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; 
                }

                nxbToUpdate.Pname = name;
                nxbToUpdate.Address = address;
                nxbToUpdate.Phone = phone;
                nxbToUpdate.Email = email;
                nxbToUpdate.Status = status;

                LMS_PRN221Context.Ins.SaveChanges();
                MessageBox.Show("Update Thành Công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                LoadNXB();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhà xuất bản để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFields()
        {
            txbNXBName.Clear();
            txbAddress.Clear();
            txbPhone.Clear();
            txbEmail.Clear();
            cbxStatus.SelectedIndex = -1;
            selectedNXBId = null;
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\d{10,15}$");
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
