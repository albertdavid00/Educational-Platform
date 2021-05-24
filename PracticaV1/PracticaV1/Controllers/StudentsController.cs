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
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var students = db.Students.OrderBy(p => p.LastName);
            ViewBag.students = students;
            return View();
        }
        // Show
        [Authorize(Roles = "Student,Admin,Professor")]
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
                   /* TempData["message"] = "Informatiile au fost adaugate cu succes!";*/
                    return RedirectToAction("Index", "Courses");
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
        public ActionResult Search(string search)
        {


            //SEARCH 
            search = search ?? "";
            /* if (Request.Params.Get("search") != null)
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
             }*/
            List<String> searchItems = new List<string>(search.Split(" .,?!()[]{};:".ToCharArray()));
            searchItems = searchItems.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var students = db.Students;
            List<Student> searchStudents = new List<Student>();
            if(searchItems.Count() > 0)
            {
                foreach (var student in students)
                {
                    foreach (var item in searchItems.ToArray())
                    {
                        if (student.FirstName.Contains(item) || student.LastName.Contains(item))
                        {
                            searchStudents.Add(student);
                            break;
                        }
                    }
                }
            }
            ViewBag.Students = searchStudents;
            ViewBag.Search = search;
            return View("StudentsSearch");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Student student = db.Students.Find(id);
            ApplicationUser user = student.User;
            db.Students.Remove(student);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Student")]
        public ActionResult MyProfile()
        {
            string uid = User.Identity.GetUserId();
            var stud = db.Students.Where(s => s.UserId == uid);
            if (stud.Count() == 0)
            {
                return RedirectToAction("New", "Students");
            }
            else
            {
                int pid = stud.FirstOrDefault().StudentId;   // Current user -> student id
                return RedirectToAction("Show/" + pid.ToString());
            }
        }


        //EDIT
        [Authorize(Roles = "Student,Admin")]
        public ActionResult Edit(int id)
        {
            Student student = db.Students.Find(id);
            if (student.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(student);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Courses");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Student")]
        public ActionResult Edit(int id, Student requestStudent)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Student student = db.Students.Find(id);

                    if (TryUpdateModel(student))
                    {
                        ///* course.CourseName = requestCourse.CourseName;
                        // course.ProfessorId = requestCourse.ProfessorId;*/
                        student = requestStudent;

                        db.SaveChanges();
                        //TempData["message"] = "Profilul a fost modificat";
                        return RedirectToAction("Index", "Courses");
                    }
                    return View(requestStudent);
                }
                else
                {
                    return View(requestStudent);
                }
            }
            catch (Exception)
            {
                return View(requestStudent);
            }
        }

    }
}