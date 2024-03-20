namespace Doan_Web_CK.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Response { get; set; }
        public int Rate { get; set; }

        public string? AccountId { get; set; }
        public ApplicationUser? Account { get; set; }
    }
}
