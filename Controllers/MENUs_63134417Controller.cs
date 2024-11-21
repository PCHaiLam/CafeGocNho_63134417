using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CafeGocNho_63134417.Models;

namespace CafeGocNho_63134417.Controllers
{
    public class MENUs_63134417Controller : Controller
    {
        private CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();

        // GET: MENUs_63134417
        public ActionResult Index(string search = "", string filter = "all", string tableId = null)
        {
            if (string.IsNullOrEmpty(tableId))
            {
                ViewBag.ShowAlert = true;
            }

            IQueryable<MENU> menuQuery = db.MENU.Include(m => m.LOAIMATHANG);

            if (filter != "all")
            {
                menuQuery = menuQuery.Where(m => m.MALOAI == filter);
            }

            if (!string.IsNullOrEmpty(search))
            {
                menuQuery = menuQuery.Where(m => m.TENMH.Contains(search));
            }

            ViewBag.Filter = filter;
            ViewBag.Search = search;
            ViewBag.TableId = tableId;

            return View(menuQuery.ToList());
        }

        //ThongKeMenu_63134417
        public ActionResult ThongKeMenu_63134417()
        {
            return View()
;
        }

        // GET: MENUs_63134417/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENU mENU = db.MENU.Find(id);
            if (mENU == null)
            {
                return HttpNotFound();
            }
            return View(mENU);
        }

        // GET: MENUs_63134417/Create
        public ActionResult Create()
        {
            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI");
            return View();
        }

        // POST: MENUs_63134417/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAMH,TENMH,GIACA,DVT,MALOAI")] MENU mENU)
        {
            if (ModelState.IsValid)
            {
                db.MENU.Add(mENU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI", mENU.MALOAI);
            return View(mENU);
        }

        // GET: MENUs_63134417/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENU mENU = db.MENU.Find(id);
            if (mENU == null)
            {
                return HttpNotFound();
            }
            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI", mENU.MALOAI);
            return View(mENU);
        }

        // POST: MENUs_63134417/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAMH,TENMH,GIACA,DVT,MALOAI")] MENU mENU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mENU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI", mENU.MALOAI);
            return View(mENU);
        }

        // GET: MENUs_63134417/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENU mENU = db.MENU.Find(id);
            if (mENU == null)
            {
                return HttpNotFound();
            }
            return View(mENU);
        }

        // POST: MENUs_63134417/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MENU mENU = db.MENU.Find(id);
            db.MENU.Remove(mENU);
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
