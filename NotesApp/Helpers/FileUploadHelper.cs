using Microsoft.AspNetCore.Hosting;
using NotesApp.Models;

namespace NotesApp.Helpers
{
    public class FileUploadHelper: IFileUploadHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UploadedFile(NoteViewModel model)
        {
            string? uniqueFileName = null;

            if (model.NoteImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NoteImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.NoteImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
