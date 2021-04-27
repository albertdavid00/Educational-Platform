using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracticaV1.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Anul de studiu este obligatoriu")]
        public int YearOfStudy { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

    }
}