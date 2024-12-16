
using GP.core.Sepecifitction;
using GP.Core.Entities;
using GP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specificatios
{
    public class ShipmentCountSpecification : BaseSpecification<Shipment>
    {
        private readonly IGenericRepositroy<Shipment> shipmentRepo;
       

        public ShipmentCountSpecification()
        {
            
        }
        public ShipmentCountSpecification( IGenericRepositroy<Shipment> shipmentRepo)
          
        {
            this.shipmentRepo = shipmentRepo;
        }

        public async Task<int> GetShipmentCountAsync(string userID)
        {
            var specification = new ShipmentSpecification(new TripwShSpecParams { /* البارامترات اللازمة لتحديد المواصفات */ });

            var shipments = await shipmentRepo.GetAllWithSpecAsyn(specification);

            var userShipments = shipments.Where(s => s.IdentityUserId == userID);

            return userShipments.Count();
        }
        public async Task<int> GetAllShipmentCountAsync()
        {
        
            var specification = new ShipmentSpecification(new TripwShSpecParams { /* البارامترات اللازمة لتحديد المواصفات */ });

            // استدعاء جميع الشحنات باستخدام المواصف
            var shipments = await shipmentRepo.GetAllWithSpecAsyn(specification);

            // حساب عدد الشحنات
            return shipments.Count();
        }

    }


}
