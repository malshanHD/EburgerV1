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
    public class burger_typeController : Controller
    {
        private E_burgerEntities db = new E_burgerEntities();

        // GET: burger_type
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.burger_type.ToList());
        }

        // GET: burger_type/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            burger_type burger_type = db.burger_type.Find(id);
            if (burger_type == null)
            {
                return HttpNotFound();
            }
            return View(burger_type);
        }

        // GET: burger_type/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: burger_type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "typeID,typeName,typeImage,ImageFile")] burger_type burger_type)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(burger_type.ImageFile.FileName);
                string extension = Path.GetExtension(burger_type.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                burger_type.typeImage = "~/Images/MenuImage/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/MenuImage/"), fileName);
                burger_type.ImageFile.SaveAs(fileName);

                db.burger_type.Add(burger_type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(burger_type);
        }

        // GET: burger_type/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            burger_type burger_type = db.burger_type.Find(id);
            if (burger_type == null)
            {
                return HttpNotFound();
            }
            return View(burger_type);
        }

        // POST: burger_type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "typeID,typeName,typeImage")] burger_type burger_type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(burger_type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(burger_type);
        }

        // GET: burger_type/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            burger_type burger_type = db.burger_type.Find(id);
            if (burger_type == null)
            {
                return HttpNotFound();
            }
            return View(burger_type);
        }

        // POST: burger_type/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            burger_type burger_type = db.burger_type.Find(id);
            db.burger_type.Remove(burger_type);
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
