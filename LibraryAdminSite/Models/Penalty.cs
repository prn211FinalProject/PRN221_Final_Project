using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class Penalty
    {
        public int Id { get; set; }
        public int? Oid { get; set; }
        public int? Uid { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }

        public virtual BorrowInformation? OidNavigation { get; set; }
        public virtual User? UidNavigation { get; set; }
    }
}
