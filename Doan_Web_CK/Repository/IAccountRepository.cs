﻿using Doan_Web_CK.Models;

namespace Doan_Web_CK.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> GetByIdAsync(string id);
        Task AddAsync(ApplicationUser account);
        Task DeleteAsync(int id);
        Task UpdateAsync(ApplicationUser account);
    }
}