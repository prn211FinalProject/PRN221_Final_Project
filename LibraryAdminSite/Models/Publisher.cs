using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class Publisher
    {
        public Publisher()
        {
            BookTitles = new HashSet<BookTitle>();
        }

        public int Id { get; set; }
        public string? Pname { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<BookTitle> BookTitles { get; set; }
    }
}
