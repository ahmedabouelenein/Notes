using System;
using System.ComponentModel.DataAnnotations;

namespace Notes.Api
{
    public class Note
    {
        [Key]
        [Required]
        public Guid Id { get;private set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; private set; }
        [Required]
        [MaxLength(3000)]
        public string Details { get; private set; }

        [MaxLength(100)]
        public string Category { get; private set; }
      
        [MaxLength(7)]
        public string Color { get; private set; }
        [Required]
        public Guid UserId { get; private set; }

        private Note()
        {

        }
        public Note(Guid userId, string title, string details, string category, string color)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Details = details;
            Category = category;
            Color = color;

        }
    }
}
