using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GP.core.Sepecifitction;

namespace GP.Core.Specificatios
{
    public class TripSpecifications :BaseSpecification<Trip>
    {
        public TripSpecifications(TripwShSpecParams tripwShSpec) //get
        {
            includes.Add(T => T.FromCity);
            includes.Add(T => T.ToCity);
            includes.Add(T => T.ToCity.Country);
            includes.Add(T => T.FromCity.Country);
            //includes.Add(sh => sh.User);
            if (!string.IsNullOrEmpty(tripwShSpec.Sort))
            {
                switch (tripwShSpec.Sort)
                {
                    case "KgAsc":
                        AddOrderBy(T => T.availableKg);
                        break;
                    case "KgDesc":
                        AddOrderByDescending(T => T.availableKg);
                        break;

                    default:
                        AddOrderBy(T => T.ToCity.Country.NameCountry);
                        break;
                }
            }
            //totalTrips=50;
            //pageSize=10;
            //pageIndex=3;
            ApplyPagination(tripwShSpec.PageSize*(tripwShSpec.PageIndex-1),tripwShSpec.PageSize);
        }

        public TripSpecifications(int id):base(T=>T.Id==id) //getById
        {
            includes.Add(T => T.FromCity);
            includes.Add(T => T.ToCity);
            includes.Add(T => T.ToCity.Country);
            includes.Add(T => T.FromCity.Country);
            //includes.Add(T => T.FromCity.CountryId);
            //includes.Add(T => T.ToCity.CountryId);
        }

        public TripSpecifications(string userId) : base(t => t.UserId == userId)
        {
            includes.Add(t => t.FromCity);
            includes.Add(t => t.ToCity);
            includes.Add(t => t.ToCity.Country);
            includes.Add(t => t.FromCity.Country);
            // Add more includes if needed
        }



    }
}
