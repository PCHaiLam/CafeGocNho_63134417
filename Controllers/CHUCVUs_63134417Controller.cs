using CafeGocNho_63134417.Models;
using System.Linq;
using System.Web.Mvc;

namespace CafeGocNho_63134417.Controllers
{
    public class CHUCVUs_63134417Controller : Controller
    {
        private CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();

        // Kiểm tra tài khoản và trả về vai trò
        public string CheckUser(string username, string password)
        {
            string encryptedPassword = Helper.Md5Helper.EncryptMD5(password);

            var nhanVien = db.NHANVIEN
                             .Where(x => x.EMAIL == username && x.MATKHAU == encryptedPassword)
                             .FirstOrDefault();

            if (nhanVien != null)
            {
                // Lấy thông tin chức vụ từ bảng CHUCVU
                var chucVu = db.CHUCVU
                               .Where(x => x.MACV == nhanVien.MACV)
                               .FirstOrDefault();

                if (chucVu != null)
                {
                    // Lưu thông tin vào session
                    Session["Ten"] = nhanVien.TENNV;
                    Session["MaNV"] = nhanVien.MANV;
                    Session["Role"] = chucVu.MACV; // Lưu mã chức vụ (admin, manager, employee)

                    return chucVu.MACV; // Trả về mã chức vụ
                }
            }

            // Nếu không tìm thấy, đặt session về null
            Session["Ten"] = null;
            Session["MaNV"] = null;
            Session["Role"] = null;
            return "Unknown";
        }

        // Hiển thị trang đăng nhập
        public ActionResult DangNhap()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(NHANVIEN qt)
        {
            if (ModelState.IsValid)
            {
                var role = CheckUser(qt.EMAIL, qt.MATKHAU);

                if (role == "admin")
                {
                    return RedirectToAction("TongQuanHoaDon_63134417", "HOADONs_63134417");
                }
                else if (role == "manager" || role == "employee")
                {
                    return RedirectToAction("Index", "BANs_63134417");
                }
                else
                {
                    ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng.";
                }
            }

            return View(qt);
        }

        // Đăng xuất
        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa toàn bộ session
            return RedirectToAction("DangNhap", "CHUCVUs_63134417");
        }
    }
}
