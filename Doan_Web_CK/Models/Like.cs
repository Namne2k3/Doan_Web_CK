namespace Doan_Web_CK.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public int BlogId { get; set; }
        public Blog? Blog { get; set; }
    }
}
