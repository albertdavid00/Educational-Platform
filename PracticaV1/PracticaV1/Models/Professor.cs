using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracticaV1.Models
{
    public class Professor
    {
        [Key]
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Gradul didactic este obligatoriu")]
        public string DidacticDegree { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}