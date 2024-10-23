using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class Blog
    {
        public int Bid { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }
        public int? Uid { get; set; }

        public virtual User? UidNavigation { get; set; }
    }
}
