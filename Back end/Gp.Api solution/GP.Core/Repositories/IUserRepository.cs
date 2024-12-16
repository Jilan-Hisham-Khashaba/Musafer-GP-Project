using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.core.Entities.identity;

namespace GP.Core.Repositories
{
     public  interface IUserRepository
    {
        Task<AppUser> GetByIdAsync(string id);
        Task<AppUser> GetByEmailAsync(string email);
        Task AddAsync(AppUser user);
        Task UpdateAsync(AppUser user);
        Task DeleteAsync(AppUser user);
    }
}
