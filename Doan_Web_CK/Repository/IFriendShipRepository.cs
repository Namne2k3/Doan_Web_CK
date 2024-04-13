using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface IFriendShipRepository
    {
        Task<IEnumerable<Friendship>> GetAllAsync();
        Task UpdateAsync(Friendship friendship);
        Task DeleteAsync(int id);
        Task<Friendship> GetByIdAsync(int id);
        Task RemoveFriendsByUserId(string id);
    }
}
