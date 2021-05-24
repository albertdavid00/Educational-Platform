using Microsoft.AspNet.Identity;
using PracticaV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PracticaV1.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new PracticaV1.Models.ApplicationDbContext();
        // GET: Courses
        [Authorize(Roles ="Student,Professor,Admin")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Student"))
            {
                var student = db.Students.Where(s => s.UserId == userId).FirstOrDefault();
                var courses = student.Courses;
                ViewBag.Courses = courses;

                return View();
            }
            else if (User.IsInRole("Professor"))
            {
                var professor = db.Professors.Where(p => p.UserId == userId).FirstOrDefault();
                var courses = professor.Courses;
                ViewBag.Courses = courses;
                return View();
            }
            else  // user == admin
            {
                var courses = db.Courses;
                ViewBag.Courses = courses;
                if (TempData.ContainsKey("message"))
                {
                    ViewBag.Message = TempData["message"];
                }

                return View();
            }
        }
        // Show
        [Authorize(Roles ="Student,Professor,Admin")]
        public ActionResult Show(int id)
        {
            Course course = db.Courses.Find(id);
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Professor")){
                var professorId = db.Professors.Where(p => p.UserId == userId).FirstOrDefault().ProfessorId;
                if (professorId != course.ProfessorId)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            Course course = new Course();
            course.Profs = GetAllProfessors();

            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    TempData["message"] = "Cursul a fost adaugat cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    course.Profs = GetAllProfessors();
                    return View(course);
                }
            }
            catch (Exception)
            {
                course.Profs = GetAllProfessors();
                return View(course);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Course course = db.Courses.Find(id);
            course.Profs = GetAllProfessors();

            return View(course);
            
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Course requestCourse)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Course course = db.Courses.Find(id);

                    if (TryUpdateModel(course))
                    {
                        ///* course.CourseName = requestCourse.CourseName;
                        // course.ProfessorId = requestCourse.ProfessorId;*/
                        course = requestCourse;
                        
                        db.SaveChanges();
                        TempData["message"] = "Cursul a fost modificat";
                        return RedirectToAction("Index");
                    }
                    return View(requestCourse);
                }
                else
                {
                    requestCourse.Profs = GetAllProfessors();
                    return View(requestCourse);
                }
            }
            catch (Exception)
            {
                requestCourse.Profs = GetAllProfessors();
                return View(requestCourse);
            }
        }

        [Authorize(Roles ="Admin")]
        public ActionResult StudentsToEnroll(int id)
        {
            Course course = db.Courses.Find(id);
            var studentsAlreadyEnrolled = course.Students;
            var allStudents = db.Students.ToList();
            List<Student> studentsToEnroll = new List<Student>();
            foreach (var student in allStudents)
            {
                bool alreadyEnrolled = studentsAlreadyEnrolled.Contains(student);
                if (!alreadyEnrolled)
                {
                    studentsToEnroll.Add(student);
                }
            }
            ViewBag.Students = studentsToEnroll;
            return View(course);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult EnrollStudent(int id, int id2)
        {
            Course course = db.Courses.Find(id);
            Student student = db.Students.Find(id2);
            student.Courses.Add(course);
            course.Students.Add(student);
            db.SaveChanges();
            return RedirectToAction("StudentsToEnroll/" + id.ToString());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult StudentsToRemove(int id)
        {
            Course course = db.Courses.Find(id);
            var studentsAlreadyEnrolled = course.Students.ToList();
            ViewBag.Students = studentsAlreadyEnrolled;
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveStudent(int id, int id2)
        {
            Course course = db.Courses.Find(id);
            Student student = db.Students.Find(id2);
            student.Courses.Remove(course);
            course.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("StudentsToRemove/" + id.ToString());
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                Course course = db.Courses.Find(id);
                db.Courses.Remove(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllProfessors()
        {
            var selectList = new List<SelectListItem>();
            var professors = from prof in db.Professors
                             select prof;
            foreach(var prof in professors)
            {
                selectList.Add(new SelectListItem
                {
                    Value = prof.ProfessorId.ToString(),
                    Text = prof.FirstName.ToString() + " " + prof.LastName.ToString()
                });
            }
            return selectList;
        }
    }
}