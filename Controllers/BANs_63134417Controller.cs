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
                return View(new List<BAN>());
            }
        }
        ////ThongKeBan_63134417
        public ActionResult ThongKeBan_63134417()
        {
            var table = db.BAN.ToList();
            return View(table);
        }

        [HttpPost]
        public ActionResult Create()
        {
            var table = new BAN
            {
                TINHTRANG = 0
            };

            db.BAN.Add(table);
            db.SaveChanges();
            Session["Notification"] = "Thêm bàn thành công!";

            return RedirectToAction("ThongKeBan_63134417");
        }

        // POST: BANs_63134417/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int MABAN)
        {
            var table = db.BAN.Find(MABAN);
            if (table.TINHTRANG != 0)
            {
                Session["Notification"] = "Bàn đang có khách, không thể xóa!";
                return RedirectToAction("ThongKeBan_63134417");
            }

            db.BAN.Remove(table);
            db.SaveChanges();
            Session["Notification"] = "Xóa bàn thành công!";
            return RedirectToAction("ThongKeBan_63134417");
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
