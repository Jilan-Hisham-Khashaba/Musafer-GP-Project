namespace Gp.Api.Hellpers
{
    public class DocumentSetting
    {
        public static string UploadImage(IFormFile file, string FolderName)
        {
            try
            {
                string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FolderName);
                //step 2 Get file Name and Make it Unique 
                string fileName = $"{Guid.NewGuid()}{file.FileName}";
                //3. Get FilePath
                string FilePath = Path.Combine(FolderPath, fileName);

                var fs = new FileStream(FilePath, FileMode.Create);
                file.CopyTo(fs);

                return fileName;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"An error occurred while uploading the image: {ex.Message}");
                return null;
            }
        }
    }
}
