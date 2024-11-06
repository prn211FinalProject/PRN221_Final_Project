using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class BorrowInformation
    {
        public BorrowInformation()
        {
            Bids = new HashSet<BookCopy>();
        }

        public int Oid { get; set; }
        public int? Uid { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? CheckoutDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }

        public virtual User? UidNavigation { get; set; }

        public virtual ICollection<BookCopy> Bids { get; set; }
    }
}
