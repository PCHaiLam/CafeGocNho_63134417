using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CafeGocNho_63134417.Models;

namespace CafeGocNho_63134417.Controllers
{
    public class NHANVIENs_63134417Controller : Controller
    {
        private CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();
        private readonly Helper.LayId layId = new Helper.LayId();

        // GET: NHANVIENs_63134417
        public ActionResult Index()
        {
            return View(db.NHANVIEN.ToList());
        }

        // GET: NHANVIENs_63134417/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIEN.Find(id);
            if (nHANVIEN == null)
            {
                return HttpNotFound();
            }
            return View(nHANVIEN);
        }

        // GET: NHANVIENs_63134417/Create
        public ActionResult Create()
        {
            ViewBag.MANV = layId.LayMa("NHANVIEN");
            return View();
        }

        // POST: NHANVIENs_63134417/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MANV,HONV,TENNV,EMAIL,DIACHI,MATKHAU,GIOITINH,SDT,CCCD,MACV")] NHANVIEN nHANVIEN)
        {
            if (nHANVIEN == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            nHANVIEN.MANV = layId.LayMa("NHANVIEN");
            nHANVIEN.MATKHAU = Helper.Md5Helper.EncryptMD5(nHANVIEN.MATKHAU);

            if (ModelState.IsValid)
            {
                try
                {
                    db.NHANVIEN.Add(nHANVIEN);
                    db.SaveChanges();
                    Session["Notification"] = "Thêm nhân viên thành công!";
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }

            return View(nHANVIEN);
        }


        // GET: NHANVIENs_63134417/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIEN.Find(id);
            if (nHANVIEN == null)
            {
                return HttpNotFound();
            }
            return View(nHANVIEN);
        }

        // POST: NHANVIENs_63134417/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MANV,HONV,TENNV,EMAIL,DIACHI,MATKHAU,MACV,GIOITINH,SDT,CCCD")] NHANVIEN nHANVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHANVIEN).State = EntityState.Modified;
                db.SaveChanges();
                Session["Notification"] = "Sửa thông tin nhân viên thành công!";
                return RedirectToAction("Index");
            }
            return View(nHANVIEN);
        }

        // GET: NHANVIENs_63134417/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIEN.Find(id);
            if (nHANVIEN == null)
            {
                return HttpNotFound();
            }
            return View(nHANVIEN);
        }

        // POST: NHANVIENs_63134417/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string MANV)
        {
            NHANVIEN nHANVIEN = db.NHANVIEN.Find(MANV);
            db.NHANVIEN.Remove(nHANVIEN);
            db.SaveChanges();
            Session["Notification"] = "Xóa nhân viên thành công!";
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
