using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFAccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public EFAccountRepository(ApplicationDbContext context) { _context = context; }
        public async Task AddAsync(ApplicationUser account)
        {
            _context.ApplicationUsers.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task AddBlogAsync(ApplicationUser user, Blog blog)
        {
            user?.Blogs?.Add(blog);
            await _context.SaveChangesAsync();
        }

        public async Task AddFriendShipAsync(ApplicationUser user, Friendship friendship)
        {
            if (user.Friendships == null)
            {
                user.Friendships = new List<Friendship>();
            }
            user.Friendships.Add(friendship);

            await _context.SaveChangesAsync();
        }

        public async Task AddNofiticationAsync(ApplicationUser user, Nofitication nofitication)
        {
            if (user.Nofitications == null)
            {
                user.Nofitications = new List<Nofitication>();
            }
            user.Nofitications.Add(nofitication);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _context.ApplicationUsers.FindAsync(id);
            _context.ApplicationUsers.Remove(account);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _context.ApplicationUsers.FindAsync(id);
        }

        public async Task UpdateAsync(ApplicationUser account)
        {
            _context.ApplicationUsers.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
