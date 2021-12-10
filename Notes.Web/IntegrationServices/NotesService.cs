using Notes.Web.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Notes.Web.IntegrationServices
{
    public class NotesService : INotesService
    {
        private readonly HttpClient _httpClient;

        public NotesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<NoteViewModel>> GetNotes()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(string.Format(NotesServiceConfiguration.Methods.Get));
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<NoteViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }
        public async Task<Guid> CreateNote(NoteViewModel note)
        {
            var notePayload = new StringContent(JsonSerializer.Serialize(note), UnicodeEncoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(string.Format(NotesServiceConfiguration.Methods.Create), notePayload);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Guid>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


        }
        public async Task DeleteNote(Guid noteId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(string.Format(NotesServiceConfiguration.Methods.Delete, noteId));
            await response.Content.ReadAsStringAsync();

        }
        public async Task<NoteViewModel> GetNoteById(Guid id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(string.Format(NotesServiceConfiguration.Methods.GetById, id));
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<NoteViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }
    }
}
