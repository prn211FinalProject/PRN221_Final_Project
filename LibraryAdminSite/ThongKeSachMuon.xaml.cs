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
        public SeriesCollection PieSeriesCollection { get; set; }

        public ThongKeSachMuon()
        {
            InitializeComponent();
            PieSeriesCollection = new SeriesCollection();
            LoadCateThongKe();
            DataContext = this;
            LoadTopBook();
        }

        private void LoadCateThongKe()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            totalNotAvailableCount = LMS_PRN221Context.Ins.BookCopies
                .Include(x => x.BookTitle)
                .Where(x => x.BookTitle.Hide == false)
                .Count(bc => bc.Status == false);

            var bookThongKe = LMS_PRN221Context.Ins.BookTitles
                .Include(x => x.CidNavigation)
                .Include(x => x.BookCopies)
                .Where(x => x.Hide == false && x.CidNavigation.Status == true)
                .Where(x => x.BookCopies
                    .Any(bc => bc.Oids
                        .Any(borrowInfo => borrowInfo.BorrowDate != null))
                )
                .Select(x => new
                {
                    Id = x.CidNavigation.Id,
                    CName = x.CidNavigation.Cname,
                    NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false)
                })
                .Where(x => x.NotAvailableCount > 0)
                .ToList();

            var cateThongKe = bookThongKe
                .GroupBy(x => new { x.Id, x.CName })
                .Select(g => new
                {
                    Id = g.Key.Id,
                    CName = g.Key.CName,
                    NotAvailableCount = g.Sum(x => x.NotAvailableCount),
                    Rate = totalNotAvailableCount > 0
                        ? Math.Round(((double)g.Sum(x => x.NotAvailableCount) / totalNotAvailableCount) * 100, 2)
                        : 0
                })
                // Sắp xếp theo tỷ lệ giảm dần
                .OrderByDescending(x => x.Rate)
                .ToList();

            lvDisplay.ItemsSource = cateThongKe;

            if (PieSeriesCollection != null)
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
                    HasBeenBorrowed = x.BookCopies
                        .Any(bc => bc.Oids.Any(bi => bi.BorrowDate != null))
                })
                .Where(x => x.HasBeenBorrowed)
                  .Where(x => x.NotAvailableCount > 0)
                .OrderByDescending(x => x.NotAvailableCount)
                .ToList();

            lvDisplay2.ItemsSource = cateThongKe;
        }


    }
}