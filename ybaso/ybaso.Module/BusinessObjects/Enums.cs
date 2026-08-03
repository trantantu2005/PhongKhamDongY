using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ybaso.Module.BusinessObjects
{
    public enum VaiTroTaiKhoan
    {
        BacSi = 0,
        NhanVien = 1,
        BenhNhan = 2,
        QuanTri = 3
    }
    public enum LoaiLuotKham
    {
        BenhNhanMoi = 0,
        BenhNhanCu = 1,
        TaiKham = 2
    }
    public enum TrangThaiCaKham
    {
        ChoKham = 0,
        DangKham = 1,
        DaKham = 2,
        DaHuy = 3
    }

    public enum LoaiCaKham
    {
        TrucTiep = 0,
        TuXa = 1
    }
}