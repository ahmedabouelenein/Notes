using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Notes.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly ILogger<NoteController> _logger;
        private readonly NotesDbContext _dbContext;

        public NoteController(ILogger<NoteController> logger, NotesDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<Guid> Create(CreateNoteDto inputDto)
        {
            var userId =User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var note = new Note(userId, inputDto.Title, inputDto.Details, inputDto.Category, inputDto.Color);
            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();
            return note.Id;
        }
        [HttpDelete()]
        public async Task Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var note = await _dbContext.Notes.Where(x => x.UserId == userId).FirstOrDefaultAsync(x => x.Id == id);
            if (note == null)
                return;
            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();

        }
        [HttpGet]
        public async Task<IEnumerable<NoteDto>> Get()
        {

            return await QueryNotes().ToListAsync();
        }
        [HttpGet("GetById")]
        public async Task<NoteDto> Get(Guid id)
        {
            return await QueryNotes().FirstOrDefaultAsync(x => x.Id == id);
        }
        private IQueryable<NoteDto> QueryNotes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return _dbContext.Notes.Where(x => x.UserId == userId).Select(x => new NoteDto()
            {
                Id = x.Id,
                Title = x.Title,
                Category = String.IsNullOrEmpty(x.Category) ? "" : x.Category,
                Color = x.Color,
                Details = x.Details
            });
        }
    }
}
