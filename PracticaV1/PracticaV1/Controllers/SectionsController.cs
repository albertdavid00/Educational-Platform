using Microsoft.AspNet.Identity;
using PracticaV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PracticaV1.Controllers
{
    public class SectionsController : Controller
    {
        private ApplicationDbContext db = new PracticaV1.Models.ApplicationDbContext();
        // GET: Sections
        [Authorize(Roles = "Professor,Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Professor,Admin")]
        public ActionResult New(Section section)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var professorId = db.Professors.Where(p => p.UserId == userId).FirstOrDefault().ProfessorId;
                var courseId = section.CourseId;
                var course = db.Courses.Find(courseId);
                if (ModelState.IsValid && professorId == course.ProfessorId)
                {
                    db.Sections.Add(section);
                    db.SaveChanges();
                    return Redirect("/Courses/Show/" + section.CourseId);
                }
                else
                {
                    return Redirect("/Courses/Show/" + section.CourseId);
                }
            }

            catch (Exception)
            {
                return Redirect("/Courses/Show/" + section.CourseId);
            }
        }


        [Authorize(Roles = "Professor,Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                Section section = db.Sections.Find(id);
                var creatorId = section.Course.Professor.UserId;
                var courseId = section.CourseId;
                if (User.Identity.GetUserId() == creatorId || User.IsInRole("Admin"))
                {
                    db.Sections.Remove(section);
                    db.SaveChanges();
                }

                return Redirect("/Courses/Show/" + courseId.ToString());
            }
            catch (Exception)
            {
                return Redirect("/Courses/Index");
            }
        }
    }
}