using Eburger.Models;
using Eburger.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Eburger.Controllers
{

    public class HomeController : Controller
    {
        private E_burgerEntities db = new E_burgerEntities();
        public ActionResult Index()
        {
            int limitCount = 8;
            var username = User.Identity.GetUserId();

            var homedata = new homeview
            {

                burgers = db.tbl_burger.OrderByDescending(x => x.BurgerID).Take(limitCount),
                burgerTypes = db.burger_type.ToList(),
                carts = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(x => x.userID == username).Where(y => y.cartStatus == false).Where(o => o.orderStatus == false)

            };

            //var items = db.tbl_burger.OrderByDescending(x => x.BurgerID);
            return View(homedata);
        }

        public ActionResult Burgerview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbl_burger tbl_Burger = db.tbl_burger.Find(id);


            if (tbl_Burger == null)
            {
                return HttpNotFound();
            }

            return View(tbl_Burger);
        }

        public ActionResult Menuview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var homedata = new homeview
            {
                burgers = db.tbl_burger.Where(x => x.typeID == id).ToList(),
                menu = db.burger_type.Where(y => y.typeID == id).ToList(),
                burgerTypes = db.burger_type.ToList()
            };

            //var items = db.tbl_burger.OrderByDescending(x => x.BurgerID);
            return View(homedata);
        }

        public ActionResult Search(string searchtext)
        {
            var homedata = new homeview
            {
                burgers = db.tbl_burger.Where(x => x.BurgerName.Contains(searchtext) || searchtext == null).ToList(),
                burgerTypes = db.burger_type.ToList()
            };

            return View(homedata);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreateCart([Bind(Include = "cartID,BurgerID,userID,quantity,totamount,cartStatus,orderStatus")] cart cart)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                cart.userID = username;
                db.carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult cartview()
        {
            var username = User.Identity.GetUserId();

            var carts = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(x => x.userID == username).Where(y => y.cartStatus == false).Where(o => o.orderStatus == false);
            return View(carts.ToList());


            //var homedata = new homeview
            //{
            //    carts = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(x => x.userID == username).Where(y => y.cartStatus == false).Where(o => o.orderStatus == false)
            //};

            //return View(homedata);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            cart cart = db.carts.Find(id);
            db.carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");


        }

        public ActionResult DeleteAll()
        {
            var username = User.Identity.GetUserId();
            var cart = db.carts.Include(c => c.AspNetUser).Include(c => c.tbl_burger).Where(x => x.userID == username).Where(y => y.cartStatus == false).Where(o => o.orderStatus == false);
            db.carts.RemoveRange(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}