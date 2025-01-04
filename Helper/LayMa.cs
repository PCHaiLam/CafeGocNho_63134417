using CafeGocNho_63134417.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeGocNho_63134417.Helper
{
    public class LayId
    {
        private readonly CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();
        public string LayMa(string table, string maLoai = null)
        {
            if (table == "HOADON")
            {
                var maMax = db.HOADON.ToList().Select(n => n.MAHD).Max();
                int maHD = int.Parse(maMax.Substring(2)) + 1;
                return "HD" + maHD.ToString().PadLeft(3, '0');
            } else

            if (table == "NHANVIEN")
            {
                var maMax = db.NHANVIEN.ToList().Select(n => n.MANV).Max();
                int maNV = int.Parse(maMax.Substring(2)) + 1;
                return "NV" + maNV.ToString().PadLeft(3, '0');
            } else

            if (table == "BAN")
            {
                var maMax = db.BAN.ToList().Select(n => n.MABAN).Max();
                int maBan = maMax + 1;
                return maBan.ToString();
            }

            else
            {
                string maMax = db.MENU.Where(n => n.MALOAI == maLoai).Select(n => n.MAMH).Max();

                if (string.IsNullOrEmpty(maMax))
                {
                    return maLoai + "001";
                }

                int maMH = int.Parse(maMax.Substring(maLoai.Length)) + 1;
                return maLoai + maMH.ToString("D3");
            }

        }
    }
}