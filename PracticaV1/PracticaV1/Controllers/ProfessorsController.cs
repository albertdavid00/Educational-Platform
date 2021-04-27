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
        public ActionResult Index()
        {
            var professors = db.Professors.OrderBy(p => p.LastName);
            ViewBag.professors = professors;
            return View();
        }
        // Show
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
                    TempData["message"] = "Informatiile au fost adaugate cu succes!";
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

        
    }
}