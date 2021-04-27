using Microsoft.AspNet.Identity;
using PracticaV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PracticaV1.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new PracticaV1.Models.ApplicationDbContext();
        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.OrderBy(p => p.LastName);
            ViewBag.students = students;
            return View();
        }
        // Show
        public ActionResult Show(int id)
        {
            Student student = db.Students.Find(id);
            ViewBag.UserId = User.Identity.GetUserId();
            return View(student);
        }

        [Authorize(Roles = "Student,Admin")]
        public ActionResult New()
        {
            Student student = new Student();
            student.UserId = User.Identity.GetUserId();
            string uid = User.Identity.GetUserId();
            var students = from s in db.Students
                             where s.UserId == uid
                             select s;
            ViewBag.Students = students.Count();
            if (students.Count() == 0)
                return View(student);

            return RedirectToAction("Index", "Students");
        }

        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        public ActionResult New(Student student)
        {

            student.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    TempData["message"] = "Informatiile au fost adaugate cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(student);
                }
            }
            catch (Exception)
            {
                return View(student);
            }
        }

        [Authorize(Roles = "Student,Admin,Professor")]
        public ActionResult Search()
        {
            

            //SEARCH 
            var search = "";
            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                var students = db.Students.Where(s => s.LastName.Contains(search));
                ViewBag.Students = students;
            }
            ViewBag.SearchString = search;
            // END OF SEARCH BAR
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View("StudentsSearch");
        }
    }
}