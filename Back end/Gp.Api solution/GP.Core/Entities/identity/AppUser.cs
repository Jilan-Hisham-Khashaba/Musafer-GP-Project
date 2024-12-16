using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GP.core.Entities.identity
{
    public class AppUser : IdentityUser
    {


        public string DisplayName { get; set; }

        public string? PhotoPicture { get; set; }

        public DateTime? LastLogin { get; set; }

        public Address Address { get; set; }//Nevagtianla prop=>one
    }

}
