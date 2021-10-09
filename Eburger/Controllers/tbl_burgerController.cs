using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Eburger.Models;

namespace Eburger.Controllers
{
    public class tbl_burgerController : Controller
    {
        private E_burgerEntities db = new E_burgerEntities();

        // GET: tbl_burger
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var tbl_burger = db.tbl_burger.Include(t => t.burger_type);
            return View(tbl_burger.ToList());
        }

        // GET: tbl_burger/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_burger tbl_burger = db.tbl_burger.Find(id);
            if (tbl_burger == null)
            {
                return HttpNotFound();
            }
            return View(tbl_burger);
        }

        // GET: tbl_burger/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.typeID = new SelectList(db.burger_type, "typeID", "typeName");
            return View();
        }

        // POST: tbl_burger/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BurgerID,BurgerName,typeID,UnitPrice,BurgerWeight,ImagePath,Descriptions,ImageFile")] tbl_burger tbl_burger)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(tbl_burger.ImageFile.FileName);
                string extension = Path.GetExtension(tbl_burger.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                tbl_burger.ImagePath = "~/Images/burgerImage/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/burgerImage/"), fileName);
                tbl_burger.ImageFile.SaveAs(fileName);

                db.tbl_burger.Add(tbl_burger);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.typeID = new SelectList(db.burger_type, "typeID", "typeName", tbl_burger.typeID);
            return View(tbl_burger);
        }

        // GET: tbl_burger/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_burger tbl_burger = db.tbl_burger.Find(id);
            if (tbl_burger == null)
            {
                return HttpNotFound();
            }
            ViewBag.typeID = new SelectList(db.burger_type, "typeID", "typeName", tbl_burger.typeID);
            return View(tbl_burger);
        }

        // POST: tbl_burger/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BurgerID,BurgerName,typeID,UnitPrice,BurgerWeight,ImagePath,Descriptions")] tbl_burger tbl_burger)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_burger).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.typeID = new SelectList(db.burger_type, "typeID", "typeName", tbl_burger.typeID);
            return View(tbl_burger);
        }

        // GET: tbl_burger/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_burger tbl_burger = db.tbl_burger.Find(id);
            if (tbl_burger == null)
            {
                return HttpNotFound();
            }
            return View(tbl_burger);
        }

        // POST: tbl_burger/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_burger tbl_burger = db.tbl_burger.Find(id);
            db.tbl_burger.Remove(tbl_burger);
            db.SaveChanges();
            return RedirectToAction("Index");
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
