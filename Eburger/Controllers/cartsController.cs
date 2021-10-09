using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Eburger.Models;
using Microsoft.AspNet.Identity;
using Eburger.ViewModel;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace Eburger.Controllers
{
    public class cartsController : Controller
    {
        private E_burgerEntities db = new E_burgerEntities();

        // GET: carts
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();

            var carts = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(x => x.userID == username).Where(y => y.cartStatus == false).Where(o => o.orderStatus == false);
            return View(carts.ToList());
        }

        //admin dashboard
        [Authorize(Roles = "Admin")]
        public ActionResult Admindashboard()
        {
            var today = DateTime.Now;
            var lastThree= DateTime.Now.AddMonths(-1);
            var lastyear = DateTime.Now.AddMonths(-12);

            var dashboard = new homeview
            {
                
                ord = db.orders.ToList(),
                ordlastthree=db.orders.Where(d => d.orderDate >= lastThree).ToList(),
                ordYear = db.orders.Where(d => d.orderDate >= lastyear).ToList()

            };
            return View(dashboard);
        }

        public ActionResult Income()
        {
            var lastThree = DateTime.Now.AddMonths(-1);

            List<order> Income = new List<order>();
            Income = db.orders.Include(c => c.AspNetUser).Where(d => d.orderDate >= lastThree).ToList();


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Income.rpt"));

            rd.SetDataSource(Income);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Income Report.pdf");
        }

        public ActionResult BurgerStatistic()
        {

            List<cart> Income = new List<cart>();
            Income = db.carts.ToList();


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "saleStatistic.rpt"));

            rd.SetDataSource(Income);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Burger statistic Report.pdf");
        }

        public ActionResult BurgerReport()
        {

            List<tbl_burger> Income = new List<tbl_burger>();
            Income = db.tbl_burger.ToList();


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "BurgerReport.rpt"));

            rd.SetDataSource(Income);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Burgers Report.pdf");
        }

        public ActionResult SalesReport()
        {

            List<order> Income = new List<order>();
            Income = db.orders.ToList();


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "SalesChart.rpt"));

            rd.SetDataSource(Income);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Burgers Report.pdf");
        }

        //check orders
        [Authorize(Roles = "Admin")]
        public ActionResult Checkorder()
        {

            var OrderItems = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(y => y.cartStatus == true).Where(o => o.orderStatus == false).ToList();
            //var OrderItems=from crt from cart

            return View(OrderItems);
        }

        //order view
        [Authorize(Roles = "Admin")]
        public ActionResult Orderview(string username)
        {
            var OrderItems = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(y => y.cartStatus == true).Where(o => o.orderStatus == false).Where(n => n.userID.ToString() == username).ToList();
            return View(OrderItems);
        }


        //order confirm
        [Authorize(Roles = "Admin")]
        public ActionResult Orderconfirm(string username, int amountof, order ord)
        {
            
            ord.userID = username;
            ord.totalAmount = amountof;
            ord.orderDate = DateTime.Now;
            db.orders.Add(ord);

            db.SaveChanges();

            int neworderid = ord.orderID;

            var ValidCustomers = db.carts.Where(c => c.cartStatus == true).Where(d => d.orderStatus == false).Where(m => m.userID == username).ToList();
            ValidCustomers.ForEach(c => c.orderNo = neworderid);

            (from p in db.carts
             where p.userID == username
             select p).ToList().ForEach(x => x.orderStatus = true);

            //(from p in db.carts
            // where p.userID == username 
            // select p).ToList().ForEach(x => x.orderNo = neworderid);

            

            db.SaveChanges();

            return RedirectToAction("Checkorder");
        }

        [Authorize(Roles = "Deliverman")]
        public ActionResult Checkdelivery()
        {

            var deleviryorder = db.orders.Include(c => c.AspNetUser).Where(y => y.deliveryStart == false).ToList();
            //var OrderItems=from crt from cart

            return View(deleviryorder);
        }

        [Authorize(Roles = "Deliverman")]
        public ActionResult Ongoing()
        {
            var deleviryorder = db.orders.Include(c => c.AspNetUser).Where(y => y.deliveryStart == true).Where(x => x.deliveryStatus==false).ToList();
            return View(deleviryorder);
        }


        public ActionResult Takedelivery(string Email, int OrderID, string totalAmount,  string fName)
        {
            
            (from p in db.orders
             where p.orderID == OrderID
             select p).ToList().ForEach(x => x.deliveryStart = true);

            db.SaveChanges();

            //send emai after confirmed order
            var senderEmail = new MailAddress("asegroupsams@gmail.com", "E Burger");
            var receiverEmail = new MailAddress(Email, "Receiver");
            var password = "mfourbrothers20";
            string msg = "Dear " + fName +",";
            string sub = "Order Delivery";
            string body = "Your order is ready. Your order number is " + OrderID + ". Your order will be delivered to you within 10-15 minutes. The total value of your order is "+totalAmount+" LKR. Thank you for dealing with us. We are committed to providing you with a reliable service. If you want to get more details, please call us 01113255522. Thank you...!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = msg + "" + body
            })
            {
                smtp.Send(mess);
            }

            return RedirectToAction("Checkdelivery");
        }

        public ActionResult Paymentsuccess(string Email, int OrderID, string totalAmount, string fName, payment pay)
        {

            (from p in db.orders
             where p.orderID == OrderID
             select p).ToList().ForEach(x => x.deliveryStatus = true);

            pay.orderID = OrderID;
            pay.paymentDate = DateTime.Now;
            db.payments.Add(pay);

            db.SaveChanges();

            //send emai after confirmed order
            var senderEmail = new MailAddress("asegroupsams@gmail.com", "E Burger");
            var receiverEmail = new MailAddress(Email, "Receiver");
            var password = "mfourbrothers20";
            string msg = "Dear " + fName + ",";
            string sub = "Order Delivery";
            string body = "We received LKR "+totalAmount+" for your order (Order no."+OrderID+"). Thank you for dealing with us. Cheers...!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = msg + "" + body
            })
            {
                smtp.Send(mess);
            }

            return RedirectToAction("Ongoing");
        }

        // GET: carts/Details/5
        [Authorize(Roles = "Customer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cart cart = db.carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: carts/Create
        
        public ActionResult Create()
        {
            ViewBag.userID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.BurgerID = new SelectList(db.tbl_burger, "BurgerID", "BurgerName");
            return RedirectToAction("Index", "Home");
        }

        // POST: carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cartID,BurgerID,userID,quantity,totamount,cartStatus,orderStatus")] cart cart, string returnUrl)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                cart.userID = username;
                db.carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
                //return RedirectToAction("Index","Home");
            }

            return View();
        }

        //placed orders
        [Authorize(Roles = "Customer")]
        public ActionResult PlacedOrders()
        {
            var username = User.Identity.GetUserId();
            var PlaceOrders = db.orders.Where(o => o.userID == username);
            return View(PlaceOrders);
        }

        //view placed order items
        [Authorize(Roles = "Customer")]
        public ActionResult PlacedOrderView(int? id)
        {
            var username = User.Identity.GetUserId();
            var orderItems = db.carts.Where(o => o.userID == username).Where(c => c.orderNo == id);
            return View(orderItems);
        }

        // GET: carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cart cart = db.carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.userID = new SelectList(db.AspNetUsers, "Id", "Email", cart.userID);
            ViewBag.BurgerID = new SelectList(db.tbl_burger, "BurgerID", "BurgerName", cart.BurgerID);
            return View(cart);
        }

        // POST: carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cartID,BurgerID,userID,quantity,totamount,cartStatus,orderStatus")] int amountof, cart cart)
        {
            var username = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();
            (from p in db.carts
             where p.userID == username
             select p).ToList().ForEach(x => x.cartStatus = true);

            db.SaveChanges();

            //send emai after confirmed order
            var senderEmail = new MailAddress("asegroupsams@gmail.com", "E Burger");
            var receiverEmail = new MailAddress(email, "Receiver");
            var password = "mfourbrothers20";
            string msg = "Dear Valued customer, ";
            string sub = "Order confirmed";
            string body = "Thank you for dealing with us. We received your order. You will be notified by call as soon as the order is ready. The total value of your order is " + amountof + " rupees. If you want to get more details, please call us 01113255522";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = msg + "" + body
            })
            {
                smtp.Send(mess);
            }


            return RedirectToAction("Index");


            //var cartdata = db.carts.Where(x => x.userID == username).FirstOrDefault();

            //if(cartdata != null)
            //{
            //    cartdata.cartStatus = true;
            //    db.Entry(cart).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}


            ViewBag.userID = new SelectList(db.AspNetUsers, "Id", "Email", cart.userID);
            ViewBag.BurgerID = new SelectList(db.tbl_burger, "BurgerID", "BurgerName", cart.BurgerID);
            return View(cart);
        }

        // GET: carts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cart cart = db.carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cart cart = db.carts.Find(id);
            db.carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Cartremove()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
