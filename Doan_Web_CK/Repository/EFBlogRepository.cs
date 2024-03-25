using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFBlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;
        public EFBlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
        }
        public async Task AddLikeAsync(Blog blog, Like like)
        {
            blog.Likes.Add(like);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blog = await _context.Blogs.SingleOrDefaultAsync(p => p.Id == id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Blogs.ToListAsync();
        }
        public Task DeleteCommentsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteLikeAsync(Blog blog, Like like)
        {
            blog.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public Task UpdateCommentsAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<Blog> GetByIdAsync(int? blogId)
        {
            return await _context.Blogs.FindAsync(blogId);
        }

        public async Task AddCommentAsync(Blog blog, Comment comment)
        {
            blog.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
