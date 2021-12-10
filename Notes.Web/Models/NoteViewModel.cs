using System;
using System.ComponentModel.DataAnnotations;

namespace Notes.Web.Models
{
    public class NoteViewModel
    {

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Details { get; set; }
  
        [MaxLength(100)]
        public string Category { get; set; }
     
        [MaxLength(7)]
        public string Color { get; set; }

        public Guid Id { get; set; }


    }
}
