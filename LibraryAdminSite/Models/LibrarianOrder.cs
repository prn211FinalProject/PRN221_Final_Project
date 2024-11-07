using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class LibrarianOrder
    {
        public int Id { get; set; }
        public int? LibrarianId { get; set; }
        public int? BorrowOrderId { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public bool? Status { get; set; }

        public virtual BorrowInformation? BorrowOrder { get; set; }
        public virtual User? Librarian { get; set; }
    }
}
