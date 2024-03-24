using System.ComponentModel.DataAnnotations;

namespace Doan_Web_CK.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public int? ReferenceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Required]
        public string Content { get; set; }
        public string? AccountId { get; set; } // Khóa ngoại tham chiếu đến Id của Account
        public ApplicationUser? Account { get; set; }

        public string BlogImageUrl { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime PublishDate { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Like>? Likes { get; set; }
        public IEnumerable<Comment>? Comments { get; set; }
    }
}
