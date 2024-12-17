namespace WhiteLagoon.Web.Helpers
{
    public class ImageSettings<TEntity>
    {
        public static void DeleteFile(string PictureURL)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", PictureURL);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static string GetImageURL(IFormFile file)
        {
            var folderName = typeof(TEntity).Name;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot" ,"Images",folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath,fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            return Path.Combine("Images", folderName,fileName);
        }
    }
}
