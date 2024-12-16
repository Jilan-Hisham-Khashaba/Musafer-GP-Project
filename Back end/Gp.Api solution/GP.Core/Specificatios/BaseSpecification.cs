using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specificatios
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> criteria { get; set; }//where
        public List<Expression<Func<T, object>>> includes { get; set; } = new List<Expression<Func<T, object>>>(); //include
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        
        public async Task<int> CountAsync(IQueryable<T> query)
        {
            return await query.CountAsync(criteria);
        }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }
        public List<Expression<Func<T, object>>> ThenIncludes { get; set; } = new List<Expression<Func<T, object>>>();

        public BaseSpecification()
        {

        }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            this.criteria = criteria;


        }

        public void AddOrderBy(Expression<Func<T, object>> OrderBy)
        {
            this.OrderBy = OrderBy;
        }
        public void AddOrderByDescending(Expression<Func<T, object>> OrderByDescending)
        {
            this.OrderByDescending = OrderByDescending;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;
        }


        public void AddThenInclude(Expression<Func<T, object>> thenInclude)
        {
            ThenIncludes.Add(thenInclude);
        }

        // تحويل التضمينات إلى تضمينات فعلية مع ThenInclude
        public void ApplyIncludes(IQueryable<T> query)
        {
            foreach (var includeExpression in includes)
            {
                query = query.Include(includeExpression);
            }

            foreach (var thenIncludeExpression in ThenIncludes)
            {
                // تحويل تضمينات ThenInclude إلى تضمينات فعلية
                // query = query.ThenInclude(thenIncludeExpression); // يمكنك استخدام هذا إذا كنت تستخدم Entity Framework Core 6.0 أو أحدث
                                                                   // query = query.Include(thenIncludeExpression);

                var path = (thenIncludeExpression.Body as MemberExpression)?.ToString();
                if (path == null)
                    throw new ArgumentException("Expression must be a member expression");

                query = query.Include(path);
            }

        }
      
    }
}
