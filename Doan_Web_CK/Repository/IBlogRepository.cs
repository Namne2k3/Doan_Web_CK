using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllAsync();
        Task AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);
        Task DeleteAsync(int id);
        Task AddCommentsAsync(Comment comment);
        Task UpdateCommentsAsync();

        Task DeleteCommentsAsync();
        Task AddLikeAsync();
        Task DeleteLikeAsync();
        Task<Blog> GetByIdAsync(int? id);
    }
}
