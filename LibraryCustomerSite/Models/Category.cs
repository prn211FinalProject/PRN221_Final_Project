using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class Category
    {
        public Category()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string? Cname { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
