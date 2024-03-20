using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public class EFNofiticationRepository : INotifiticationRepository
    {
        private readonly ApplicationDbContext _context;

        public EFNofiticationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Nofitication nofitication)
        {
            await _context.Nofitications.AddAsync(nofitication);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Nofitication>> GetAllNotifitions()
        {
            throw new NotImplementedException();
        }

        public Task<Nofitication> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Nofitication nofitication)
        {
            throw new NotImplementedException();
        }
    }
}
