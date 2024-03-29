using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFLikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;
        public EFLikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(Like like)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Like>> GetAllLikeAsync()
        {
            return await _context.Likes
                .Include(p => p.ApplicationUser)
                .Include(p => p.Blog)
                .ToListAsync();
        }

        public async Task<Like> GetLikeByIdAsync(int id)
        {
            return await _context.Likes.FindAsync(id);
        }
    }
}
