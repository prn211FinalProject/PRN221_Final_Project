using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class Category
    {
        public Category()
        {
            BookTitles = new HashSet<BookTitle>();
        }

        public int Id { get; set; }
        public string? Cname { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<BookTitle> BookTitles { get; set; }
    }
}
