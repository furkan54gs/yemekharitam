using MvcCF5.Context;
using MvcCF5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCF5.Controllers
{
    public class CategoryController : Controller
    {
        BorrowContext db = new BorrowContext();

        // GET: Category
        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    return View(db.Categories.ToList());
                }
                else
                {
                    return RedirectToAction("NotAuthorized", "Error");
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            if (Session["Id"] != null)
            {
                return View();
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category category)
        {
            
            db.Categories.Add(category);
            db.SaveChanges();
            return RedirectToAction("Index");
                
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Id"] != null)
            {
                return View(db.Categories.Where(x => x.Id == id).FirstOrDefault());
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }
            
        }

        // POST: Category/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Category category)
        {

            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Category/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            bool result = true;
            Category category = db.Categories.Where(x => x.Id == id).FirstOrDefault();
            db.Categories.Remove(category);
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
