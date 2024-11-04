using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Phone]
        public string? Phone { get; set; }
        public bool? Status { get; set; }
        public int? RoleId { get; set; }
        public string? Password { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<BorrowInformation> BorrowInformations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
