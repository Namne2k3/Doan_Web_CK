using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFFriendShipRepository : IFriendShipRepository
    {
        private readonly ApplicationDbContext _context;
        public EFFriendShipRepository(ApplicationDbContext context) { _context = context; }

        public async Task DeleteAsync(Friendship friendship)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Friendship>> GetAllAsync()
        {
            return await _context.Friendships
                .Include(p => p.User)
                .Include(p => p.Friend)
                .ToListAsync();
        }

        public async Task UpdateAsync(Friendship friendship)
        {
            _context.Friendships.Update(friendship);
            await _context.SaveChangesAsync();
        }
    }
}
