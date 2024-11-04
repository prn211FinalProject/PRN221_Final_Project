using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class BookTitle
    {
        public BookTitle()
        {
            BookCopies = new HashSet<BookCopy>();
            Feedbacks = new HashSet<Feedback>();
        }

        public int Id { get; set; }
        public int? Cid { get; set; }
        public string? Bname { get; set; }
        public string? Author { get; set; }
        public int? Quantity { get; set; }
        public int? UnitInStock { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public bool Hide { get; set; }
        public int? PublisherId { get; set; }
        public DateTime? PublishDate { get; set; }

        public virtual Category? CidNavigation { get; set; }
        public virtual Publisher? Publisher { get; set; }
        public virtual ICollection<BookCopy> BookCopies { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
