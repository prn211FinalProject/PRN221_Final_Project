using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class Wishlist
    {
        public Wishlist()
        {
            Bids = new HashSet<BookCopy>();
        }

        public int Id { get; set; }
        public int? Uid { get; set; }

        public virtual User? UidNavigation { get; set; }

        public virtual ICollection<BookCopy> Bids { get; set; }
    }
}
