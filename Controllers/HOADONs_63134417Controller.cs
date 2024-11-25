using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CafeGocNho_63134417.Models;
using Newtonsoft.Json;

namespace CafeGocNho_63134417.Controllers
{
    public class HOADONs_63134417Controller : Controller
    {
        private readonly CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();

        // GET: HOADONs_63134417
        [HttpPost]
        public ActionResult XacNhan_63134417(string maHD, int maBan, string maNV, string cartData)
        {
            var cart = JsonConvert.DeserializeObject<List<CHITIETHOADON>>(cartData);
            var hoaDon = db.HOADON.Include(h => h.BAN).FirstOrDefault(h => h.MAHD == maHD && h.MABAN == maBan);

            if (hoaDon == null)
            {
                hoaDon = new HOADON
                {
                    MAHD = maHD,
                    MABAN = maBan,
                    MANV = maNV,
                    THOIGIAN = DateTime.Now,
                    THANHTOAN = 0
                };

                db.HOADON.Add(hoaDon);
                db.SaveChanges();
            }

            var ban = db.BAN.FirstOrDefault(b => b.MABAN == maBan);
            if (ban != null)
            {
                ban.TINHTRANG = 1;  
                db.SaveChanges(); 
            }

            foreach (var item in cart)
            {
                var existingChiTiet = db.CHITIETHOADON
                    .FirstOrDefault(ct => ct.MAHD == hoaDon.MAHD && ct.MAMH == item.MAMH);

                if (existingChiTiet != null)
                {
                    existingChiTiet.SOLUONG += item.SOLUONG;
                }
                else
                {
                    var chiTietHoaDon = new CHITIETHOADON
                    {
                        MAHD = hoaDon.MAHD,
                        MAMH = item.MAMH,
                        SOLUONG = item.SOLUONG,
                    };
                    db.CHITIETHOADON.Add(chiTietHoaDon);
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index", new { tableId = maBan });
        }
        [HttpPost]
        public ActionResult ThanhToan_63134417(string maHD, string maBan, int discount)
        {
            var hoaDon = db.HOADON.FirstOrDefault(h => h.MAHD == maHD && h.MABAN.ToString() == maBan && h.THANHTOAN == 0);

            if (hoaDon != null)
            {
                hoaDon.THANHTOAN = 1;
                hoaDon.BAN.TINHTRANG = 0;
                hoaDon.GIAMGIA = discount;
                hoaDon.NV_THANHTOAN = Session["MaNV"].ToString();

                var chiTietHoaDonList = db.CHITIETHOADON.Where(ct => ct.MAHD == maHD).ToList();

                foreach (var chiTiet in chiTietHoaDonList)
                {
                    // Lấy thông tin mặt hàng từ bảng MENU
                    var menuItem = db.MENU.FirstOrDefault(m => m.MAMH == chiTiet.MAMH);
                    if (menuItem != null)
                    {
                        // Trừ số lượng trong MENU
                        menuItem.SOLUONGHANG -= chiTiet.SOLUONG;

                        // Đảm bảo không để số lượng âm
                        if (menuItem.SOLUONGHANG < 0)
                        {
                            menuItem.SOLUONGHANG = 0;
                        }
                    }
                }
                    db.SaveChanges();
            }

            return RedirectToAction("Index", new { tableId = maBan });
        }
        [HttpPost]
        public ActionResult Xoa_63134417(string maHD, string maBan, string maMH)
        {
            var chiTietHoaDon = db.CHITIETHOADON
                          .FirstOrDefault(c => c.HOADON.MAHD == maHD && c.MAMH == maMH);

            if (chiTietHoaDon != null)
            {
                db.CHITIETHOADON.Remove(chiTietHoaDon);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { tableId = maBan });
        }

        public ActionResult Index(string tableId = null)
        {
            if (string.IsNullOrEmpty(tableId))
            {
                ViewBag.ShowAlert = true;
            }

            var hOADON = db.HOADON.Include(h => h.BAN).Include(h => h.NHANVIEN);
            var menu = db.MENU.Include(m => m.LOAIMATHANG);

            Helper.LayId getid = new Helper.LayId();
            string maHD;

            var chiTietHoaDon = db.CHITIETHOADON
                .Include(c => c.MENU)
                .Include(h => h.HOADON)
                .Where(c => c.HOADON.MABAN.ToString() == tableId && c.HOADON.THANHTOAN == 0)
                .ToList();

            if (chiTietHoaDon.Any())
            {
                maHD = chiTietHoaDon.First().MAHD; // Lấy mã hóa đơn đầu tiên trong danh sách
                ViewBag.Pay = true;
            }
            else
            {
                maHD = getid.LayMa("HOADON"); // Tạo mã mới nếu không có hóa đơn nào chưa xác nhận
                ViewBag.Pay = false;

            }

            ViewBag.TableId = tableId;
            ViewBag.BillId = maHD;
            ViewBag.CTHD = chiTietHoaDon;
            ViewBag.Menu = menu;

            return View(hOADON.ToList());
        }
        //thống kê
        public ActionResult ThongKeHoaDon_63134417(string search, string dateOption = "", DateTime? date = null)
        {
            var danhSachHoaDon = db.HOADON.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                danhSachHoaDon = danhSachHoaDon.Where(h => h.MAHD.Contains(search));
            }

            if (dateOption == "today")
            {
                DateTime today = DateTime.Today;
                danhSachHoaDon = danhSachHoaDon.Where(h => DbFunctions.TruncateTime(h.THOIGIAN) == today);
            }
            else if (dateOption == "custom" && date.HasValue)
            {
                danhSachHoaDon = danhSachHoaDon.Where(h => DbFunctions.TruncateTime(h.THOIGIAN) == DbFunctions.TruncateTime(date.Value));
            }
            else if (dateOption == "all")
            {
            }

            var result = danhSachHoaDon.ToList();

            return View(result);
        }
        
        //tổng quan
        public ActionResult TongQuanHoaDon_63134417()
        {
            DateTime today = DateTime.Today;
            var doanhThu = db.HOADON
                            .Where(h => h.THANHTOAN == 1 && DbFunctions.TruncateTime(h.THOIGIAN) == today)
                            .Sum(h => h.CHITIETHOADON.Sum(ct => (ct.SOLUONG * ct.MENU.GIACA) *  (1 - h.GIAMGIA*0.01) )) ?? 0;

            var hangDaBan = db.CHITIETHOADON
                                .Where(c => DbFunctions.TruncateTime(c.HOADON.THOIGIAN) == today && c.HOADON.THANHTOAN == 1)
                                .Sum(ct => ct.SOLUONG) ??0;

            var hangTon = db.MENU.Sum(m => m.SOLUONGHANG) ??0;

            var chiTietSanPhamDaBan = db.CHITIETHOADON.Where(c => DbFunctions.TruncateTime(c.HOADON.THOIGIAN) == today && c.HOADON.THANHTOAN == 1).ToList();

            var tongTienhang = db.HOADON
                            .Where(h => h.THANHTOAN == 1 && DbFunctions.TruncateTime(h.THOIGIAN) == today)
                            .Sum(h => h.CHITIETHOADON.Sum(ct => (ct.SOLUONG * ct.MENU.GIACA))) ?? 0;

            ViewBag.doanhThu =  doanhThu;
            ViewBag.hangDaBan = hangDaBan;
            ViewBag.hangTon = hangTon;
            ViewBag.chiTietSanPhamDaBan = chiTietSanPhamDaBan;
            ViewBag.tongTienhang = tongTienhang;

            return View();
        }

        // GET: HOADONs_63134417/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADON.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            return View(hOADON);
        }

        // GET: HOADONs_63134417/Create
        public ActionResult Create()
        {
            ViewBag.MABAN = new SelectList(db.BAN, "MABAN", "MABAN");
            ViewBag.MANV = new SelectList(db.NHANVIEN, "MANV", "HONV");
            return View();
        }

        // POST: HOADONs_63134417/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAHD,MABAN,MANV,THOIGIAN,THANHTOAN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                db.HOADON.Add(hOADON);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MABAN = new SelectList(db.BAN, "MABAN", "MABAN", hOADON.MABAN);
            ViewBag.MANV = new SelectList(db.NHANVIEN, "MANV", "HONV", hOADON.MANV);
            return View(hOADON);
        }

        // GET: HOADONs_63134417/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADON.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            ViewBag.MABAN = new SelectList(db.BAN, "MABAN", "MABAN", hOADON.MABAN);
            ViewBag.MANV = new SelectList(db.NHANVIEN, "MANV", "HONV", hOADON.MANV);
            return View(hOADON);
        }

        // POST: HOADONs_63134417/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAHD,MABAN,MANV,THOIGIAN,THANHTOAN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MABAN = new SelectList(db.BAN, "MABAN", "MABAN", hOADON.MABAN);
            ViewBag.MANV = new SelectList(db.NHANVIEN, "MANV", "HONV", hOADON.MANV);
            return View(hOADON);
        }

        // GET: HOADONs_63134417/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADON.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            return View(hOADON);
        }

        // POST: HOADONs_63134417/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HOADON hOADON = db.HOADON.Find(id);
            db.HOADON.Remove(hOADON);
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
