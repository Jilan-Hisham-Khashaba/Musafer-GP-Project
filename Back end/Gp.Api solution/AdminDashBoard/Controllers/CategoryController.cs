using AdminDashBoard.Models;
using AutoMapper;
using GP.APIs.Dtos;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Twilio.Http;

namespace AdminDashBoard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IGenericRepositroy<Category> categoryRepo;
        private readonly IMapper mapper;

        public CategoryController(IGenericRepositroy<Category> categoryRepo, IMapper mapper)
        {
            this.categoryRepo = categoryRepo;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchValue)
        {
            var categories = await categoryRepo.GetAllAsync();
            var CategoryList = new List<CategoryModel>();

            foreach (var category in categories)
            {
                var categoryData = new CategoryModel
                {
                    Id = category.Id,
                    Name = category.TypeName,
                };

                CategoryList.Add(categoryData);
            }

            // تصفية القائمة بناءً على قيمة البحث إذا كانت غير فارغة
            if (!string.IsNullOrEmpty(searchValue))
            {
                CategoryList = CategoryList.Where(c => c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(CategoryList);
        }

        public IActionResult AddCat()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCat(CategoryModel category)
        {
            if (!string.IsNullOrEmpty(category.Name))
            {
                var categoryEntity = mapper.Map<Category>(category);
                await categoryRepo.AddAsync(categoryEntity);
                await categoryRepo.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add category!";
            }

            return View(category);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // احصل على الفئة المراد تعديلها من الريبوزيتوري
            var category = await categoryRepo.GetByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            // قم بتحويل الفئة من نوع Category إلى CategoryModel باستخدام AutoMapper
            var categoryModel = mapper.Map<CategoryModel>(category);

            // عرض الفئة للتعديل في النموذج
            return View(categoryModel);
        }

        [HttpPost]
     
        public IActionResult Edit(int id, CategoryModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {                 
                    var categoryEntity = mapper.Map<Category>(category);
                     categoryRepo.Update(categoryEntity);
                     categoryRepo.SaveChangesAsync();
                   
                   // TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the category. Please try again.");
                    return View(category);
                }
            }

      
            return View(category);
        }

        public ActionResult Delete(int id)
        {
            var category = categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
         
        }
        [HttpPost]
       
        public  async Task< IActionResult> DeleteConfirmed(int id)
        {
            var category =await categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

          await  categoryRepo.DeleteAsync(category.Id);
            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
