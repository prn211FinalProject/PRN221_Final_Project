using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class Book
    {
        public Book()
        {
            Feedbacks = new HashSet<Feedback>();
            Oids = new HashSet<BorrowInformation>();
            Wids = new HashSet<Wishlist>();
        }

        public int Id { get; set; }
        public int? Cid { get; set; }
        public string? Bname { get; set; }
        public string? Author { get; set; }
        public int? Quantity { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public bool? Status { get; set; }

        public virtual Category? CidNavigation { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        public virtual ICollection<BorrowInformation> Oids { get; set; }
        public virtual ICollection<Wishlist> Wids { get; set; }
    }
}
