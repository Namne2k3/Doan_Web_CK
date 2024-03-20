using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllComments();
        Task<Comment> GetByIdAsync(int id);
        Task AddAsync(Comment comment);
        Task DeleteAsync(int id);
        Task UpdateAsync(Comment comment);
    }
}
