using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class BorrowInformation
    {
        public BorrowInformation()
        {
            Bids = new HashSet<Book>();
        }

        public int Oid { get; set; }
        public int? Uid { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Note { get; set; }
        public bool? Status { get; set; }

        public virtual User? UidNavigation { get; set; }

        public virtual ICollection<Book> Bids { get; set; }
    }
}
