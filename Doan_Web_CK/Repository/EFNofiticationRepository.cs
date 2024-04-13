using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteAsync(int id)
        {
            var nof = await _context.Nofitications
                .Include(p => p.Blog)
                .Include(p => p.SenderAccount)
                .Include(p => p.RecieveAccount)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (nof != null)
            {
                _context.Nofitications.Remove(nof);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Nofitication>> GetAllNotifitions()
        {
            return await _context.Nofitications
                .Include(p => p.Blog)
                .Include(p => p.SenderAccount)
                .Include(p => p.RecieveAccount)
                .ToListAsync();
        }

        public async Task<Nofitication> GetByIdAsync(int id)
        {
            return await _context.Nofitications
                .Include(p => p.Blog)
                .Include(p => p.SenderAccount)
                .Include(p => p.RecieveAccount)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task RemoveNofsByUserId(string id)
        {
            var nofs = await _context.Nofitications.Where(p => p.SenderAccountId == id || p.RecieveAccountId == id).ToListAsync();
            if (nofs != null)
            {
                foreach (var item in nofs)
                {
                    _context.Nofitications.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public Task UpdateAsync(Nofitication nofitication)
        {
            throw new NotImplementedException();
        }
    }
}
