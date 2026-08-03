using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.DatabaseUpdate
{
    public static class KhoDuocTestSeeder
    {
        public static void Seed(IObjectSpace objectSpace)
        {
            // Nếu đã có dữ liệu test rồi thì không tạo lại nữa.
            var daCoDuLieuTest = objectSpace.FindObject<DanhMucThuoc>(
                CriteriaOperator.Parse("MaThuoc = ?", "TEST001")
            );

            if (daCoDuLieuTest != null)
                return;

            ListDvt dvtThang = LayHoacTaoDvt(objectSpace, "Thang", true);
            ListDvt dvtGram = LayHoacTaoDvt(objectSpace, "Gram", false);

            Nhanvien nhanVienKho = LayHoacTaoNhanVien(
                objectSpace,
                "Nhân viên",
                "Kho mẫu",
                "0900000001"
            );

            Nhanvien bacSiMau = LayHoacTaoNhanVien(
                objectSpace,
                "Bác sĩ",
                "Mẫu",
                "0900000002"
            );

            Benhnhan benhNhanMau = LayHoacTaoBenhNhan(objectSpace);

            // 1. Tạo dược liệu mẫu.
            var dsDuocLieu = new (string Ma, string Ten, decimal SoLuongNhap, decimal GiaNhap)[]
            {
                ("DL001", "Đảng sâm", 50000m, 500m),
                ("DL002", "Bạch truật", 50000m, 350m),
                ("DL003", "Phục linh", 50000m, 300m),
                ("DL004", "Cam thảo", 50000m, 180m),
                ("DL005", "Toan táo nhân", 50000m, 450m),
                ("DL006", "Viễn chí", 50000m, 420m),
                ("DL007", "Ngũ vị tử", 50000m, 520m),
                ("DL008", "Mạch môn", 50000m, 380m),
                ("DL009", "Đương quy", 50000m, 480m),
                ("DL010", "Xuyên khung", 50000m, 400m),
                ("DL011", "Sinh khương", 50000m, 150m),
                ("DL012", "Quế chi", 50000m, 360m)
            };

            DuocLieu[] duocLieus = dsDuocLieu
                .Select(x => TaoDuocLieuMau(
                    objectSpace,
                    x.Ma,
                    x.Ten,
                    dvtGram,
                    x.GiaNhap
                ))
                .ToArray();

            // 2. Tạo phiếu nhập kho. Khi commit, phiếu này cộng tồn vào tab Dược liệu.
            TaoPhieuNhapMau(objectSpace, nhanVienKho, duocLieus, dsDuocLieu, dvtGram);
            objectSpace.CommitChanges();

            // 3. Tạo danh mục thuốc/thang thuốc.
            // Khi commit, DanhMucThuoc sẽ tự trừ dược liệu theo thành phần và giữ lại tồn thang thuốc.
            var dsThuoc = new (string Ma, string Ten, decimal SoLuongTonBanDau)[]
            {
                ("TEST001", "Tứ quân gia Toan táo nhân, viễn chí, ngũ vị tử", 100m),
                ("TEST002", "Qua lâu ung bạch quế chi thang", 80m),
                ("TEST003", "Thất tiếu tán", 70m),
                ("TEST004", "Tứ nghịch thang", 90m),
                ("TEST005", "Sinh mạch tán", 120m),
                ("TEST006", "Bổ tâm hoàn", 100m),
                ("TEST007", "Tứ vật thang gia A giao, Chích thảo, Bá tử nhân", 60m),
                ("TEST008", "Huyết phủ trục ứ thang", 60m),
                ("TEST009", "Đạo đàm thang", 80m),
                ("TEST010", "Tô hợp hương hoàn", 50m)
            };

            DanhMucThuoc[] thuocs = dsThuoc
                .Select(x => TaoThuocMau(
                    objectSpace,
                    x.Ma,
                    x.Ten,
                    dvtThang,
                    x.SoLuongTonBanDau
                ))
                .ToArray();

            TaoThanhPhanThuocMau(objectSpace, thuocs, duocLieus, dvtGram);

            objectSpace.CommitChanges();

            // 4. Tạo đơn thuốc và phiếu xuất mẫu để test luồng xuất thang thuốc cho bệnh nhân.
            Cakham caKhamMau = TaoCaKhamMau(objectSpace, bacSiMau, benhNhanMau);

            DonThuoc donThuocMau = TaoDonThuocMau(
                objectSpace,
                caKhamMau,
                thuocs,
                dvtThang
            );

            TaoPhieuXuatNhapTam(
                objectSpace,
                nhanVienKho,
                donThuocMau,
                thuocs,
                dvtThang
            );

            objectSpace.CommitChanges();
        }

        private static ListDvt LayHoacTaoDvt(IObjectSpace objectSpace, string tenDvt, bool macDinh)
        {
            var dvt = objectSpace.FindObject<ListDvt>(
                CriteriaOperator.Parse("TenDvt = ?", tenDvt)
            );

            if (dvt != null)
                return dvt;

            dvt = objectSpace.CreateObject<ListDvt>();
            dvt.TenDvt = tenDvt;
            dvt.Macdinh = macDinh;

            return dvt;
        }

        private static Nhanvien LayHoacTaoNhanVien(
            IObjectSpace objectSpace,
            string hoDem,
            string ten,
            string dienThoai)
        {
            var nv = objectSpace.FindObject<Nhanvien>(
                CriteriaOperator.Parse("Ten = ?", ten)
            );

            if (nv != null)
                return nv;

            nv = objectSpace.CreateObject<Nhanvien>();
            nv.Hodem = hoDem;
            nv.Ten = ten;
            nv.Dienthoai = dienThoai;
            nv.Email = $"{dienThoai}@test.local";

            return nv;
        }

        private static Benhnhan LayHoacTaoBenhNhan(IObjectSpace objectSpace)
        {
            var bn = objectSpace.FindObject<Benhnhan>(
                CriteriaOperator.Parse("Ten = ?", "Test kho")
            );

            if (bn != null)
                return bn;

            bn = objectSpace.CreateObject<Benhnhan>();
            bn.Ho = "Bệnh nhân";
            bn.Ten = "Test kho";
            bn.Gioitinh = 1;
            bn.Ngaysinh = new DateTime(2000, 1, 1);
            bn.Dienthoai = "0900000003";
            bn.Diachi = "Dữ liệu mẫu để test kho dược";
            bn.Tiensu = "Không";

            return bn;
        }

        private static DuocLieu TaoDuocLieuMau(
            IObjectSpace objectSpace,
            string maDuocLieu,
            string tenDuocLieu,
            ListDvt dvt,
            decimal giaNhap)
        {
            var duocLieu = objectSpace.FindObject<DuocLieu>(
                CriteriaOperator.Parse("MaDuocLieu = ?", maDuocLieu)
            );

            if (duocLieu != null)
                return duocLieu;

            duocLieu = objectSpace.CreateObject<DuocLieu>();
            duocLieu.MaDuocLieu = maDuocLieu;
            duocLieu.TenDuocLieu = tenDuocLieu;
            duocLieu.DonViTinh = dvt;
            duocLieu.SoLuongTon = 0;
            duocLieu.TonToiThieu = 1000;
            duocLieu.GiaNhapGanNhat = giaNhap;
            duocLieu.SuDung = true;
            duocLieu.MoTa = "Dữ liệu dược liệu mẫu để test kho dược";

            return duocLieu;
        }

        private static DanhMucThuoc TaoThuocMau(
            IObjectSpace objectSpace,
            string maThuoc,
            string tenThuoc,
            ListDvt dvt,
            decimal soLuongTonBanDau)
        {
            var thuoc = objectSpace.FindObject<DanhMucThuoc>(
                CriteriaOperator.Parse("MaThuoc = ?", maThuoc)
            );

            if (thuoc != null)
                return thuoc;

            thuoc = objectSpace.CreateObject<DanhMucThuoc>();
            thuoc.MaThuoc = maThuoc;
            thuoc.TenThuoc = tenThuoc;
            thuoc.DonViTinh = dvt;
            thuoc.SoLuongTon = soLuongTonBanDau;
            thuoc.TonToiThieu = 10;
            thuoc.TuDongTinhGiaBan = true;
            thuoc.GiaBan = 0;
            thuoc.SuDung = true;
            thuoc.MoTa = "Thang thuốc mẫu. Khi tạo sẽ trừ dược liệu theo thành phần.";

            return thuoc;
        }

        private static void TaoPhieuNhapMau(
            IObjectSpace objectSpace,
            Nhanvien nhanVienKho,
            DuocLieu[] duocLieus,
            (string Ma, string Ten, decimal SoLuongNhap, decimal GiaNhap)[] dsDuocLieu,
            ListDvt dvt)
        {
            var phieuNhap = objectSpace.CreateObject<PhieuNhapKho>();
            phieuNhap.MaPhieuNhap = "PN_TEST_001";
            phieuNhap.NhanVien = nhanVienKho;
            phieuNhap.NgayNhap = DateTime.Now;
            phieuNhap.NhaCungCap = "Nhà cung cấp dược liệu mẫu";
            phieuNhap.GhiChu = "Phiếu nhập mẫu: nhập dược liệu vào kho";
            phieuNhap.TrangThai = TrangThaiPhieuNhapKho.DaNhap;
            phieuNhap.DaCapNhatTon = false;

            for (int i = 0; i < duocLieus.Length; i++)
            {
                var ct = objectSpace.CreateObject<ChiTietPhieuNhap>();
                ct.PhieuNhap = phieuNhap;
                ct.DuocLieu = duocLieus[i];
                ct.SoLuong = dsDuocLieu[i].SoLuongNhap;
                ct.DonViTinh = dvt;
                ct.GiaNhap = dsDuocLieu[i].GiaNhap;
                ct.SoLo = "LO_TEST";
                ct.HanSuDung = DateTime.Now.AddYears(2);
                ct.GhiChu = "Dữ liệu nhập kho dược liệu mẫu";
            }
        }

        private static void TaoThanhPhanThuocMau(
            IObjectSpace objectSpace,
            DanhMucThuoc[] thuocs,
            DuocLieu[] duocLieus,
            ListDvt dvt)
        {
            // Mỗi thang thuốc mẫu có 3 vị dược liệu.
            // Số lượng là gram cho 1 thang.
            int[][] viTriDuocLieu = new int[][]
            {
                new[] { 0, 1, 4 },
                new[] { 2, 3, 11 },
                new[] { 8, 9, 3 },
                new[] { 0, 10, 11 },
                new[] { 0, 7, 6 },
                new[] { 4, 5, 6 },
                new[] { 8, 3, 4 },
                new[] { 8, 9, 0 },
                new[] { 2, 3, 7 },
                new[] { 5, 6, 11 }
            };

            for (int i = 0; i < thuocs.Length; i++)
            {
                TaoThanhPhan(objectSpace, thuocs[i], duocLieus[viTriDuocLieu[i][0]], 12m, dvt);
                TaoThanhPhan(objectSpace, thuocs[i], duocLieus[viTriDuocLieu[i][1]], 10m, dvt);
                TaoThanhPhan(objectSpace, thuocs[i], duocLieus[viTriDuocLieu[i][2]], 8m, dvt);
            }
        }

        private static void TaoThanhPhan(
            IObjectSpace objectSpace,
            DanhMucThuoc thuoc,
            DuocLieu duocLieu,
            decimal soLuong,
            ListDvt dvt)
        {
            bool daCo = thuoc.ThanhPhanThuocs.Any(x => x.DuocLieu == duocLieu);

            if (daCo)
                return;

            var thanhPhan = objectSpace.CreateObject<ThanhPhanThuoc>();
            thanhPhan.Thuoc = thuoc;
            thanhPhan.DuocLieu = duocLieu;
            thanhPhan.SoLuong = soLuong;
            thanhPhan.DonViTinh = dvt;
            thanhPhan.GhiChu = "Thành phần mẫu cho 1 thang thuốc";
            thanhPhan.CapNhatThanhTien();
        }

        private static Cakham TaoCaKhamMau(
            IObjectSpace objectSpace,
            Nhanvien bacSi,
            Benhnhan benhNhan)
        {
            var ca = objectSpace.CreateObject<Cakham>();
            ca.NgayCT = DateTime.Now;
            ca.Bacsi = bacSi;
            ca.BenhNhan = benhNhan;
            ca.LydoKham = "Ca khám mẫu để test đơn thuốc và xuất kho";
            ca.Benhchung = "Tâm khí hư";
            ca.Khihuyet = "Khí hư";
            ca.Huthuc = "Khí hư";
            ca.Amduong = "Dương hư";
            ca.DaPhanTich = true;

            return ca;
        }

        private static DonThuoc TaoDonThuocMau(
            IObjectSpace objectSpace,
            Cakham ca,
            DanhMucThuoc[] thuocs,
            ListDvt dvt)
        {
            var don = objectSpace.CreateObject<DonThuoc>();
            don.MaDonThuoc = "DT_TEST_001";
            don.Ca = ca;
            don.NgayKeDon = DateTime.Now;
            don.ChanDoan = "Tâm khí hư";
            don.GhiChu = "Đơn thuốc mẫu để test xuất kho thang thuốc đã làm sẵn";
            don.TrangThai = TrangThaiDonThuoc.ChuaXuat;

            TaoChiTietDonThuoc(objectSpace, don, thuocs[0], 2, dvt, "Sắc uống ngày 1 thang");
            TaoChiTietDonThuoc(objectSpace, don, thuocs[4], 1, dvt, "Dùng theo chỉ định");
            TaoChiTietDonThuoc(objectSpace, don, thuocs[5], 1, dvt, "Dùng buổi tối");

            return don;
        }

        private static void TaoChiTietDonThuoc(
            IObjectSpace objectSpace,
            DonThuoc don,
            DanhMucThuoc thuoc,
            decimal soLuong,
            ListDvt dvt,
            string lieuDung)
        {
            var ct = objectSpace.CreateObject<ChiTietDonThuoc>();
            ct.DonThuoc = don;
            ct.Thuoc = thuoc;
            ct.SoLuong = soLuong;
            ct.DonViTinh = dvt;
            ct.DonGia = thuoc.GiaBan;
            ct.LieuDung = lieuDung;
            ct.GhiChu = "Dữ liệu test";
        }

        private static void TaoPhieuXuatNhapTam(
            IObjectSpace objectSpace,
            Nhanvien nhanVienKho,
            DonThuoc don,
            DanhMucThuoc[] thuocs,
            ListDvt dvt)
        {
            var phieuXuat = objectSpace.CreateObject<PhieuXuatKho>();
            phieuXuat.MaPhieuXuat = "PX_TEST_001";
            phieuXuat.NhanVien = nhanVienKho;
            phieuXuat.DonThuoc = don;
            phieuXuat.NgayXuat = DateTime.Now;
            phieuXuat.GhiChu = "Phiếu xuất mẫu: khi xác nhận chỉ trừ tồn trong Danh mục thuốc";
            phieuXuat.TrangThai = TrangThaiPhieuXuatKho.NhapTam;
            phieuXuat.DaCapNhatTon = false;

            TaoChiTietPhieuXuat(objectSpace, phieuXuat, thuocs[0], 2, dvt);
            TaoChiTietPhieuXuat(objectSpace, phieuXuat, thuocs[4], 1, dvt);
            TaoChiTietPhieuXuat(objectSpace, phieuXuat, thuocs[5], 1, dvt);
        }

        private static void TaoChiTietPhieuXuat(
            IObjectSpace objectSpace,
            PhieuXuatKho phieuXuat,
            DanhMucThuoc thuoc,
            decimal soLuong,
            ListDvt dvt)
        {
            var ct = objectSpace.CreateObject<ChiTietPhieuXuat>();
            ct.PhieuXuat = phieuXuat;
            ct.Thuoc = thuoc;
            ct.SoLuong = soLuong;
            ct.DonViTinh = dvt;
            ct.GiaXuat = thuoc.GiaBan;
            ct.GhiChu = "Dữ liệu xuất kho mẫu";
        }
    }
}
