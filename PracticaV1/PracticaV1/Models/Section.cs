using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracticaV1.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul sectiunii este obligatoriu")]
        public string Title { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set;}
        public virtual ICollection<File> Files { get; set; }
    }
}