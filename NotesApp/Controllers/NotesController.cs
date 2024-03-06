using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NotesApp.Authorization;
using NotesApp.Data;
using NotesApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NotesController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Notes
        public async Task<IActionResult> Index(string searchString, DateTime? startDate,
           DateTime? endDate)
        {
            if (_context.Note == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Note'  is null.");
            }

            var notes = from n in _context.Note
                        select n;

            var currentUserId = _userManager.GetUserId(User);

            notes = notes.Where(n => n.OwnerID == currentUserId);

            IQueryable<string> dateQuery = from n in notes
                                           orderby n.CreatedDate
                                           select n.CreatedDate.Value.Date.ToShortDateString();


            if (!String.IsNullOrEmpty(searchString))
            {
                notes = notes.Where(s => s.Title!.Contains(searchString) || s.Description!.Contains(searchString));
            }


            if (startDate != null && endDate != null)
            {
                notes = notes.Where(x => x.CreatedDate.Value.Date >= startDate.Value.Date && x.CreatedDate.Value.Date <= endDate.Value.Date);
            }

            var noteDateVM = new NoteDateViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                Notes = await notes.ToListAsync()
            };

            return View(noteDateVM);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);

            var noteVM = new NoteViewModel()
            {
                Id = note.Id,
                OwnerID = note.OwnerID,
                Title = note.Title,
                Description = note.Description,
                CreatedDate= note.CreatedDate,
                ExistingImage   = note.ImageFileName,
                LastEdited = note.LastEdited
            };

            if (note == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, note, NoteOperations.Read);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(noteVM);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,OwnerId,Title,Description,CreatedDate")] Note note)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        note.OwnerID = _userManager.GetUserId(User);
        //        var isAuthorized = await _authorizationService.AuthorizeAsync(User, note, NoteOperations.Create);

        //        if (!isAuthorized.Succeeded)
        //        {
        //            return Forbid();
        //        }

        //        _context.Add(note);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(note);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteViewModel noteVM)
        {
            if (ModelState.IsValid)
            {
                noteVM.OwnerID = _userManager.GetUserId(User);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, noteVM, NoteOperations.Create);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                string uniqueFileName = UploadedFile(noteVM);

                Note note = new Note
                {
                    Id = noteVM.Id,
                    OwnerID = noteVM.OwnerID,
                    Title = noteVM.Title,
                    Description = noteVM.Description,
                    CreatedDate = DateTime.Now,
                    ImageFileName = uniqueFileName
                };

                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.FindAsync(id);

            var noteVM = new NoteViewModel()
            {
                Id = note.Id,
                OwnerID = note.OwnerID,
                Title = note.Title,
                Description = note.Description,
                ExistingImage = note.ImageFileName,
                CreatedDate = note.CreatedDate,
                LastEdited  = note.LastEdited,
                
            };

            if (note == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, note, NoteOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(noteVM);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NoteViewModel noteVM)
        {
            if (id != noteVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    noteVM.OwnerID = _userManager.GetUserId(User);
                    var isAuthorized = await _authorizationService.AuthorizeAsync(User, noteVM, NoteOperations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        return Forbid();
                    }

                    var note = await _context.Note.FindAsync(noteVM.Id);
                    note.Title = noteVM.Title;
                    note.Description = noteVM.Description;
                    note.LastEdited = DateTime.Now;
                    //note.CreatedDate = noteVM.CreatedDate;
                    note.OwnerID = noteVM.OwnerID;  

                    if (noteVM.NoteImage != null)
                    {
                        if (noteVM.ExistingImage != null)
                        {
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", noteVM.ExistingImage);
                            System.IO.File.Delete(filePath);
                        }

                        note.ImageFileName = UploadedFile(noteVM);
                    }
  

                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(noteVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);

            var noteVM = new NoteViewModel()
            {
                Id = note.Id,
                OwnerID = note.OwnerID,
                Title = note.Title,
                Description = note.Description,
                CreatedDate = note.CreatedDate,
                ExistingImage = note.ImageFileName,
                LastEdited = note.LastEdited
            };

            if (note == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, note, NoteOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(noteVM);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Note.FindAsync(id);

            if (!string.IsNullOrEmpty(note.ImageFileName))
            {
                var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", note.ImageFileName);
                if (await _context.SaveChangesAsync() > 0)
                {
                    if (System.IO.File.Exists(CurrentImage))
                    {
                        System.IO.File.Delete(CurrentImage);
                    }
                }
            }
            
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, note, NoteOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            if (note != null)
            {
                _context.Note.Remove(note);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Note.Any(e => e.Id == id);
        }

        private string UploadedFile(NoteViewModel model)
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
