using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PracticaV1.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Denumirea este obligatorie")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Profesorul este obligatoriu")]
        public int ProfessorId { get; set; }
        public virtual Professor Professor { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Section> Sections { get; set; }

        public IEnumerable<SelectListItem> Profs { get; set; }
    }
}