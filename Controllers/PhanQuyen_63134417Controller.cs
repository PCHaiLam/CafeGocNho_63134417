using Antlr.Runtime.Misc;
using CafeGocNho_63134417.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace CafeGocNho_63134417.Controllers
{
    public class PhanQuyen_63134417Controller : Controller
    {
        // GET: PhanQuyen_63134417
        private CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();

        public bool CheckUser(string username, string password, out string role)
        {
            var kq = db.NHANVIEN.Where(x => x.EMAIL == username && x.MATKHAU == password).FirstOrDefault();
            if (kq != null)
            {
                Session["Email"] = kq.EMAIL;
                Session["Ten"] = kq.TENNV;
                Session["MaNV"] = kq.MANV;

                switch (kq.CHUCVU)
                {
                    case 0:
                        role = "Quản lý";
                        break;
                    case 1:
                        role = "Thu ngân";
                        break;
                    case 2:
                        role = "Nhân viên";
                        break;
                    default:
                        role = "Unknown";
                        break;
                }

                Session["Role"] = role;
                return true;
            }
            else
            {
                Session["Email"] = null;
                Session["Ten"] = null;
                Session["MaNV"] = null;
                role = "Unknown";
                Session["Role"] = role;
                return false;
            }
        }
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(NHANVIEN qt)
        {
            if (ModelState.IsValid)
            {
                if (CheckUser(qt.EMAIL, qt.MATKHAU, out string role))
                {
                    FormsAuthentication.SetAuthCookie(qt.EMAIL, true);

                    if (role == "Quản lý")
                    {
                        return RedirectToAction("TongQuanHoaDon_63134417", "HOADONs_63134417");
                    }
                    else if (role == "Thu ngân" || role == "Nhân viên")
                    {
                        return RedirectToAction("Index", "BANs_63134417");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(qt);
        }

    }
}