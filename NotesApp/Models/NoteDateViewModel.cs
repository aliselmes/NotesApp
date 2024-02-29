using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class NoteDateViewModel
    {
        public List<Note>? Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? NoteDate { get; set; }
        public string? SearchString { get; set; }
    }
}
