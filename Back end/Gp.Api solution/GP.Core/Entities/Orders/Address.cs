using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities.Orders
{
    //1 to 1 total 
    public class Address
    {
        public Address()
        {
            
        }
        public Address(string fName, string lName, string street, string city, string country)
        {
            FName = fName;
            LName = lName;
            Street = street;
            City = city;
            Country = country;
        }

        public string FName { get; set; }

        public string LName { get; set; }

        public string Street { get; set; }


        public string City { get; set; }


        public string Country { get; set; }


    }
}
