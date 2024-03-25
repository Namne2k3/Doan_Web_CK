using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllAsync();
        Task AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);
        Task DeleteAsync(int id);
        Task UpdateCommentsAsync();
        Task AddCommentAsync(Blog blog, Comment comment);
        Task DeleteCommentsAsync();
        Task AddLikeAsync(Blog blog, Like like);
        Task DeleteLikeAsync(Blog blog, Like like);
        Task<Blog> GetByIdAsync(int? id);
    }
}
