using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class NoteDateViewModel
    {
        public List<Note>? Notes { get; set; }
        public SelectList? Dates { get; set; }
        public string? NoteDate { get; set; }
        public string? SearchString { get; set; }
    }
}
