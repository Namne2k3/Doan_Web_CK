using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Doan_Web_CK.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? ImageUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public List<Blog>? Blogs { get; set; }
        public List<Nofitication>? Nofitications { get; set; }
        public List<Friendship>? Friendships { get; set; }

    }
}
