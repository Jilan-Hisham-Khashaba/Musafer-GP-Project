using GP.Core.Entities;
using GP.Core.Repositories;
using GP.core.Sepecifitction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specificatios
{
    public class TripCountSpecification : BaseSpecification<Trip>
    {
        private readonly IGenericRepositroy<Trip> tripRepo;


        public TripCountSpecification()
        {

        }
        public TripCountSpecification(IGenericRepositroy<Trip> tripRepo)

        {
            this.tripRepo = tripRepo;
        }

        public async Task<int> GetTripCountAsync(string userID)
        {
            var specification = new TripSpecifications(new TripwShSpecParams { /* البارامترات اللازمة لتحديد المواصفات */ });

            var Trips = await tripRepo.GetAllWithSpecAsyn(specification);

            var userTrips = Trips.Where(s => s.UserId == userID);

            return userTrips.Count();
        }

        public async Task<int> GetTripAllCountAsync()
        {
            var specification = new TripSpecifications(new TripwShSpecParams { /* البارامترات اللازمة لتحديد المواصفات */ });

            var Trips = await tripRepo.GetAllWithSpecAsyn(specification);

          

            return Trips.Count();
        }
    }

}
