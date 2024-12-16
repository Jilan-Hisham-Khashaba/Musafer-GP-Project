using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repositories
{
    public interface ICategoryRepository : IGenericRepositroy<Category>
    {

        Task<Category> GetCategoryByNameAsync(string categoryName);
        // يمكنك إضافة المزيد من الوظائف حسب احتياجاتك للمدينة
    }

}
