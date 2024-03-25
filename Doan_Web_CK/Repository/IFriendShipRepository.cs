using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface IFriendShipRepository
    {
        Task<IEnumerable<Friendship>> GetAllAsync();
    }
}
