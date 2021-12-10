namespace Notes.Api.Dtos
{
    public class CreateNoteDto
    {
        public string Title { get; set; }

        public string Details { get; set; }

        public string Category { get; set; }

        public string Color { get; set; }
    }
}
