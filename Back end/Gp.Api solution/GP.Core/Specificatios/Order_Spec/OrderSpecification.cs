using GP.Core.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace GP.Core.Specificatios.Order_Spec
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string emailORUserId)
            : base(o => o.Email == emailORUserId ||o.recievedUserId==emailORUserId)
        {
            includes.Add(o => o.Request);
            includes.Add(o => o.Request.Shipment);
            //includes.Add(o => o.Request.Shipment.Products.Select(sh=>sh.Category));
            includes.Add(o => o.Request.Shipment.FromCity.Country);
            includes.Add(o => o.Request.Shipment.ToCity.Country);
            includes.Add(o => o.Request.Trip.FromCity.Country);
            includes.Add(o => o.Request.Trip.ToCity.Country);
            includes.Add(o => o.Request.Trip);
            includes.Add(o => o.Request.Shipment.FromCity);
           includes.Add(o => o.Request.Shipment.ToCity);
            includes.Add(o => o.Request.Trip.FromCity);
            includes.Add(o => o.Request.Trip.ToCity);
          //  AddThenInclude(o => o.Request.Shipment.Products);


            AddOrderByDescending(o => o.Request.Trip.arrivalTime);
        }
        public OrderSpecification(int id,string email)
            : base(o => o.Email == email || o.recievedUserId == email && o.Id==id )
        {
            includes.Add(o => o.Request);
            includes.Add(o => o.Request.Shipment);
            includes.Add(o => o.Request.Shipment.Products);
            includes.Add(o => o.Request.Shipment.FromCity.Country);
            includes.Add(o => o.Request.Shipment.ToCity.Country);
            includes.Add(o => o.Request.Trip.FromCity.Country);
            includes.Add(o => o.Request.Trip.ToCity.Country);
            includes.Add(o => o.Request.Trip);
            includes.Add(o => o.Request.Shipment.FromCity);
            includes.Add(o => o.Request.Shipment.ToCity);
            includes.Add(o => o.Request.Trip.FromCity);
            includes.Add(o => o.Request.Trip.ToCity);
           


           
        }
        public OrderSpecification(int id)
       : base(o => o.Id == id)
        {
            includes.Add(o => o.Request);
            includes.Add(o => o.Request.Shipment);
            includes.Add(o => o.Request.Shipment.Products);
            includes.Add(o => o.Request.Shipment.FromCity.Country);
            includes.Add(o => o.Request.Shipment.ToCity.Country);
            includes.Add(o => o.Request.Trip.FromCity.Country);
            includes.Add(o => o.Request.Trip.ToCity.Country);
            includes.Add(o => o.Request.Trip);
            includes.Add(o => o.Request.Shipment.FromCity);
            includes.Add(o => o.Request.Shipment.ToCity);
            includes.Add(o => o.Request.Trip.FromCity);
            includes.Add(o => o.Request.Trip.ToCity);




        }
        public OrderSpecification()
          : base()
        {
            includes.Add(o => o.Request);
            includes.Add(o => o.Request.Shipment);
            includes.Add(o => o.Request.Shipment.Products);
            includes.Add(o => o.Request.Shipment.FromCity.Country);
            includes.Add(o => o.Request.Shipment.ToCity.Country);
            includes.Add(o => o.Request.Trip.FromCity.Country);
            includes.Add(o => o.Request.Trip.ToCity.Country);
            includes.Add(o => o.Request.Trip);
            includes.Add(o => o.Request.Shipment.FromCity);
            includes.Add(o => o.Request.Shipment.ToCity);
            includes.Add(o => o.Request.Trip.FromCity);
            includes.Add(o => o.Request.Trip.ToCity);




        }


    }
}
