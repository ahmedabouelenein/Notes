using Microsoft.EntityFrameworkCore;

namespace Notes.Api
{
    public class NotesDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public NotesDbContext(DbContextOptions<NotesDbContext> dbContextOptions) :
            base(dbContextOptions)
        {
        }
    }
}
