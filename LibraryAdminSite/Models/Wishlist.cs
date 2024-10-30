using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class Wishlist
    {
        public Wishlist()
        {
            Bids = new HashSet<Book>();
        }

        public int Id { get; set; }
        public int? Uid { get; set; }

        public virtual User? UidNavigation { get; set; }

        public virtual ICollection<Book> Bids { get; set; }
    }
}
