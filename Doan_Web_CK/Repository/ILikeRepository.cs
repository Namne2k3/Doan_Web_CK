using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface ILikeRepository
    {
        Task<IEnumerable<Like>> GetAllLikeAsync();
        Task<Like> GetLikeByIdAsync(int id);
        Task DeleteAsync(Like like);
        Task RemoveLikesByUserId(string id);
    }
}
