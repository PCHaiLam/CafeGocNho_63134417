using CafeGocNho_63134417.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeGocNho_63134417.Helper
{
    public class LayId
    {
        private CafeGocNho_63134417Entities db = new CafeGocNho_63134417Entities();
        public string LayMa(string table) 
        {
            if(table == "HOADON")
            {
                var maMax = db.HOADON.ToList().Select(n => n.MAHD).Max();
                int maHD = int.Parse(maMax.Substring(2)) + 1;
                string NV = String.Concat("000", maHD.ToString());
                return "HD" + NV.Substring(maHD.ToString().Length - 1);
            } else
            if(table == "NHANVIEN")
            {
                var maMax = db.HOADON.ToList().Select(n => n.MAHD).Max();
                int maNV = int.Parse(maMax.Substring(2)) + 1;
                string NV = String.Concat("00", maNV.ToString());
                return "NV" + NV.Substring(maNV.ToString().Length - 1);
            } else
            {
                var maMax = db.HOADON.ToList().Select(n => n.MAHD).Max();
                int maMH = int.Parse(maMax.Substring(2)) + 1;
                string NV = String.Concat("00", maMH.ToString());
                return NV.Substring(maMH.ToString().Length - 1);
            }
        }
    }
}