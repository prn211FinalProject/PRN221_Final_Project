using System;
using System.Collections.Generic;

namespace LibraryCustomerSite.Models
{
    public partial class User
    {
        public User()
        {
            Blogs = new HashSet<Blog>();
            BorrowInformations = new HashSet<BorrowInformation>();
            Feedbacks = new HashSet<Feedback>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Uid { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? Status { get; set; }
        public bool? Gender { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<BorrowInformation> BorrowInformations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
