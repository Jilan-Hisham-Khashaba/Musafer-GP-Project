using GP.Core.Entities;
using GP.Core.Entities.Orders;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GP.Repository
{
    public class GenericRepositorty<T> : IGenericRepositroy<T> where T : BaseEntity
    {
        private readonly StoreContext dbContext;

        public GenericRepositorty(StoreContext dbContext)
        {
            this.dbContext = dbContext;
        }
       
       public async ValueTask DisposeAsync()
        => await dbContext.DisposeAsync();

  

        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            await dbContext.Set<T>().AddRangeAsync(entities);
            return await dbContext.SaveChangesAsync();
        }

   
        #region static qui
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Shipment))
            {
                return (IEnumerable<T>)await dbContext.shipments.Include(T => T.Products).ThenInclude(T => T.Category).ToListAsync();

            }
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifiaction(spec).CountAsync();
        }

      

        #endregion 
        public async Task<IEnumerable<T>> GetAllWithSpecAsyn(ISpecification<T> spec)
        {
            return await ApplySpecifiaction(spec).ToListAsync();
        }

        public async Task<T> GetByIdwithSpecAsyn(ISpecification<T> spec)
        {
            return await ApplySpecifiaction(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecifiaction(ISpecification<T> spec)
        {
            return SpecificationEvalutor<T>.GetQuery(dbContext.Set<T>(), spec);
        }
        //public async Task<T> GetByIdAsyn(int id)
        //{
        //    return await dbContext.Set<T>().FindAsync(id);
        //}
        public async Task<T> GetByIdAsync(int id)
        {
            return await dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Trip> GetByIdAsyncTRIP(int id)
        {
            return await dbContext.Trips.FirstOrDefaultAsync(e => e.Id == id);
        }



        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        public async Task AddProductToShipmentAsync(IEnumerable<Product> products, Shipment shipment)
        {
            // تحميل الشحنة مع المنتجات المرتبطة بها من قاعدة البيانات
            var shipmentWithProducts = await dbContext.shipments
                                                    .Include(s => s.Products)
                                                    .SingleOrDefaultAsync(s => s.Id == shipment.Id);

            // إضافة المنتجات إلى الشحنة
            foreach (var product in products)
            {
                shipmentWithProducts.Products.Add(product);
            }

            // حفظ التغييرات في قاعدة البيانات
            //await dbContext.SaveChangesAsync();
        }

        public Task RemoveProductFromShipmentAsync(Product product, Shipment shipment)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProductsByShipmentIdAsync(int shipmentId)
        {
            throw new NotImplementedException();
        }
        public async Task<Product> GetByNameAsync(string Name)
        {
            // قم بتنفيذ استعلام للحصول على المنتج بناءً على اسمه
            // يعتمد ذلك على هيكل جدول قاعدة البيانات والتسميات المستخدمة
            return await dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == Name);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }
       
        public async Task<bool> DeleteAsync(int id)
        {
            var Delete = await dbContext.Set<T>().FindAsync(id);

            if (Delete == null)
            {
                return false; // Request with the given ID doesn't exist
            }

            dbContext.Remove(Delete);
            await dbContext.SaveChangesAsync();

            return true; // Request deleted successfully
        }
        public async Task<bool> DeleteAsyncComment(int id)
        {
            var Delete = await dbContext.Set<Comments>().FindAsync(id);

            if (Delete == null)
            {
                return false; // Request with the given ID doesn't exist
            }

            dbContext.Remove(Delete);
            await dbContext.SaveChangesAsync();

            return true; // Request deleted successfully
        }


        public void Delete(T entity)

      => dbContext.Set<T>().Remove(entity);

        public void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        public async Task AddAsync(T entity)
          => await dbContext.Set<T>().AddAsync(entity);

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {

            return await dbContext.Set<T>().CountAsync(predicate);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            // Apply the predicate to the DbSet and return the filtered result asynchronously
            return await dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        // الدالة الجديدة لجلب الطلبات التي تم إنشاؤها اليوم
        public async Task<List<Order>> GetOrdersCreatedTodayAsync()
        {
            var today = DateTime.Today;
            return await dbContext.Orders
                .Where(o => EF.Functions.DateDiffDay(o.DateOfCreation, today) == 0)
                .ToListAsync();
        }
    }
}

