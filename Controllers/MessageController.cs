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
    public class MessageController : Controller
    {
        BorrowContext db = new BorrowContext();
        // GET: Message
        public ActionResult Index(string mesaj)
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    if (mesaj != null)
                        return View(db.Messages.Where(x => x.Comment == mesaj).ToList());
                    else
                        return View(db.Messages.ToList());
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

        // GET: Message/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Message/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Message/Create
        [HttpPost]
        public ActionResult Create(Message message)
        {

            message.Comment = "New";
            db.Messages.Add(message);
            db.SaveChanges();
            return RedirectToAction("Contact", "Home");
        }

        // GET: Message/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Id"] != null)
            {
                return View(db.Messages.Where(x => x.Id == id).FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Message/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Message message)
        {
            message.Comment = "Old";
            db.Entry(message).State = EntityState.Modified;
            db.SaveChanges();
            return View(db.Messages.Where(x => x.Id == id).FirstOrDefault());
        }

        // GET: Message/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Message/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {

            bool result = true;
            Message message = db.Messages.Where(x => x.Id == id).FirstOrDefault();
            db.Messages.Remove(message);
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
