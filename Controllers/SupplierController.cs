using MvcCF5.Context;
using MvcCF5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCF5.Controllers
{
    public class SupplierController : Controller
    {


        BorrowContext db = new BorrowContext();
        // GET: Supplier
        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    return View(db.Suppliers.ToList());
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

        // GET: Supplier/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Supplier/Create
        public ActionResult Create()
        {
            if (Session["Id"] != null)
            {
                List<Product> list3 = db.Products.ToList();
                ViewBag.prod = new SelectList(list3, "Id", "Name");
                return View();
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }

        }

        // POST: Supplier/Create
        [HttpPost]
        public ActionResult Create(Supplier supplier)
        {

            db.Suppliers.Add(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: Supplier/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Id"] != null)
            {
                List<Product> list3 = db.Products.ToList();
                ViewBag.prod = new SelectList(list3, "Id", "Name");
                return View(db.Suppliers.Where(x => x.Id == id).FirstOrDefault());
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }

        }

        // POST: Supplier/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Supplier supplier)
        {
            db.Entry(supplier).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Supplier/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Supplier/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            bool result = true;
            Supplier supplier = db.Suppliers.Where(x => x.Id == id).FirstOrDefault();
            db.Suppliers.Remove(supplier);
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImageUpload(int id)
        {
            return View(db.Suppliers.Where(x => x.Id == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult ImageUpload(int id, HttpPostedFileBase uploadfile)
        {
            if (uploadfile.ContentLength > 0)
            {
                string guiandname = (Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadfile.FileName));
                string filePath = Path.Combine(Server.MapPath("~/Content/images"), guiandname);
                uploadfile.SaveAs(filePath); //dosyayı klasöre kaydediyor
                Supplier supplier = db.Suppliers.Where(x => x.Id == id).FirstOrDefault();
                supplier.Image = guiandname;

                int userId = db.Suppliers.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
                Image img = db.Images.Where(x => x.UserImg == userId).FirstOrDefault();
                img.Img = guiandname;

                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View(db.Suppliers.Where(x => x.Id == id).FirstOrDefault());

        }
        public ActionResult ImageDelete(int id)
        {

            int userId = db.Suppliers.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
            Image img = db.Images.Where(x => x.UserImg == userId).FirstOrDefault();
            img.Img = null;

            Supplier supplier = db.Suppliers.Where(x => x.Id == id).FirstOrDefault();
            supplier.Image = null;
            db.SaveChanges();

            return RedirectToAction("ImageUpload", new { id });
        }

        public ActionResult ShowProduct(int id)
        {
            return View(db.Products.Where(x => x.SupplierId == id).ToList());
        }

        public ActionResult SuppMapView(int id)
        {
            return View(db.Suppliers.Where(x => x.Id == id).FirstOrDefault());

        }

        [HttpPost]
        public ActionResult SuppMapView(int id, string latitude, string longtitude)
        {

            latitude = latitude.Replace('.', ',');
            longtitude = longtitude.Replace('.', ',');


            Supplier supplier = db.Suppliers.Where(x => x.Id == id).FirstOrDefault();
            supplier.latitude = decimal.Parse(latitude);
            supplier.longtitude = decimal.Parse(longtitude);

            db.Entry(supplier).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Processed = true;
            return View(db.Suppliers.Where(x => x.Id == id).FirstOrDefault());
        }


        public ActionResult ShowProducts(int id)
        {
            if (Session["Id"] != null)
            {

                if (id == Convert.ToInt32(Session["Id"]) && Session["Role"].ToString() == "Supplier" || Session["Role"].ToString() == "Admin")
                {
                    int supId = db.Suppliers.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
                    return View(db.Products.Where(x => x.SupplierId == supId).ToList());
                }
                else
                {
                    TempData["Message"] = "Bu alana giriş yetkiniz yoktur.";
                    return RedirectToAction("ErrorPage", "Error");
                }

            }
            else
            {
                TempData["Message"] = "Kullanıcı kimliğiniz yanlış. Tekrar giriş yapmayı deneyin.";
                return RedirectToAction("ErrorPage", "Error");
            }
        }

    }
}
