using MvcCF5.Context;
using MvcCF5.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Helpers;
using System.Net;
using System.Net.Mail;

namespace MvcCF5.Controllers
{
    public class HomeController : Controller
    {

        BorrowContext db = new BorrowContext();

        public ActionResult Index()
        {

            List<Product> plist = db.Products.Take(20).Where(x => x.Status == true && x.Image != null).ToList();
            ViewBag.prod = plist;
            ViewBag.t = db.Settings.Select(x => x.Title).First();
            ViewBag.d = db.Settings.Select(x => x.Description).First();
            ViewBag.k = db.Settings.Select(x => x.Keywords).First();
            List<string> clist = db.Categories.Where(x => x.Status == true).Select(x => x.Name).ToList();
            ViewBag.cat = clist;

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                List<UserAccount> list = db.UserAccounts.ToList();

                foreach (var item in list)
                {
                    if (item.Name == account.Name)
                    {
                        ModelState.AddModelError("", "İsim kullanılmaktadır");
                        return View();
                    }
                    else if (item.Email == account.Email)
                    {
                        ModelState.AddModelError("", "E-posta kullanılmaktadır");
                        return View();
                    }
                }
                account.Latitude = Convert.ToDecimal(39.928953);
                account.Longtitude = Convert.ToDecimal(32.854815);
                // account.Status = false;          status false olsa bile ödeme ekranında bilgi güncellemesi olabilir.
                db.UserAccounts.Add(account);
                db.SaveChanges();
                TempData["Message"] = account.Name + " başarıyla kayıt oldunuz. Giriş yapabilirsiniz.";

                //mail
                string to = account.Email;
                string from = "mfurkansasmaz@gmail.com";
                string password = "Sakarya.54";
                string subject = "Kaydınız Başarılı.";
                string body = "Son bir kaç işlemden sonra sipariş verebilirsiniz." +
                    "Ama bundan önce lütfen üye bilgileri sayfasından kişisel bilgilerinizi güncelleyiniz." +
                    "Size daha iyi hizmet verebilmemiz için lütfen konum ayarlarını açınız." +
                    "Yemek haritam ailesi olarak teşekkür ederiz. ";
                MailGonder(to, from, password, subject, body);
                //


                if (account.Role == "Supplier")
                {
                    UserAccount userAccount = db.UserAccounts.Where(x => x.Name == account.Name).FirstOrDefault();
                    Supplier supp = new Supplier();

                    supp.Name = userAccount.Name;
                    supp.UserId = userAccount.Id;
                    supp.Status = false;
                    supp.longtitude = Convert.ToDecimal(32.854815);
                    supp.latitude = Convert.ToDecimal(39.928953);
                    supp.distance = 0;
                    db.Suppliers.Add(supp);
                    db.SaveChanges();

                }
                UserAccount usr = db.UserAccounts.Where(x => x.Name == account.Name).FirstOrDefault();
                Image img = new Image();
                img.Img = null;
                img.UserImg = usr.Id;
                db.Images.Add(img);
                db.SaveChanges();

                return RedirectToAction("Login");
            }
            ModelState.Clear();

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserAccount user)
        {
            var usr = db.UserAccounts.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (usr != null)
            {
                Session["Id"] = usr.Id.ToString();
                Session["Name"] = usr.Name.ToString();
                Session["Role"] = usr.Role.ToString();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "E-posta veya şifre yanlış");
            }
            return View();

        }


        public ActionResult LoggedIn()
        {
            if (Session["Id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }


        public ActionResult ForgetPassword()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string email)
        {
            if (email != "")
            {
                UserAccount user = db.UserAccounts.Where(x => x.Email == email).FirstOrDefault();
                if (user != null)
                {
                    string to = user.Email;
                    string from = "mfurkansasmaz@gmail.com";
                    string password = "Sakarya.54";
                    string subject = "Şifre Hatırlatma";
                    string body = "Merhaba " + user.Name +
                        ". Kullanımda olan şifreniz : " +
                        user.Password;
                    MailGonder(to, from, password, subject, body);
                    TempData["Message"] = "Şifreniz e-posta adresine gönderilmiştir";
                    RedirectToAction("ForgetPassword");
                }
                else
                {
                    TempData["Message"] = "Böyle bir e-posta adresi yok";
                    RedirectToAction("ForgetPassword");
                }

            }
            else
            {
                TempData["Message"] = "Lütfen e-posta adresi girin";
                RedirectToAction("ForgetPassword");
            }
            return View();
        }


        public ActionResult UserList()
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {

                    //List<Image> imlist = db.Images.ToList();
                    //ViewBag.imlist = imlist;
                    //ViewBag.k = imlist.Count;
                    return View(db.UserAccounts.ToList());
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


        public ActionResult Edit(int id)
        {
            return View(db.UserAccounts.Where(x => x.Id == id).FirstOrDefault());
        }


        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {

            bool result = true;
            UserAccount userAccount = db.UserAccounts.Where(x => x.Id == id).FirstOrDefault();
            db.UserAccounts.Remove(userAccount);
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            return View(db.UserAccounts.Where(x => x.Id == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {

            UserAccount userAccount = db.UserAccounts.Where(x => x.Id == id).FirstOrDefault();
            db.UserAccounts.Remove(userAccount);
            db.SaveChanges();

            return RedirectToAction("UserList");
        }

        public ActionResult AboutUs()
        {
            Setting setting = db.Settings.FirstOrDefault();
            ViewBag.aboutus = setting.AboutUs;
            return View();
        }

        public ActionResult Referances()
        {
            Setting setting = db.Settings.FirstOrDefault();
            ViewBag.referances = setting.Referances;
            return View();
        }

        public ActionResult Contact()
        {
            Setting setting = db.Settings.FirstOrDefault();
            ViewBag.contact = setting.Contact;
            return View();
        }

        public JsonResult GetSuppliers()
        {
            // markerlar 
            //string markerImageUrl = "samedefe.com/examples/custom_marker.png";

            var model = db.Suppliers.ToList();

            List<dynamic> json = new List<dynamic>();
            foreach (var item in model)
            {
                if (item.Status == true)
                {
                    dynamic mark = new JObject();
                    mark.id = item.Id;
                    mark.name = item.Name;
                    mark.lat = item.latitude.ToString();
                    mark.lng = item.longtitude.ToString();
                    mark.img = item.Image;
                    mark.det = item.Detail;
                    json.Add(mark);
                }
            }
            return Json(
                 JsonConvert.SerializeObject(json), JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult SupplierDetail(int id)
        {
            Supplier supplier = db.Suppliers.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.supp = supplier;

            List<Product> plist = db.Products.Where(x => x.Status == true && x.SupplierId == id).ToList();
            ViewBag.prod = plist;

            return View();
        }

        [HttpPost]
        public JsonResult GetLocation(String lati, String lngi)
        {

            lati = lati.Replace('.', ',');
            lngi = lngi.Replace('.', ',');
            double x = double.Parse(lati);
            double y = double.Parse(lngi);
            var slist = db.Suppliers.ToList();
            List<dynamic> tson = new List<dynamic>();

            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);
                UserAccount usr = db.UserAccounts.Where(m => m.Id == convertKey).FirstOrDefault();

                if (usr.Latitude == Convert.ToDecimal(39.928953) && usr.Longtitude == Convert.ToDecimal(32.854815))
                {
                    usr.Latitude = decimal.Parse(lati);
                    usr.Longtitude = decimal.Parse(lngi);
                    db.SaveChanges();
                }
            }

            foreach (var item in slist)
            {
                double rlat1 = Math.PI * x / 180;
                double rlat2 = Math.PI * decimal.ToDouble(item.latitude) / 180;
                double theta = y - decimal.ToDouble(item.longtitude);
                double rtheta = Math.PI * theta / 180;
                double dist =
                    Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                    Math.Cos(rlat2) * Math.Cos(rtheta);
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515 * 1.609344; //km cinsinden

                if (item.distance >= dist && item.Image != null)
                {
                    dynamic mark = new JObject();
                    mark.id = item.Id;
                    mark.name = item.Name;
                    mark.img = item.Image;
                    mark.det = item.Detail;
                    tson.Add(mark);
                }

            }
            return Json(
                JsonConvert.SerializeObject(tson), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Ara(string search, string option)
        {
            List<string> clist = db.Categories.Where(x => x.Status == true).Select(x => x.Name).ToList();
            ViewBag.cat = clist;
            if (option == "Restoran")
            {
                if (search == "All")
                {
                    List<Supplier> slist = db.Suppliers.ToList();
                    ViewBag.supp = slist;
                    return View();
                }
                else
                {
                    List<Supplier> slist = db.Suppliers.Where(x => x.Name.Contains(search)).ToList();
                    if (slist.Count == 0)
                    {
                        ViewBag.message = "Üzgünüz. Sonuç bulunamadı :(";
                        return View();
                    }
                    ViewBag.supp = slist;
                    return View();
                }
            }

            else if (option == "Ürün")
            {
                if (search == "All")
                {
                    List<Product> plist = db.Products.ToList();
                    ViewBag.prod = plist;
                    return View();
                }
                else
                {
                    List<Product> plist = db.Products.Where(x => x.Name.Contains(search)).ToList();
                    if (plist.Count == 0)
                    {
                        ViewBag.message = "Üzgünüz. Sonuç bulunamadı :(";
                        return View();
                    }
                    ViewBag.prod = plist;
                    return View();
                }
            }

            else if (option == "Kategori")
            {
                List<int> idList = db.Categories.Where(x => x.Name.Contains(search)).Select(x => x.Id).ToList();
                var prod = new List<Product>();
              
                foreach (var item in idList)
                {
                    prod.AddRange(db.Products.Where(x => x.CategoryId == item).ToList());
                }
                if (prod.Count == 0)
                {
                    ViewBag.prod = null;
                    ViewBag.message = "Üzgünüz. Sonuç bulunamadı :(";
                    return View();
                }
                else
                {
                    ViewBag.prod = prod;
                    return View();
                }

            }
            else
            {
                ViewBag.message = "Lütfen kategori seçiniz.";
                return View();
            }
        }

        public ActionResult CustomerInform()
        {

            if (Session["Id"] != null)
            {

                int convertKey = Convert.ToInt32(Session["Id"]);
                ViewBag.ImgPath = db.Images.Where(x => x.UserImg == convertKey).Select(x => x.Img).FirstOrDefault();
                ViewBag.suppDetail = db.Suppliers.Where(x => x.UserId == convertKey).Select(x => x.Detail).FirstOrDefault();
                return View(db.UserAccounts.Where(x => x.Id == convertKey).FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        [HttpPost]
        public ActionResult CustomerInform(int id, UserAccount userAccount, string detail)
        {

            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);

                if (userAccount.Id == convertKey)
                {
                    if (userAccount.Role == "Supplier")
                    {

                        Supplier supp = db.Suppliers.Where(x => x.UserId == id).FirstOrDefault();
                        supp.Name = userAccount.Name;

                        supp.longtitude = Convert.ToDecimal(userAccount.Longtitude);
                        supp.latitude = Convert.ToDecimal(userAccount.Latitude);
                        supp.distance = Convert.ToDouble(userAccount.Distance);
                        supp.Address = userAccount.Address;
                        supp.Detail = detail; //kontol et
                        supp.Tel = userAccount.Tel;
                        supp.Date = DateTime.Now;
                        if (userAccount.Longtitude != null && userAccount.Latitude != null && userAccount.Address != null && userAccount.Tel != null && userAccount.Distance != null)
                        {
                            supp.Status = true;
                        }
                        db.SaveChanges();
                    }

                    if (userAccount.Longtitude != null && userAccount.Latitude != null && userAccount.Address != null && userAccount.Tel != null)
                    {
                        userAccount.Status = true;
                    }
                    db.Entry(userAccount).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Processed = true;

                    return RedirectToAction("CustomerInform");
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


        public ActionResult UserImgUpload(int id)
        {
            return View(db.Images.Where(x => x.UserImg == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult UserImgUpload(int id, HttpPostedFileBase uploadfile)
        {
            if (uploadfile.ContentLength > 0)
            {

                string guiandname = (Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadfile.FileName));
                string filePath = Path.Combine(Server.MapPath("~/Content/images"), guiandname);
                WebImage img = new WebImage(uploadfile.InputStream);
                if (img.Width > 1000)
                    img.Resize(800, 600);
                img.Save(filePath); //kaydet
                Image image = db.Images.Where(x => x.UserImg == id).FirstOrDefault();

                UserAccount user = db.UserAccounts.Where(x => x.Id == id).FirstOrDefault();
                if (user.Role == "Supplier")
                {
                    Supplier supplier = db.Suppliers.Where(x => x.UserId == id).FirstOrDefault();
                    supplier.Image = guiandname;
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                }

                image.Img = guiandname;
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("UserImgUpload", new { id });

        }


        public ActionResult UserImgDelete(int id)
        {
            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);

                if (Session["Role"].ToString() == "Supplier")
                {
                    Supplier sup = db.Suppliers.Where(x => x.UserId == convertKey).FirstOrDefault();
                    sup.Image = null;
                }

                Image image = db.Images.Where(x => x.Id == id).FirstOrDefault();
                id = image.Id;
                image.Img = null;
                db.SaveChanges();

                return RedirectToAction("UserImgUpload", new { id = image.UserImg });
            }

            else
            {
                return RedirectToAction("Login", "Home");
            }

        }


        public ActionResult ProductDetail(int id)
        {

            ViewBag.slider = db.Images.Where(x => x.ProductImg == id).Select(x => x.Img).ToList();
            Product product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            Supplier supp = db.Suppliers.Where(x => x.Id == product.SupplierId).FirstOrDefault();
            ViewBag.supp = supp;

            ViewBag.comments = db.Comments.Where(x => x.ProductId == id && x.Status == true).ToList();

            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);


                if (db.OrderDetails.Where(x => x.ProductId == id && x.UserId == convertKey && x.Status == true).FirstOrDefault() != null)
                {
                    ViewBag.canComment = true;
                }

                UserAccount usr = db.UserAccounts.Where(m => m.Id == convertKey).FirstOrDefault();
                double rlat1 = Math.PI * Convert.ToDouble(usr.Latitude) / 180;
                double rlat2 = Math.PI * Convert.ToDouble(supp.latitude) / 180;
                double theta = Convert.ToDouble(usr.Longtitude - supp.longtitude);
                double rtheta = Math.PI * theta / 180;
                double dist =
                    Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                    Math.Cos(rlat2) * Math.Cos(rtheta);
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515 * 1.609344; //km cinsinden
                ViewBag.distance = dist;
            }

            return View(product);
        }


        private void MailGonder(string to, string from, string password, string subject, string body)
        {
            using (MailMessage mm = new MailMessage(from, to))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(from, password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                try
                {
                    smtp.Send(mm);
                }
                catch
                {

                }

            }
        }

        public ActionResult Categories(string select)
        {
            return RedirectToAction("Ara", "Home", new { search = select, option = "Kategori" });

        }


        public ActionResult ShowCustomerInform(int id)
        {
            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);
                if (id == convertKey)
                {
                    ViewBag.suppDetail = db.Suppliers.Where(x => x.UserId == convertKey).Select(x => x.Detail).FirstOrDefault();
                    ViewBag.Img = db.Images.Where(x => x.UserImg == convertKey).Select(x => x.Img).FirstOrDefault();
                    return View(db.UserAccounts.Where(x => x.Id == id).FirstOrDefault());
                }
                else
                {
                    return RedirectToAction("NotAuthorized", "Error");
                }
            }
            else
            {
                TempData["Message"] = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Home");
            }
        }


    }
}
