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
        public async Task<IEnumerable<Like>> GetAllLikeAsync()
        {
            return await _context.Likes.ToListAsync();
        }

        public async Task<Like> GetLikeByIdAsync(int id)
        {
            return await _context.Likes.FindAsync(id);
        }
    }
}
