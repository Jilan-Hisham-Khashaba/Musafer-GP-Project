using AutoMapper;
using Gp.Api.Dtos;
using GP.Core.Entities;

namespace Gp.Api.Hellpers
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductsDto, string>
    {
        private readonly IConfiguration configuration;

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Product source, ProductsDto destination, string destMember, ResolutionContext context)
        {
            //if (source.Products != null && source.Products.Any())
            //{
                
            //    var firstProduct = source.Products.First();
                if (!string.IsNullOrEmpty(source.PictureUrl))
                {
                    return $"{configuration["ApiBaseUrl"]}images/products/{source.PictureUrl}";
                }
            
            return string.Empty;

        }
        //public static string UploadImage(IFormFile file, string FolderName)
        //{
        //    try
        //    {
        //        string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FolderName);
        //        //step 2 Get file Name and Make it Unique 
        //        string fileName = $"{Guid.NewGuid()}{file.FileName}";
        //        //3. Get FilePath
        //        string FilePath = Path.Combine(FolderPath, fileName);

        //        var fs = new FileStream(FilePath, FileMode.Create);
        //        file.CopyTo(fs);

        //        return fileName;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions
        //        Console.WriteLine($"An error occurred while uploading the image: {ex.Message}");
        //        return null;
        //    }
        //}
    }
}
