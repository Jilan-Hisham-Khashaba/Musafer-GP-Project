using GP.Core.Entities;
using GP.Core.Entities.Orders;
using GP.Core.Specificatios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repositories
{
    public interface IGenericRepositroy<T> where T : BaseEntity 
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<bool> DeleteAsyncComment(int id);

        Task<Trip> GetByIdAsyncTRIP(int id);
        Task<T> GetByIdAsync(int  id);

        Task<List<Order>> GetOrdersCreatedTodayAsync();

        Task<int> AddRangeAsync(IEnumerable<T> entities);
        Task AddAsync(T entity);

    

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<T>> GetAllWithSpecAsyn(ISpecification<T> spec);

        Task<T> GetByIdwithSpecAsyn(ISpecification<T> spec);

        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);

        // إضافة العمليات الخاصة بالعلاقة بين المنتجات والشحنات هنا
        Task AddProductToShipmentAsync(IEnumerable<Product> products, Shipment shipment);
        Task RemoveProductFromShipmentAsync(Product product, Shipment shipment);
        Task<IEnumerable<Product>> GetProductsByShipmentIdAsync(int shipmentId);
        Task<int> SaveChangesAsync();

        
        Task<Product> GetByNameAsync(string Name);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        

        //Task<T> Update(T entity);
        void Update(T entity);

        void Delete(T entity);


        Task<int> CountAsync(Expression<Func<T, bool>> predicate);


        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate); // Added WhereAsync method


    }
}
