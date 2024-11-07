using System;
using System.Collections.Generic;

namespace LibraryAdminSite.Models
{
    public partial class PasswordResetToken
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }

        public virtual User? User { get; set; }
    }
}
