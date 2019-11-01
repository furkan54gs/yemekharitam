using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCF5.Context;
using MvcCF5.Models;
using System.Data.Entity;

namespace MvcCF5.Controllers
{

    public class SettingController : Controller
    {
        BorrowContext db = new BorrowContext();
        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        // GET: Setting/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Setting/Create

        public ActionResult Create()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    return View(db.Settings.First());
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

        // POST: Setting/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Setting setting)
        {
            db.Entry(setting).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        // GET: Setting/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Setting/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            return View();
        }

        // GET: Setting/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Setting/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            return View();
        }
    }
}
