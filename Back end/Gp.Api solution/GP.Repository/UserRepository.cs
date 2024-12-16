using GP.Core.Repositories;
using GP.Repository.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.core.Entities.identity;

namespace GP.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppIdentityDbContext _context;

        public UserRepository(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(AppUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
