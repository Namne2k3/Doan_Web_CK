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
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllComments()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}
