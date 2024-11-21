using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CafeGocNho_63134417.Models;

namespace CafeGocNho_63134417.Controllers
{
    public class BANs_63134417Controller : Controller
    {
        private CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();

        // GET: BANs_63134417
        public ActionResult Index(int page = 1, string filter = "all", int? tableId = null)
        {
            if (Session["Ten"] == null || Session["Ten"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "PhanQuyen_63134417");
            }

            int pageSize = 24; // số lượng bàn trogn 1 trang
            IQueryable<BAN> tablesQuery = db.BAN;

            // lọc bàn trống
            switch (filter.ToLower())
            {
                case "available":
                    tablesQuery = tablesQuery.Where(b => b.TINHTRANG == 0);
                    break;
                case "occupied":
                    tablesQuery = tablesQuery.Where(b => b.TINHTRANG != 0);
                    break;
                default:
                    break;
            }

            // phân trang bàn
            int totalItems = tablesQuery.Count();
            var totalPages = totalItems > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 1; // Đảm bảo totalPages luôn >= 1

            // Điều chỉnh lại giá trị của page để đảm bảo nó hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            // Kiểm tra thêm để đảm bảo Skip không nhận giá trị âm
            if (totalItems > 0)
            {
                var tables = tablesQuery
                    .OrderBy(b => b.MABAN)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.Filter = filter;

                return View(tables);
                }
            else
            {
                //ViewBag.NoTable = "Không có bàn nào để hiển thị.";
                return View(new List<BAN>()); // Trả về danh sách trống
            }
        }
        ////ThongKeBan_63134417
        public ActionResult ThongKeBan_63134417()
        {
            return View()
;
        }
        // GET: BANs_63134417/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAN bAN = db.BAN.Find(id);
            if (bAN == null)
            {
                return HttpNotFound();
            }
            return View(bAN);
        }

        // GET: BANs_63134417/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BANs_63134417/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MABAN,TINHTRANG")] BAN bAN)
        {
            if (ModelState.IsValid)
            {
                db.BAN.Add(bAN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bAN);
        }

        // GET: BANs_63134417/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAN bAN = db.BAN.Find(id);
            if (bAN == null)
            {
                return HttpNotFound();
            }
            return View(bAN);
        }

        // POST: BANs_63134417/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MABAN,TINHTRANG")] BAN bAN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bAN);
        }

        // GET: BANs_63134417/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAN bAN = db.BAN.Find(id);
            if (bAN == null)
            {
                return HttpNotFound();
            }
            return View(bAN);
        }

        // POST: BANs_63134417/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BAN bAN = db.BAN.Find(id);
            db.BAN.Remove(bAN);
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
