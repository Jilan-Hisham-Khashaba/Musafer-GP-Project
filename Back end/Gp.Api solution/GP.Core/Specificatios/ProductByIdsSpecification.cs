using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specificatios
{
    public class ProductByIdsSpecification : BaseSpecification<Product>
    {
        private readonly int? _productId;

        public ProductByIdsSpecification(int? productId)
        {
            _productId = productId;
        }

        public Expression<Func<Product, bool>> Criteria => product => product.Id == _productId;
    }
}
