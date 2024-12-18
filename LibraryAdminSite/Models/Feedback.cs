﻿using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class Feedback
    {
        public int Id { get; set; }
        public int? Bid { get; set; }
        public int? Uid { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }

        public virtual Book? BidNavigation { get; set; }
        public virtual User? UidNavigation { get; set; }
    }
}
