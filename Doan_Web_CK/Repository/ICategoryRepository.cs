using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task DeleteAsync(int id);
        Task UpdateAsync(Category category);

    }
}
