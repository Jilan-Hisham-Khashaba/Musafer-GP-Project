using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specificatios
{
    public class CitySpecification : BaseSpecification<City>
    {
         public CitySpecification()
        {
            includes.Add(T => T.Country);
        }
    }
}
