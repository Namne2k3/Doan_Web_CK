using System.ComponentModel.DataAnnotations;

namespace Doan_Web_CK.Models
{
    public class Nofitication
    {
        public int Id { get; set; }

        public int? BlogId { get; set; }
        public Blog? Blog { get; set; } // Đổi tên thuộc tính 'blog' thành 'Blog'

        public string RecieveAccountId { get; set; }
        public ApplicationUser? RecieveAccount { get; set; }
        public string SenderAccountId { get; set; }
        public ApplicationUser? SenderAccount { get; set; }

        [Required]
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }
}
