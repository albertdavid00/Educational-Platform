using Microsoft.AspNet.Identity;
using PracticaV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PracticaV1.Controllers
{
    public class ProfessorsController : Controller
    {
        private ApplicationDbContext db = new PracticaV1.Models.ApplicationDbContext();

        // GET: Professors
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var professors = db.Professors.OrderBy(p => p.LastName);
            ViewBag.professors = professors;
            return View();
        }
        // Show
        [Authorize(Roles = "Student,Admin,Professor")]
        public ActionResult Show(int id)
        {
            Professor prof = db.Professors.Find(id);
            ViewBag.UserId = User.Identity.GetUserId();

            return View(prof);
        }

        [Authorize(Roles = "Professor,Admin")]
        public ActionResult New()
        {
            Professor professor = new Professor();
            professor.UserId = User.Identity.GetUserId();
            string uid = User.Identity.GetUserId();
            var professors = from p in db.Professors
                           where p.UserId == uid
                           select p;
            ViewBag.Professors = professors.Count();
            if (professors.Count() == 0)
                return View(professor);
            
            return RedirectToAction("Index", "Professors");
        }

        [HttpPost]
        [Authorize(Roles = "Professor,Admin")]
        public ActionResult New(Professor professor)
        {
            
            professor.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Professors.Add(professor);
                    db.SaveChanges();
                   /* TempData["message"] = "Informatiile au fost adaugate cu succes!";*/
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(professor);
                }
            }
            catch (Exception)
            {
                return View(professor);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Professor prof = db.Professors.Find(id);
            ApplicationUser user = prof.User;
            db.Professors.Remove(prof);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Professors");

        }

        [Authorize(Roles = "Professor")]
        public ActionResult MyProfile()
        {
            string uid = User.Identity.GetUserId();
            var prof = db.Professors.Where(s => s.UserId == uid);
            if (prof.Count() == 0)
            {
                return RedirectToAction("New", "Professors");
            }
            else
            {
                int pid = prof.FirstOrDefault().ProfessorId;   // Current user -> professor id
                return RedirectToAction("Show/" + pid.ToString());
            }
        }

        //EDIT
        [Authorize(Roles = "Professor,Admin")]
        public ActionResult Edit(int id)
        {
            Professor prof = db.Professors.Find(id);
            if (prof.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(prof);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Courses");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Professor")]
        public ActionResult Edit(int id, Professor requestProfessor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Professor prof = db.Professors.Find(id);

                    if (TryUpdateModel(prof))
                    {
                        ///* course.CourseName = requestCourse.CourseName;
                        // course.ProfessorId = requestCourse.ProfessorId;*/
                        prof = requestProfessor;

                        db.SaveChanges();
                        //TempData["message"] = "Profilul a fost modificat";
                        return RedirectToAction("Index", "Courses");
                    }
                    return View(requestProfessor);
                }
                else
                {
                    return View(requestProfessor);
                }
            }
            catch (Exception)
            {
                return View(requestProfessor);
            }
        }
    }
}