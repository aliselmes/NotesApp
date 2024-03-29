﻿using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class NoteViewModel: Note
    {
        [Display(Name = "Image")]
        public IFormFile? NoteImage { get; set; }

        [Display(Name = "Image")]
        public string? ExistingImage { get; set; }
    }
}
