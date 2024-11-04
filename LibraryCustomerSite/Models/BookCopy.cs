using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class BookCopy
    {
        public BookCopy()
        {
            Oids = new HashSet<BorrowInformation>();
            Wids = new HashSet<Wishlist>();
        }

        public string Id { get; set; } = null!;
        public int? BookTitleId { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }

        public virtual BookTitle? BookTitle { get; set; }

        public virtual ICollection<BorrowInformation> Oids { get; set; }
        public virtual ICollection<Wishlist> Wids { get; set; }
    }
}
