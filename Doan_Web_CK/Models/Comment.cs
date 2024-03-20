using System.ComponentModel.DataAnnotations;

namespace Doan_Web_CK.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public string? AccountId { get; set; }
        public ApplicationUser? Account { get; set; }

        public int? BlogId { get; set; }
        public Blog? Blog { get; set; }

        public DateTime CommentDate { get; set; }
    }
}
