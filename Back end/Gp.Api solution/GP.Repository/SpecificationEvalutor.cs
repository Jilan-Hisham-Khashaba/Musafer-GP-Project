using GP.Core.Entities;
using GP.Core.Specificatios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace GP.Repository
{
    public static class SpecificationEvalutor<TEntity> where TEntity : BaseEntity
    {//dbcontext.trips
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;//context.trip

            if (spec.criteria is not null)
                query = query.Where(spec.criteria);//p=>p.id ==id 
                                                   //context.trip.where(p=>p.id ==id)
            if(spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending );

            if(spec.IsPaginationEnabled)
                query =query.Skip(spec.Skip).Take(spec.Take);

            query = spec.includes.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));
            //context.trip.where(p=>p.id ==id).include(p=>p.tocity).include(p=>p.fromcity)
            // Apply then includes
           
            return query;
        }
    }
}
