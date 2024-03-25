using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFFriendShipRepository : IFriendShipRepository
    {
        private readonly ApplicationDbContext _context;
        public EFFriendShipRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<Friendship>> GetAllAsync()
        {
            return await _context.Friendships.ToListAsync();
        }
    }
}
