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
            totalNotAvailableCount = LMS_PRN221Context.Ins.BookCopies
                .Include(x => x.BookTitle)
                .Where(x => x.BookTitle.Hide == false)
                .Count(bc => bc.Status == false);

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var bookThongKe = LMS_PRN221Context.Ins.BookTitles
                .Include(x => x.CidNavigation)
                .Include(x => x.BookCopies)
                .Where(x => x.Hide == false && x.CidNavigation.Status == true)
                .Where(x => x.BookCopies
                    .Any(bc => bc.Oids
                        .Any(borrowInfo => borrowInfo.BorrowDate.Value.Month == currentMonth
                                        && borrowInfo.BorrowDate.Value.Year == currentYear))
                )
                .Select(x => new
                {
                    Id = x.CidNavigation.Id,
                    CName = x.CidNavigation.Cname,
                    NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false)
                })
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
            int currentMonth = DateTime.Now.Month;  

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
                    BorrowedInCurrentMonth = x.BookCopies
                        .Any(bc => bc.Oids
                            .Any(bi => bi.BorrowDate.HasValue && bi.BorrowDate.Value.Month == currentMonth))
                })
                .Where(x => x.BorrowedInCurrentMonth)  
                .ToList();

            lvDisplay2.ItemsSource = cateThongKe;
        }

        private void FilterThongKe()
        {
            var cateThongKe = LMS_PRN221Context.Ins.BookTitles
         .Include(x => x.CidNavigation)
         .Include(x => x.BookCopies)
         .ThenInclude(x => x.Oids)
         .Where(x => x.Hide == false && x.CidNavigation.Status == true)
         .AsQueryable();

            if (dpFromDate2.SelectedDate.HasValue)
            {
                DateTime fromDate = dpFromDate2.SelectedDate.Value;
                cateThongKe = cateThongKe.Where(x => x.BookCopies
                    .Any(bc => bc.Oids
                        .Any(bi => bi.BorrowDate.HasValue && bi.BorrowDate.Value >= fromDate)));
            }

            if (dpToDate2.SelectedDate.HasValue)
            {
                DateTime toDate = dpToDate2.SelectedDate.Value;
                cateThongKe = cateThongKe.Where(x => x.BookCopies
                    .Any(bc => bc.Oids
                        .Any(bi => bi.BorrowDate.HasValue && bi.BorrowDate.Value <= toDate)));
            }

            if (PieSeriesCollection != null)
            {
                PieSeriesCollection.Clear();
            }

            var filteredBooks = cateThongKe
                .Select(x => new
                {
                    Id = x.CidNavigation.Id,
                    Title = x.Bname,
                    NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false),
                    BorrowedCount = x.BookCopies.Count(bc => bc.Oids
                        .Any(bi => bi.BorrowDate.HasValue &&
                                    bi.BorrowDate.Value.Month == DateTime.Now.Month)) 
                })
                .ToList();

            lvDisplay2.ItemsSource = filteredBooks;

        }


        private void FilterBook()
        {

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var cateThongKe = LMS_PRN221Context.Ins.BookTitles
                .Include(x => x.CidNavigation)
                .Include(x => x.BookCopies)
                .ThenInclude(x => x.Oids)
                .Where(x => x.Hide == false && x.CidNavigation.Status == true)
                .AsQueryable();

            if (dpFromDate2.SelectedDate.HasValue)
            {
                DateTime fromDate = dpFromDate2.SelectedDate.Value;
                cateThongKe = cateThongKe.Where(x => x.BookCopies
                    .Any(bc => bc.Oids
                        .Any(bi => bi.BorrowDate.HasValue && bi.BorrowDate.Value >= fromDate)));
            }

            if (dpToDate2.SelectedDate.HasValue)
            {
                DateTime toDate = dpToDate2.SelectedDate.Value;
                cateThongKe = cateThongKe.Where(x => x.BookCopies
                    .Any(bc => bc.Oids
                        .Any(bi => bi.BorrowDate.HasValue && bi.BorrowDate.Value <= toDate)));
            }

            cateThongKe = cateThongKe.Where(x => x.BookCopies
                .Any(bc => bc.Oids
                    .Any(bi => bi.BorrowDate.HasValue &&
                               bi.BorrowDate.Value.Month == currentMonth &&
                               bi.BorrowDate.Value.Year == currentYear)));

            var filteredBooks = cateThongKe
                .Select(x => new
                {
                    Id = x.CidNavigation.Id,
                    Title = x.Bname,
                    NotAvailableCount = x.BookCopies.Count(bc => bc.Status == false),
                    BorrowedCount = x.BookCopies.Count(bc => bc.Oids
                        .Any(bi => bi.BorrowDate.HasValue &&
                                    bi.BorrowDate.Value.Month == currentMonth)) 
                })
                .ToList();

            lvDisplay2.ItemsSource = filteredBooks;
        }


        private void dpToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterThongKe();
        }

        private void dpToDate_SelectedDateChanged2(object sender, SelectionChangedEventArgs e)
        {
            FilterBook();
        }
    }
}
