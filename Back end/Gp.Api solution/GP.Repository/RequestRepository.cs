using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Emgu.CV.Ocl;
using GP.Core.Specificatios;


namespace GP.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly StoreContext dbContext;

        public RequestRepository(StoreContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> DeleteRequestAsync(int RequestId)
        {
            var requestToDelete = await dbContext.Requests.FindAsync(RequestId);

            if (requestToDelete == null)
            {
                return false; // Request with the given ID doesn't exist
            }

            dbContext.Requests.Remove(requestToDelete);
            await dbContext.SaveChangesAsync();

            return true; // Request deleted successfully
        }


        //public async Task<Request> GetRequestAsync(int requestId)
        //{
        //    // استعلام للحصول على بيانات الطلب مع تفاصيل الشحنة والرحلة
        //    var request = await dbContext.Requests
        //        .Include(r => r.Shipment).Include(r=>r.Shipment.FromCity).Include(r=>r.Shipment.ToCity)
        //        .Include(r => r.Trip).Include(r=>r.Trip.FromCity).Include(r=>r.Trip.ToCity)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(r => r.RequestId == requestId);

        //    // تحديد خيارات التسلسل
        //    var options = new JsonSerializerOptions
        //    {
        //        ReferenceHandler = ReferenceHandler.Preserve
        //    };

        //    // تسلسل كائن الطلب إلى سلسلة JSON باستخدام الخيارات المحددة
        //    var jsonString = JsonSerializer.Serialize(request, options);

        //    // فك تسلسل سلسلة JSON إلى كائن Request
        //    var deserializedRequest = JsonSerializer.Deserialize<Request>(jsonString, options);

        //    return deserializedRequest;
        //}
        public async Task<Request> GetRequestAsync(int requestId)
        {
            var request = await dbContext.Requests
                .Include(r => r.Shipment).Include(r => r.Shipment.FromCity).Include(r => r.Shipment.ToCity)
                .Include(r => r.Trip).Include(r => r.Trip.FromCity).Include(r => r.Trip.ToCity)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            return request;
        }

        public async Task<Request> GetRequestAsyncForAdmin(int requestId)
        {
            // استعلام للحصول على بيانات الطلب مع تفاصيل الشحنة والرحلة
            var request = await dbContext.Requests
                .Include(r => r.Shipment).Include(r => r.Shipment.FromCity).Include(r => r.Shipment.ToCity).Include(r=>r.Shipment.Products)
                .Include(r => r.Trip).Include(r => r.Trip.FromCity).Include(r => r.Trip.ToCity)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            // تحديد خيارات التسلسل
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // تسلسل كائن الطلب إلى سلسلة JSON باستخدام الخيارات المحددة
            var jsonString = JsonSerializer.Serialize(request, options);

            // فك تسلسل سلسلة JSON إلى كائن Request
            var deserializedRequest = JsonSerializer.Deserialize<Request>(jsonString, options);

            return deserializedRequest;
        }

        public void Detach(Request request)
        {
            dbContext.Entry(request).State = EntityState.Detached;
        }


        public async Task<Request> UpdateRequestAsync(Request request)
        {
            // التحقق مما إذا كان هناك طلبات سابقة تحمل نفس رقم الرحلة ونفس رقم الشحنة
            var existingRequest = await dbContext.Requests.FirstOrDefaultAsync(r => r.TripId == request.TripId && r.ShipmentId == request.ShipmentId);

            // إذا وُجد طلب مسبق مع نفس رقم الرحلة ونفس رقم الشحنة
            if (existingRequest != null)
            {
                // يمكنك تخصيص رسالة الخطأ حسب احتياجاتك
                throw new Exception("Request with the same trip and shipment already exists.");
            }

            // لا يوجد طلب مسبق بهذه البيانات، يتم إضافة الطلب الجديد

            // التحقق مما إذا كانت القيمة TripId موجودة في جدول Trips
            var existingTrip = await dbContext.Trips.FindAsync(request.TripId);
            var existingShipment = await dbContext.shipments.FindAsync(request.ShipmentId);

            if (existingTrip == null)
            {
                // يمكنك تخصيص رسالة الخطأ حسب احتياجاتك
                throw new Exception("TripId does not exist in Trips table");
            }
            if (existingShipment == null)
            {
                // يمكنك تخصيص رسالة الخطأ حسب احتياجاتك
                throw new Exception("ShipmentId does not exist in Shipments table");
            }

            // القيمة موجودة، يتم إجراء الإدخال في جدول Requests
            dbContext.Requests.Add(request);

            // حفظ التغييرات في قاعدة البيانات
            await dbContext.SaveChangesAsync();

            // إرجاع السجل الجديد الذي تمت إضافته
            return request;
        }
        public async Task UpdateRequestAsync2(Request request)
        {
            dbContext.Entry(request).State = EntityState.Modified;

            // Detach related entities to avoid tracking issues
            if (request.Shipment != null)
            {
                dbContext.Entry(request.Shipment).State = EntityState.Detached;
                if (request.Shipment.FromCity != null)
                    dbContext.Entry(request.Shipment.FromCity).State = EntityState.Detached;
                if (request.Shipment.ToCity != null)
                    dbContext.Entry(request.Shipment.ToCity).State = EntityState.Detached;
            }

            if (request.Trip != null)
            {
                dbContext.Entry(request.Trip).State = EntityState.Detached;
                if (request.Trip.FromCity != null)
                    dbContext.Entry(request.Trip.FromCity).State = EntityState.Detached;
                if (request.Trip.ToCity != null)
                    dbContext.Entry(request.Trip.ToCity).State = EntityState.Detached;
            }

            dbContext.Requests.Update(request);
            await dbContext.SaveChangesAsync();
        }



        public async Task<List<Core.Entities.Request>> GetAllRequestsAsync()
        {
            var AllRequest= await dbContext.Requests
                .Include(r => r.Shipment).Include(r => r.Shipment.FromCity).Include(r => r.Shipment.ToCity)
                .Include(r => r.Trip).Include(r => r.Trip.FromCity).Include(r => r.Trip.ToCity)
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // تسلسل كائن الطلب إلى سلسلة JSON باستخدام الخيارات المحددة
            var jsonString = JsonSerializer.Serialize(AllRequest, options);

            // فك تسلسل سلسلة JSON إلى كائن Request
            var deserializedRequest = JsonSerializer.Deserialize<List<Core.Entities.Request>>(jsonString, options);

            return deserializedRequest;

        }
        public async Task<List<Core.Entities.Request>> GetAllRequestsAsyncCount()
        {
            var allRequests = await dbContext.Requests
                .Include(r => r.Shipment)
                .ThenInclude(s => s.FromCity)
                .Include(r => r.Shipment)
                .ThenInclude(s => s.ToCity)
                .Include(r => r.Trip)
                .ThenInclude(t => t.FromCity)
                .Include(r => r.Trip)
                .ThenInclude(t => t.ToCity)
                .ToListAsync();

            return allRequests;
        }



        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        
    }
}
