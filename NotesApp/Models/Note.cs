using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class Note
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
