using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface INotifiticationRepository
    {
        Task<IEnumerable<Nofitication>> GetAllNotifitions();
        Task<Nofitication> GetByIdAsync(int id);
        Task AddAsync(Nofitication nofitication);
        Task DeleteAsync(int id);
        Task UpdateAsync(Nofitication nofitication);
        Task RemoveNofsByUserId(string id);
    }
}
