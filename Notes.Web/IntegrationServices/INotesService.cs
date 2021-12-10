using Notes.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Web.IntegrationServices
{
    public interface INotesService
    {
        Task<NoteViewModel> GetNoteById(Guid id);
        Task<List<NoteViewModel>> GetNotes();
        Task<Guid> CreateNote(NoteViewModel note);
        Task DeleteNote(Guid noteId);
    }
}