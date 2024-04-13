using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> GetByIdAsync(string id);
        Task AddAsync(ApplicationUser account);
        Task DeleteAsync(string id);
        Task UpdateAsync(ApplicationUser account);

        Task AddBlogAsync(ApplicationUser user, Blog blog);
        Task AddFriendShipAsync(ApplicationUser user, Friendship friendship);

        Task AddNofiticationAsync(ApplicationUser user, Nofitication nofitication);
    }
}
