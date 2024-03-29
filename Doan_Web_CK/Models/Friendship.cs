namespace Doan_Web_CK.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public bool IsConfirmed { get; set; } // Đánh dấu xem mối quan hệ đã được xác nhận hay chưa

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public string FriendId { get; set; }
        public ApplicationUser? Friend { get; set; }
    }
}
