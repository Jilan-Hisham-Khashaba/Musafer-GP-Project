using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repositories
{
    public interface IRequestRepository
    {
        Task<Request> GetRequestAsync(int RequestId);

        Task UpdateRequestAsync2(Request request);

        void Detach(Request request);
        Task<List<Core.Entities.Request>> GetAllRequestsAsyncCount();

        Task<Request> GetRequestAsyncForAdmin(int RequestId);

        Task<Request> UpdateRequestAsync(Request request);

        Task<bool> DeleteRequestAsync(int RequestId);

        Task<List<Request>> GetAllRequestsAsync();
        Task<int> SaveChangesAsync();
    }
}
