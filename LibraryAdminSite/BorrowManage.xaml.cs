using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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
	/// Interaction logic for BorrowManage.xaml
	/// </summary>
	public partial class BorrowManage : UserControl
	{
		public BorrowManage()
		{
			InitializeComponent();
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			LoadBorrowInfor();
		}
		private void LoadBorrowInfor()
		{
			var borrow = LMS_PRN221Context.Ins.BorrowInformations.Include(x => x.UidNavigation).Select(x => new
			{
				Id = x.Oid,
				Uid = x.Uid,
				FullName = x.UidNavigation.FullName,
				Email = x.UidNavigation.Email,
				CheckoutDate = x.CheckoutDate,
				BorrowDate = x.BorrowDate,
				DueDate = x.DueDate,
				Status = x.Status == true ? "Đã mượn" : "Chưa mượn",
				Phone = x.UidNavigation.Phone,
				bookCopy = x.Bids
			}).ToList();
			lvDisplay.ItemsSource = borrow;
		}
		private ObservableCollection<object> bookDisplayList = new ObservableCollection<object>();
		private void lvDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			bookDisplayList.Clear();
			var selectedItem = lvDisplay.SelectedItem;
			if (selectedItem != null)
			{
				var bookCopies = (ICollection<BookCopy>)selectedItem.GetType().GetProperty("bookCopy").GetValue(selectedItem);
				List<string> bids = bookCopies.Select(bookCopy => bookCopy.Id).ToList();
				foreach (var bid in bids)
				{
					LoadBook(bid);
				}
			}
		}

		private void LoadBook(string Bid)
		{
			var book = LMS_PRN221Context.Ins.BookCopies.Include(x => x.BookTitle).Where(x => x.Id == Bid)
				.Select(x => new
				{
					Bid = x.Id,
					Name = x.BookTitle.Bname,
					Status = x.Status == true ? "Đang sử dụng" : "Có thể sử dụng",
					Note = x.Note
				}).ToList();

			// Nếu ItemsSource chưa được khởi tạo, gán nó
			if (lvDisplayBook.ItemsSource == null)
			{
				lvDisplayBook.ItemsSource = bookDisplayList;
			}

			// Thêm các mục mới vào ObservableCollection
			foreach (var item in book)
			{
				bookDisplayList.Add(item);
			}
		}

		private void lvDisplayBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			stackStatus.Visibility = Visibility.Visible;
			stackBtn.Visibility = Visibility.Visible;
			var bookCopy = lvDisplayBook.SelectedItem;
			if (bookCopy != null)
			{
				string status = (string)bookCopy.GetType().GetProperty("Status").GetValue(bookCopy);

				rdbInUse.IsChecked = status.Equals("Đang sử dụng") ? true : false;
				rdbCanUse.IsChecked = status.Equals("Có thể sử dụng") ? true : false;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{

				var selectedItem = lvDisplayBook.SelectedItem;
				if (selectedItem != null)
				{
					string Bid = (string)selectedItem.GetType().GetProperty("Bid").GetValue(selectedItem);
					var bookCopy = LMS_PRN221Context.Ins.BookCopies.FirstOrDefault(x => x.Id == Bid);
					if (bookCopy != null)
					{
						bookCopy.Status = rdbInUse.IsChecked;
						LMS_PRN221Context.Ins.SaveChanges();
						bookDisplayList.Clear();
						var selectedItem2 = lvDisplay.SelectedItem;
						if (selectedItem2 != null)
						{
							var bookCopies = (ICollection<BookCopy>)selectedItem2.GetType().GetProperty("bookCopy").GetValue(selectedItem2);
							List<string> bids = bookCopies.Select(bookCopy => bookCopy.Id).ToList();
							foreach (var bid in bids)
							{
								LoadBook(bid);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
