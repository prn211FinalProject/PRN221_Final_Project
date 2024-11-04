using LiveCharts;
using LiveCharts.Wpf;
using LibraryAdminSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibraryAdminSite
{
    public partial class ThongKeSachMuon : UserControl
    {
        private int totalNotAvailableCount;
        public SeriesCollection PieSeriesCollection { get; set; } // Khai báo

        public ThongKeSachMuon()
        {
            InitializeComponent();
            PieSeriesCollection = new SeriesCollection(); // Khởi tạo
            LoadCateThongKe();
            DataContext = this;
            LoadTopBook();
        }

        private void LoadCateThongKe()
        {
            totalNotAvailableCount = LMS_PRN221Context.Ins.BookCopies
                .Include(x => x.BookTitle).Where(x => x.BookTitle.Hide == false)
                .Count(bc => bc.Status == false);

            var cateThongKe = LMS_PRN221Context.Ins.BookTitles
                .Include(x => x.CidNavigation)
                .Include(x => x.BookCopies)
                .ThenInclude(x => x.Oids)
                .Where(x => x.Hide == false && x.CidNavigation.Status == true)
                .Select(x => new
                {
                    Id = x.CidNavigation.Id,
                    CName = x.CidNavigation.Cname,
                    NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false),
                    Rate = totalNotAvailableCount > 0
                        ? Math.Round(((double)x.BookCopies.Count(bc => bc.Status == false) / totalNotAvailableCount) * 100, 2)
                        : 0
                })
                .ToList();

            lvDisplay.ItemsSource = cateThongKe;

            // Cập nhật PieSeriesCollection cho biểu đồ
            if (PieSeriesCollection != null) // Kiểm tra nếu khác null
            {
                PieSeriesCollection.Clear();
            }
            foreach (var item in cateThongKe)
            {
                PieSeriesCollection.Add(new PieSeries
                {
                    Title = item.CName,
                    Values = new ChartValues<double> { item.Rate },
                    DataLabels = true
                });
            }
        }

        private void LoadTopBook()
        {

            var cateThongKe = LMS_PRN221Context.Ins.BookTitles
                .Include(x => x.CidNavigation)
                .Include(x => x.BookCopies)
                .ThenInclude(x => x.Oids)
                .Where(x => x.Hide == false && x.CidNavigation.Status == true)
                .Select(x => new
                {
                    Id = x.CidNavigation.Id,
                    Title = x.Bname,
                    NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false),
                   
                })
                .ToList();

            lvDisplay2.ItemsSource = cateThongKe;

        }

        private void FilterThongKe()
        {
            var cateThongKe = LMS_PRN221Context.Ins.BookTitles
                .Include(x => x.CidNavigation)
                .Include(x => x.BookCopies)
                .ThenInclude(x => x.Oids)
                .Where(x => x.Hide == false && x.CidNavigation.Status == true).AsQueryable();

            if (dpFromDate.SelectedDate.HasValue)
            {
                DateTime fromDate = dpFromDate.SelectedDate.Value;
                cateThongKe = cateThongKe.Where(x => x.PublishDate >= fromDate);
            }
            if (dpToDate.SelectedDate.HasValue)
            {
                DateTime toDate = dpToDate.SelectedDate.Value;
                cateThongKe = cateThongKe.Where(x => x.PublishDate <= toDate);
            }

            var filteredStudents = cateThongKe.Select(x => new
            {
                Id = x.CidNavigation.Id,
                CName = x.CidNavigation.Cname,
                NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false),
                Rate = totalNotAvailableCount > 0
                    ? Math.Round(((double)x.BookCopies.Count(bc => bc.Status == false) / totalNotAvailableCount) * 100, 2)
                    : 0
            }).ToList();

            lvDisplay.ItemsSource = filteredStudents;

            // Cập nhật PieSeriesCollection với dữ liệu đã lọc
            if (PieSeriesCollection != null) // Kiểm tra nếu khác null
            {
                PieSeriesCollection.Clear();
            }
            foreach (var item in filteredStudents)
            {
                PieSeriesCollection.Add(new PieSeries
                {
                    Title = item.CName,
                    Values = new ChartValues<double> { item.Rate },
                    DataLabels = true
                });
            }
        }

        private void dpToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterThongKe();
        }
    }
}
