using MvcCF5.Context;
using MvcCF5.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using Image = MvcCF5.Models.Image;
using System.Web.Helpers;

namespace MvcCF5.Controllers
{
    public class ProductController : Controller
    {
        BorrowContext db = new BorrowContext();
        // GET: Product 
        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {

                    List<Product> product = db.Products.ToList();
                    var supp = new List<string>();
                    var cat = new List<string>();

                    foreach (var item in product)
                    {
                        cat.Add(db.Categories.Where(x => x.Id == item.CategoryId).Select(x => x.Name).FirstOrDefault());
                        supp.Add(db.Suppliers.Where(x => x.Id == item.SupplierId).Select(x => x.Name).FirstOrDefault());
                    }
                    ViewBag.catName = cat;
                    ViewBag.supName = supp;

                    return View(product);
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

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            if (Session["Id"] != null)
            {
                List<Supplier> slist = db.Suppliers.Where(x => x.Status == true).ToList();
                ViewBag.supp = new SelectList(slist, "Id", "Name");
                List<Category> clist = db.Categories.Where(x => x.Status == true).ToList();
                ViewBag.cat = new SelectList(clist, "Id", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    db.Products.Add(product);
                }
                else
                {
                    int convertKey = Convert.ToInt32(Session["Id"]);
                    int supId = db.Suppliers.Where(x => x.UserId == convertKey).Select(x => x.Id).FirstOrDefault();
                    product.SupplierId = supId;
                }

                db.Products.Add(product);
                db.SaveChanges();

                if (Session["Role"].ToString() == "Admin")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Processed = true;
                    return View();
                }

            }
            else
            {

                return RedirectToAction("Login", "Home");
            }

        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Id"] != null)
            {
                List<Supplier> slist = db.Suppliers.Where(x => x.Status == true).ToList();
                ViewBag.supp = new SelectList(slist, "Id", "Name");
                List<Category> clist = db.Categories.Where(x => x.Status == true).ToList();
                ViewBag.cat = new SelectList(clist, "Id", "Name");

                return View(db.Products.Where(x => x.Id == id).FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Product product)
        {
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            if (Session["Role"].ToString() == "Admin")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Processed = true;
                return View(db.Products.Where(x => x.Id == id).FirstOrDefault());
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {

            bool result = true;
            Product product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            if (Convert.ToInt32(Session["Id"]) == product.SupplierId || Session["Role"].ToString() == "Admin")
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImageUpload(int id)
        {
            return View(db.Products.Where(x => x.Id == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult ImageUpload(int id, HttpPostedFileBase uploadfile)
        {

            int a = db.Products.Where(x => x.Id == id).Select(x => x.SupplierId).FirstOrDefault();
            int b = db.Suppliers.Where(x => x.Id == a).Select(x => x.UserId).FirstOrDefault();

            if (uploadfile != null && uploadfile.ContentLength > 0 && Convert.ToInt32(Session["Id"]) == b || Session["Role"].ToString() == "Admin")
            {
                string guiandname = (Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadfile.FileName));
                string filePath = Path.Combine(Server.MapPath("~/Content/images"), guiandname);
                uploadfile.SaveAs(filePath); //dosyayı klasöre kaydediyor
                Product product = db.Products.Where(x => x.Id == id).FirstOrDefault();
                product.Image = guiandname;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                TempData["Message"] = "Yapma bunu :(";
                return View(db.Products.Where(x => x.Id == id).FirstOrDefault());
            }

            return View(db.Products.Where(x => x.Id == id).FirstOrDefault());

        }

        public ActionResult ImageDelete(int id)
        {

            Product product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            product.Image = null;
            db.SaveChanges();

            return RedirectToAction("ImageUpload", new { id });
        }


        public ActionResult SliderImageUpload(int id)
        {
            ViewBag.prodId = id;
            return View(db.Images.Where(x => x.ProductImg == id).ToList());
        }

        [HttpPost]
        public ActionResult SliderImageUpload(int id, HttpPostedFileBase uploadfile)
        {
            if (uploadfile != null)
            {

                string guiandname = (Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadfile.FileName));
                string filePath = Path.Combine(Server.MapPath("~/Content/images"), guiandname);
                WebImage img = new WebImage(uploadfile.InputStream);
                if (img.Width > 1000)
                    img.Resize(800, 600);
                img.Save(filePath); //dosyayı klasöre kaydediyor

                //uploadfile.SaveAs(filePath); //dosyayı klasöre kaydediyor

                Image image = new Image();


                int convertKey = Convert.ToInt32(Session["Id"]);
                int supId = db.Products.Where(x => x.Id == id).Select(x => x.SupplierId).FirstOrDefault();
                int a = db.Suppliers.Where(x => x.Id == supId).Select(x => x.UserId).FirstOrDefault();
                if (convertKey == a || Session["Role"].ToString() == "Admin")
                {
                    image.Img = guiandname;
                    image.ProductImg = id;
                    db.Images.Add(image);
                    db.SaveChanges();
                }
                ViewBag.prodId = id;
                return View(db.Images.Where(x => x.ProductImg == id).ToList());
            }
            else
            {
                ModelState.AddModelError("", "Lütfen geçerli bir dosya seçiniz.");
                return RedirectToAction("SliderImageUpload", "Product", new { id });
            }

        }

        public ActionResult SliderImageDelete(int id)
        {

            Image image = db.Images.Where(x => x.Id == id).FirstOrDefault();
            id = Convert.ToInt32(image.ProductImg);
            db.Images.Remove(image);
            db.SaveChanges();


            return RedirectToAction("SliderImageUpload", "Product", new { id });
        }


        public void ExportToExcel()
        {

            List<Product> emplist = db.Products.ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Rapor");

            ws.Cells["A3"].Value = "Id";
            ws.Cells["B3"].Value = "Ürün ismi";
            ws.Cells["C3"].Value = "Tipi";
            ws.Cells["D3"].Value = "Tedarikçi";
            ws.Cells["E3"].Value = "Kategorisi";
            ws.Cells["F3"].Value = "Adet";
            ws.Cells["G3"].Value = "Alış F.";
            ws.Cells["H3"].Value = "Satış F.";
            ws.Cells["I3"].Value = "Porsiyon(g)";
            ws.Cells["J3"].Value = "Detaylar";
            ws.Cells["K3"].Value = "Durum";
            ws.Cells["L3"].Value = "Tarih";
            ws.Cells["O5"].Value = string.Format("Bu belge {0:dd MMMM yyyy} saat {0:H: mm tt} 'de oluşturulmuştur.", DateTimeOffset.Now);

            int rowS = 4;

            foreach (var item in emplist)
            {

                ws.Row(rowS).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Row(rowS).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("FloralWhite")));
                ws.Cells[string.Format("A{0}", rowS)].Value = item.Id;
                ws.Cells[string.Format("B{0}", rowS)].Value = item.Name;
                ws.Cells[string.Format("D{0}", rowS)].Value = db.Suppliers.Where(x => x.Id == item.SupplierId).Select(x => x.Name).FirstOrDefault();
                ws.Cells[string.Format("E{0}", rowS)].Value = db.Categories.Where(x => x.Id == item.CategoryId).Select(x => x.Name).FirstOrDefault();
                ws.Cells[string.Format("F{0}", rowS)].Value = item.Amount;
                ws.Cells[string.Format("G{0}", rowS)].Value = item.Price;
                ws.Cells[string.Format("H{0}", rowS)].Value = item.SellPrice;
                ws.Cells[string.Format("I{0}", rowS)].Value = item.Weight;
                ws.Cells[string.Format("J{0}", rowS)].Value = item.Detail;
                ws.Cells[string.Format("K{0}", rowS)].Value = item.Status;
                if (item.Date != null)
                    ws.Cells[string.Format("L{0}", rowS)].Value = item.Date.Value.ToString();

                rowS++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment:filename= " + " Urunler.xlsx ");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

        [HttpPost]
        public ActionResult ProductComment(int id, FormCollection form)
        {
            if (Session["Id"] != null)
            {

                Comment comment = new Comment();
                comment.UserId = Convert.ToInt32(Session["Id"]);
                comment.Rate = Convert.ToInt32(form["rate"]);
                comment.ProductComment = form["userComment"];
                comment.ProductId = id;
                Product prod = db.Products.Where(x => x.Id == id).FirstOrDefault();
                comment.SupplierId = prod.SupplierId;
                comment.ProductName = prod.Name;

                db.Comments.Add(comment);
                db.SaveChanges();

                TempData["Message"] = "Yorumunuz onaylama işleminden sonra görüntüleneceketir.";

                return RedirectToAction("ProductDetail", "Home", new { id });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult ShowComments()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                    return View(db.Comments.ToList());
                else
                {
                    TempData["Message"] = "Kullanıcı kimliğiniz yanlış. Tekrar giriş yapmayı deneyin.";
                    return RedirectToAction("ErrorPage", "Error");
                }
            }
            else
            {
                TempData["Message"] = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult ConfirmComment(int id)
        {

            bool result = true;
            Comment comment = db.Comments.Where(x => x.Id == id).FirstOrDefault();
            comment.Status = true;
            db.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            if (Session["Id"] != null && Session["Role"].ToString() == "Admin")
            {
                bool result = true;
                Comment comment = db.Comments.Where(x => x.Id == id).FirstOrDefault();
                db.Comments.Remove(comment);
                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                bool result = false;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
