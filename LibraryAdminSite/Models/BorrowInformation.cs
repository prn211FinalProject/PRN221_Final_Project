using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class BorrowInformation
    {
        public BorrowInformation()
        {
            LibrarianOrders = new HashSet<LibrarianOrder>();
            Penalties = new HashSet<Penalty>();
            Bids = new HashSet<BookCopy>();
        }

        public int Oid { get; set; }
        public int? Uid { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? CheckoutDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Note { get; set; }
        public bool? Status { get; set; }

        public virtual User? UidNavigation { get; set; }
        public virtual ICollection<LibrarianOrder> LibrarianOrders { get; set; }
        public virtual ICollection<Penalty> Penalties { get; set; }

        public virtual ICollection<BookCopy> Bids { get; set; }
    }
}
