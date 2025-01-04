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
        private readonly CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();
        private readonly Helper.LayId layId = new Helper.LayId();

        // GET: MENUs_63134417
        public ActionResult Index(string search = "", string filter = "all")
        {
            IQueryable<MENU> menuQuery = db.MENU.Include(m => m.LOAIMATHANG).Where(m => m.TRANGTHAI_XOA != 1);

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

            return View(menuQuery.ToList());
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
        public ActionResult Create(string MALOAI)
        {
            ViewBag.MAMH = layId.LayMa("MENU", MALOAI);
            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI", MALOAI);
            ViewBag.SelectedMALOAI = MALOAI; 
            return View();
        }

        // POST: MENUs_63134417/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAMH,TENMH,GIACA,DVT,ANH,SOLUONGHANG,MALOAI")] MENU mENU, string MALOAI)
        {
            var img = Request.Files["Anh"];
            string postedFileName = System.IO.Path.GetFileName(img.FileName);
            var savePath = Server.MapPath("/Images/imgMenu/" + postedFileName);
            img.SaveAs(savePath);
            
            mENU.MAMH = layId.LayMa("MENU", MALOAI);

            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI", mENU.MALOAI);

            if (ModelState.IsValid)
            {
                mENU.ANH = postedFileName;
                db.MENU.Add(mENU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
        public ActionResult Edit([Bind(Include = "MAMH,TENMH,GIACA,DVT,ANH,SOLUONGHANG")] MENU mENU)
        {
            // Lấy thông tin đối tượng từ database để lấy tên file ảnh cũ
            var menuToUpdate = db.MENU.Find(mENU.MAMH);
            string oldFileName = menuToUpdate.ANH;

            if (Request.Files.Count > 0)
            {
                var img = Request.Files["Anh"]; 

                if (img != null && img.ContentLength > 0)
                {
                    // Người dùng có chọn ảnh mới
                    string postedFileName = System.IO.Path.GetFileName(img.FileName);
                    var savePath = Server.MapPath("/Images/imgMenu/" + postedFileName);
                    img.SaveAs(savePath);

                    // Xóa ảnh cũ (nếu có)
                    if (!string.IsNullOrEmpty(oldFileName))
                    {
                        var oldFilePath = Server.MapPath("/Images/imgMenu/" + oldFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    mENU.ANH = postedFileName; // Cập nhật ANH với tên file mới
                }
            }

            if (ModelState.IsValid)
            {
                // Gán lại các thuộc tính thay đổi cho đối tượng trong database context
                menuToUpdate.TENMH = mENU.TENMH;
                menuToUpdate.GIACA = mENU.GIACA;
                menuToUpdate.DVT = mENU.DVT;
                menuToUpdate.SOLUONGHANG = mENU.SOLUONGHANG;
                if (!string.IsNullOrEmpty(mENU.ANH))
                {
                    menuToUpdate.ANH = mENU.ANH;
                }

                db.Entry(menuToUpdate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MALOAI = new SelectList(db.LOAIMATHANG, "MALOAI", "TENLOAI", mENU.MALOAI);
            return View(mENU);
        }

        // POST: MENUs_63134417/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string MAMH)
        {
            // Tìm bản ghi trong MENU theo MAMH
            MENU mENU = db.MENU.Find(MAMH);
            if (mENU != null)
            {
                // Cập nhật trường TRANGTHAI_XOA thành 1
                mENU.TRANGTHAI_XOA = 1;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }

            Session["Notification"] = "Xóa thành công";

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
