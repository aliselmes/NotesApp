using NotesApp.Models;

namespace NotesApp.Helpers
{
    public interface IFileUploadHelper
    {
        string UploadedFile(NoteViewModel model);
    }
}

