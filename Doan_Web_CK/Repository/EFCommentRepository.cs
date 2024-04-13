using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFCommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetAllComments()
        {
            return await _context.Comments
                .Include(p => p.Account)
                .Include(p => p.Blog)
                .ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(p => p.Account)
                .Include(p => p.Blog)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task RemoveCommentsByUserId(string id)
        {
            var comments = await _context.Comments.Where(p => p.AccountId == id).ToListAsync();
            if (comments != null)
            {
                foreach (var item in comments)
                {
                    _context.Comments.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}
