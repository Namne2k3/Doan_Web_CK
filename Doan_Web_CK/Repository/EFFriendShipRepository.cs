﻿using Doan_Web_CK.Models;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Repository
{
    public class EFFriendShipRepository : IFriendShipRepository
    {
        private readonly ApplicationDbContext _context;
        public EFFriendShipRepository(ApplicationDbContext context) { _context = context; }


        public async Task DeleteAsync(int id)
        {
            var friendship = await _context.Friendships.SingleOrDefaultAsync(p => p.Id == id);
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Friendship>> GetAllAsync()
        {
            return await _context.Friendships
                .Include(p => p.User)
                .Include(p => p.Friend)
                .ToListAsync();
        }

        public async Task<Friendship> GetByIdAsync(int id)
        {
            return await _context.Friendships
                                        .Include(p => p.User)
                                        .Include(p => p.Friend)
                                        .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task RemoveFriendsByUserId(string id)
        {
            var friends = await _context.Friendships.Where(p => p.UserId == id || p.FriendId == id).ToListAsync();
            if (friends != null)
            {
                foreach (var item in friends)
                {
                    _context.Friendships.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateAsync(Friendship friendship)
        {
            _context.Friendships.Update(friendship);
            await _context.SaveChangesAsync();
        }
    }
}
