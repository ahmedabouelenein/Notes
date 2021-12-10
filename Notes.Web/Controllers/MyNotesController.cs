using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notes.Web.IntegrationServices;
using Notes.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Notes.Web.Controllers
{
    [Authorize]
    public class MyNotesController : Controller
    {
        private readonly ILogger<MyNotesController> _logger;
        private readonly INotesService _notesService;

        public MyNotesController(ILogger<MyNotesController> logger, INotesService notesService)
        {
            _logger = logger;
            _notesService = notesService;
        }

        public async Task<IActionResult> Index()
        {
            var notes = await _notesService.GetNotes();
            return View("Index", notes);
        }
        public async Task<IActionResult> Create()
        {
            return View("Create", new NoteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Save(NoteViewModel noteViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Create");
            }
            var notes = await _notesService.CreateNote(noteViewModel);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete([Required]Guid id)
        {
           
            await _notesService.DeleteNote(id);
            return RedirectToAction("Index");
        }
    }
}
