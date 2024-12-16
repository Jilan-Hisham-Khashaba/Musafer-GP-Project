using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.core.Entities.identity;

namespace GP.core.Services
{
    public interface ITokenService
    {
            Task<string> CreateTokenAsyn(AppUser user,UserManager<AppUser> userManager);
           
    }
}
