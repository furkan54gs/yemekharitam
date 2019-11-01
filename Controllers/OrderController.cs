using MvcCF5.Context;
using MvcCF5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;

namespace MvcCF5.Models
{
    public class OrderController : Controller
    {

        BorrowContext db = new BorrowContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ShopCart()
        {
            if (Session["Id"] != null)
            {
                //List<int> cList = db.ShopCarts.Where(x => x.UserId == Convert.ToInt32(Session["Id"])).Select(x => x.Product.Id).ToList();
                //List<Product> pList = db.Products.ToList();
                //List<Product> addList = new List<Product>();

                int convertKey = Convert.ToInt32(Session["Id"]);
                return View(db.ShopCarts.Where(x => x.UserId == convertKey).ToList());
            }
            else
            {
                TempData["Message"] = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult AddShopCart(int id, int quantity)
        {
            if (Session["Id"] != null)
            {
                if (Session["Role"].ToString() != "Supplier")
                {
                    ShopCart shopCart = new ShopCart();

                    shopCart.Product = db.Products.Where(x => x.Id == id).FirstOrDefault();

                    shopCart.Product.Id = id;
                    int convertKey = Convert.ToInt32(Session["Id"]);
                    shopCart.UserId = convertKey;
                    shopCart.SupplierId = db.Products.Where(x => x.Id == id).Select(x => x.SupplierId).FirstOrDefault();
                    int supId = db.Suppliers.Where(x => x.UserId == convertKey).Select(x => x.Id).FirstOrDefault();

                    shopCart.Quantity = quantity;
                    db.ShopCarts.Add(shopCart);
                    db.SaveChanges();

                    return RedirectToAction("ShopCart", "Order");
                }

                else
                {
                    TempData["Message"] = "Firmalar sipariş işlemi gerçekleştiremez.";
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
        public ActionResult ShopCartDelete(int id)
        {
            bool result = true;
            ShopCart shop = db.ShopCarts.Where(x => x.Id == id).FirstOrDefault();
            db.ShopCarts.Remove(shop);
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConfirmOrder()
        {
            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);

                List<float> fltList = (from item in db.ShopCarts
                                       where item.UserId == convertKey
                                       select (item.Quantity * item.Product.SellPrice)).ToList();

                float total = 0;
                foreach (var item in fltList)
                {
                    total += item;
                }

                ViewBag.total = total;

                return View(db.UserAccounts.Where(x => x.Id == convertKey).FirstOrDefault());
            }
            else
            {
                TempData["Message"] = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult ConfirmOrder(UserAccount userAccount, string note)
        {
            if (Session["Id"] != null)
            {
                int convertKey = Convert.ToInt32(Session["Id"]);

                if (convertKey == userAccount.Id)
                {

                    List<ShopCart> shop = db.ShopCarts.Where(x => x.UserId == convertKey).ToList();
                    int a = shop[0].SupplierId;
                    int first = 0;


                    foreach (var item in shop)
                    {
                        if (item.SupplierId != a || first == 0)
                        {
                            Order order = new Order();
                            order.UserId = convertKey;
                            order.SuppId = item.Product.SupplierId;
                            string sName = db.Suppliers.Where(x => x.Id == item.Product.SupplierId).Select(x => x.Name).FirstOrDefault();
                            order.SuppName = sName;
                            order.Name = userAccount.Name;
                            order.Phone = userAccount.Tel;
                            order.Address = userAccount.Address;
                            order.Note = note;
                            order.Latitude = Convert.ToDecimal(userAccount.Latitude);
                            order.Longtitude = Convert.ToDecimal(userAccount.Longtitude);
                            order.ShipInfo = "İşleme Alındı";
                            order.Amount = item.Quantity * item.Product.SellPrice;
                            order.Date = DateTime.Now;
                            if (userAccount.Name != null && userAccount.Tel != null && userAccount.Address != null)
                            {
                                order.Status = true;
                            }
                            else
                            {
                                ModelState.AddModelError("", "İsim,telefon ve adres kısımları boş bırakılamaz");
                                return View(db.UserAccounts.Where(x => x.Id == convertKey).FirstOrDefault());
                            }
                            db.Orders.Add(order);
                            db.SaveChanges();
                            first += 1;
                        }
                        else
                        {
                            Order orderElse = db.Orders.Where(x => x.UserId == convertKey).OrderBy(x => x.Id).ToList().Last();
                            orderElse.Amount = orderElse.Amount + item.Quantity * item.Product.SellPrice;
                            db.SaveChanges();
                        }

                        int oId = db.Orders.Max(i => i.Id);
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.UserId = item.UserId;
                        orderDetail.OrderId = oId;
                        orderDetail.ProductId = item.Product.Id;
                        orderDetail.SuppId = item.Product.SupplierId;
                        orderDetail.Name = item.Product.Name;
                        orderDetail.Quantity = item.Quantity;
                        orderDetail.SellPrice = item.Product.SellPrice;
                        orderDetail.Amount = item.Quantity * item.Product.SellPrice;
                        orderDetail.Status = true;
                        db.OrderDetails.Add(orderDetail);

                        item.Product.Amount = item.Product.Amount - item.Quantity;
                        db.ShopCarts.Remove(item);
                    }

                    db.SaveChanges();

                    TempData["Message"] = "Siparişiniz işleme alınmıştır.";

                    return RedirectToAction("ShowOrders", "Order", new { id = convertKey });
                }
                else
                {
                    TempData["Message"] = "Lütfen giriş yapın.";
                    return RedirectToAction("Login", "Home");
                }

            }
            else
            {
                TempData["Message"] = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Home");
            }


        }

        public ActionResult ShowOrders(int id)
        {
            if (Session["Id"] != null)
            {

                int convertKey = Convert.ToInt32(Session["Id"]);

                if (convertKey == id)
                {


                    if (Session["Role"].ToString() == "Supplier")
                    {
                        int supId = db.Suppliers.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
                        return View(db.Orders.Where(x => x.SuppId == supId).ToList());
                    }
                    else
                    {
                        return View(db.Orders.Where(x => x.UserId == id).ToList());
                    }
                }

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
        public ActionResult OrderDelete(int id)
        {
            bool result = true;
            Order order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            order.Status = false;
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderDetail(int id)
        {

            int control;

            if (Session["Role"].ToString() == "Supplier")
            {
                int supId = db.Orders.Where(x => x.Id == id).Select(x => x.SuppId).FirstOrDefault();
                control = db.Suppliers.Where(x => x.Id == supId).Select(x => x.UserId).FirstOrDefault();
            }
            else
            {
                control = db.Orders.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
            }

            if (Session["Id"] != null)
            {

                int convertKey = Convert.ToInt32(Session["Id"]);


                if (convertKey == control)
                {
                    ViewBag.order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
                    ViewBag.orderDetail = db.OrderDetails.Where(x => x.OrderId == id).ToList();
                    return View();
                }
                else if (Session["Role"].ToString() == "Admin")
                {
                    ViewBag.order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
                    ViewBag.orderDetail = db.OrderDetails.Where(x => x.OrderId == id).ToList();
                    return View();
                }
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
        public ActionResult OrderDetailDelete(int id)
        {
            bool result = true;
            OrderDetail orderDetail = db.OrderDetails.Where(x => x.Id == id).FirstOrDefault();
            orderDetail.Status = false;
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateShipInfo(int id, string value)
        {
            bool result = false;
            Order order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            if (value == "Tamamlandı" || value == "İşleme Alındı" || value == "Hazırlanıyor" || value == "Yolda" || value == "Onaylanmadı")
            {
                result = true;
                order.ShipInfo = value;
            }

            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OrderList(string status)
        {
            if (Session["Id"] != null && Session["Role"].ToString() == "Admin")
            {
                if (status == "All")
                {
                    return View(db.Orders.ToList());
                }
                else
                {
                    return View(db.Orders.Where(x => x.ShipInfo == status).ToList());
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult OrderEdit(int id)
        {
            if (Session["Id"] != null && Session["Role"].ToString() == "Admin")
            {

                return View(db.Orders.Where(x => x.Id == id).FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult OrderEdit(int id, Order order)
        {
            if (Session["Id"] != null && Session["Role"].ToString() == "Admin")
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OrderList", "Order", new { status = "All" });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public void OrdersExcel()
        {

            List<Order> emplist = db.Orders.Where(x => x.ShipInfo == "Tamamlandı").ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Rapor");

            ws.Cells["A3"].Value = "Sipariş Id";
            ws.Cells["B3"].Value = "Müşteri Id";
            ws.Cells["C3"].Value = "Müşteri Adı";
            ws.Cells["D3"].Value = "Enlem";
            ws.Cells["E3"].Value = "Boylam";
            ws.Cells["F3"].Value = "Telefon";
            ws.Cells["G3"].Value = "Tarih";
            ws.Cells["H3"].Value = "Toplam";
            ws.Cells["I3"].Value = "Tedarikçi Id";
            ws.Cells["J3"].Value = "Tedarikçi Adı";
            ws.Cells["K3"].Value = "Adres";
            ws.Cells["L3"].Value = "Not";
            ws.Cells["O5"].Value = string.Format("Bu belge {0:dd MMMM yyyy} saat {0:H: mm tt} 'de oluşturulmuştur.", DateTimeOffset.Now);

            int rowS = 4;

            foreach (var item in emplist)
            {

                ws.Row(rowS).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Row(rowS).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("FloralWhite")));
                ws.Cells[string.Format("A{0}", rowS)].Value = item.Id;
                ws.Cells[string.Format("B{0}", rowS)].Value = item.UserId;
                ws.Cells[string.Format("C{0}", rowS)].Value = item.Name;
                ws.Cells[string.Format("D{0}", rowS)].Value = item.Latitude;
                ws.Cells[string.Format("E{0}", rowS)].Value = item.Longtitude;
                ws.Cells[string.Format("F{0}", rowS)].Value = item.Phone;
                if (item.Date != null)
                    ws.Cells[string.Format("G{0}", rowS)].Value = item.Date.Value.ToString();
                ws.Cells[string.Format("H{0}", rowS)].Value = item.Amount;
                ws.Cells[string.Format("I{0}", rowS)].Value = item.SuppId;
                ws.Cells[string.Format("J{0}", rowS)].Value = item.SuppName;
                ws.Cells[string.Format("K{0}", rowS)].Value = item.Address;
                ws.Cells[string.Format("L{0}", rowS)].Value = item.Note;
                rowS++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment:filename= " + " Siparişler.xlsx ");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }


    }
}