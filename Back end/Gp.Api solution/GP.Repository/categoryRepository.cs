using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository
{
    public class categoryRepository : GenericRepositorty<Category>, ICategoryRepository
    {
        private readonly StoreContext dbContext;
        public categoryRepository(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    
        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(c => c.TypeName == categoryName);
        }
    }
}
