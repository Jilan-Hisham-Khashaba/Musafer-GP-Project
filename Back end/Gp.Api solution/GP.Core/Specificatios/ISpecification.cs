using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specificatios
{
    public interface ISpecification<T> where T:BaseEntity
    {
        public Expression<Func<T, bool>> criteria { get; set; } //siganutre for prop,,,,where

        public List<Expression<Func<T, object>>> includes { get; set; }    //include

        public List<Expression<Func<T, object>>> ThenIncludes { get; set; }


        public Expression<Func<T,object>> OrderBy { get; set; }

        public Expression<Func<T, object>> OrderByDescending { get; set; }

        public int Skip { get; set; }

        public int Take  { get; set; }

        public bool IsPaginationEnabled { get; set; }
    }
}
