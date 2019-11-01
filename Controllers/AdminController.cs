using MvcCF5.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCF5.Controllers
{
    public class AdminController : Controller
    {
        BorrowContext db = new BorrowContext();

        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {

                int p = db.Products.Count();
                ViewBag.p = p;
                int o = db.Orders.Count();
                ViewBag.o = o;
                int s = db.Suppliers.Count();
                ViewBag.s = s;
                int u = db.UserAccounts.Count()-s;
                ViewBag.u = u;
                int m = db.Messages.Count();
                ViewBag.m = m;
                int c = db.Categories.Count();
                ViewBag.c = c;
                int y = db.Comments.Count();
                ViewBag.y = y;
             
                return View();
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

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            return View();
        }
    }
}
