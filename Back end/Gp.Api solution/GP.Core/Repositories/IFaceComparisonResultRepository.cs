using Emgu.CV.Ocl;
using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repositories
{

    public interface IFaceComparisonResultRepository
    {
        Task SaveComparisonResult(FaceComparison result);
        Task<bool> CheckUserExistsInVerificationFaces(string userId);
        Task<FaceComparison> GetFaceComparisonResultByUserId(string userId); // Add this method

    }
}
