using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.BaoCao)]
    [XafDisplayName("Báo cáo doanh thu")]
    [DefaultProperty(nameof(TenBaoCao))]
    public class BaoCaoDoanhThu : XPObject
    {
        public BaoCaoDoanhThu(Session session) : base(session) { }

        private string tenBaoCao;

        [Size(255)]
        [XafDisplayName("Tên báo cáo")]
        public string TenBaoCao
        {
            get => tenBaoCao;
            set => SetPropertyValue(nameof(TenBaoCao), ref tenBaoCao, value);
        }

        private DateTime tuNgay;

        [XafDisplayName("Từ ngày")]
        public DateTime TuNgay
        {
            get => tuNgay;
            set => SetPropertyValue(nameof(TuNgay), ref tuNgay, value);
        }

        private DateTime denNgay;

        [XafDisplayName("Đến ngày")]
        public DateTime DenNgay
        {
            get => denNgay;
            set => SetPropertyValue(nameof(DenNgay), ref denNgay, value);
        }

        private DateTime ngayLap;

        [XafDisplayName("Ngày lập")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime NgayLap
        {
            get => ngayLap;
            set => SetPropertyValue(nameof(NgayLap), ref ngayLap, value);
        }

        private int tongSoHoaDon;

        [XafDisplayName("Tổng số hóa đơn")]
        [ModelDefault("AllowEdit", "False")]
        public int TongSoHoaDon
        {
            get => tongSoHoaDon;
            set => SetPropertyValue(nameof(TongSoHoaDon), ref tongSoHoaDon, value);
        }

        private int tongSoPhieuNhap;

        [XafDisplayName("Tổng số phiếu nhập kho")]
        [ModelDefault("AllowEdit", "False")]
        public int TongSoPhieuNhap
        {
            get => tongSoPhieuNhap;
            set => SetPropertyValue(nameof(TongSoPhieuNhap), ref tongSoPhieuNhap, value);
        }

        private decimal tongDoanhThu;

        [XafDisplayName("Tổng doanh thu hóa đơn")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongDoanhThu
        {
            get => tongDoanhThu;
            set => SetPropertyValue(nameof(TongDoanhThu), ref tongDoanhThu, value);
        }

        private decimal doanhThuTienMat;

        [XafDisplayName("Doanh thu tiền mặt")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal DoanhThuTienMat
        {
            get => doanhThuTienMat;
            set => SetPropertyValue(nameof(DoanhThuTienMat), ref doanhThuTienMat, value);
        }

        private decimal doanhThuChuyenKhoan;

        [XafDisplayName("Doanh thu chuyển khoản")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal DoanhThuChuyenKhoan
        {
            get => doanhThuChuyenKhoan;
            set => SetPropertyValue(nameof(DoanhThuChuyenKhoan), ref doanhThuChuyenKhoan, value);
        }

        private decimal doanhThuKhac;

        [XafDisplayName("Doanh thu khác")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal DoanhThuKhac
        {
            get => doanhThuKhac;
            set => SetPropertyValue(nameof(DoanhThuKhac), ref doanhThuKhac, value);
        }

        private decimal tongTienThuoc;

        [XafDisplayName("Tổng tiền thuốc")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongTienThuoc
        {
            get => tongTienThuoc;
            set => SetPropertyValue(nameof(TongTienThuoc), ref tongTienThuoc, value);
        }

        private decimal tongTienKham;

        [XafDisplayName("Tổng tiền khám")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongTienKham
        {
            get => tongTienKham;
            set => SetPropertyValue(nameof(TongTienKham), ref tongTienKham, value);
        }

        private decimal tongGiamGia;

        [XafDisplayName("Tổng giảm giá")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongGiamGia
        {
            get => tongGiamGia;
            set => SetPropertyValue(nameof(TongGiamGia), ref tongGiamGia, value);
        }

        private decimal tongChiPhiNhapKho;

        [XafDisplayName("Tổng chi phí nhập kho")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongChiPhiNhapKho
        {
            get => tongChiPhiNhapKho;
            set => SetPropertyValue(nameof(TongChiPhiNhapKho), ref tongChiPhiNhapKho, value);
        }

        private decimal loiNhuanTamTinh;

        [XafDisplayName("Lợi nhuận tạm tính")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal LoiNhuanTamTinh
        {
            get => loiNhuanTamTinh;
            set => SetPropertyValue(nameof(LoiNhuanTamTinh), ref loiNhuanTamTinh, value);
        }

        private string ghiChu;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        [Association("BaoCaoDoanhThu-HoaDons")]
        [XpoAggregated]
        [XafDisplayName("Hóa đơn bệnh nhân")]
        public XPCollection<ChiTietBaoCaoHoaDon> HoaDons
        {
            get => GetCollection<ChiTietBaoCaoHoaDon>(nameof(HoaDons));
        }

        [Association("BaoCaoDoanhThu-PhieuNhaps")]
        [XpoAggregated]
        [XafDisplayName("Phiếu nhập kho")]
        public XPCollection<ChiTietBaoCaoPhieuNhapKho> PhieuNhaps
        {
            get => GetCollection<ChiTietBaoCaoPhieuNhapKho>(nameof(PhieuNhaps));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            DateTime now = DateTime.Now;
            TuNgay = now.Date;
            DenNgay = now.Date;
            NgayLap = now;
            TenBaoCao = $"Báo cáo doanh thu - {now:dd/MM/yyyy HH:mm}";
        }

        [Action(Caption = "Tạo báo cáo", ImageName = "Action_Refresh", AutoCommit = true)]
        public void TaoBaoCao()
        {
            XoaChiTietCu();

            DateTime tu = TuNgay.Date;
            DateTime den = DenNgay.Date.AddDays(1).AddTicks(-1);

            var hoaDons = new XPQuery<HoaDonBenhNhan>(Session)
                .Where(x => x.TrangThai == TrangThaiHoaDonBenhNhan.DaThanhToan)
                .ToList()
                .Where(x =>
                    x.NgayThanhToan.HasValue &&
                    x.NgayThanhToan.Value >= tu &&
                    x.NgayThanhToan.Value <= den
                )
                .OrderBy(x => x.NgayThanhToan)
                .ToList();

            foreach (HoaDonBenhNhan hoaDon in hoaDons)
            {
                HoaDons.Add(new ChiTietBaoCaoHoaDon(Session)
                {
                    BaoCao = this,
                    HoaDon = hoaDon,
                    MaHoaDon = hoaDon.MaHoaDon,
                    NgayThanhToan = hoaDon.NgayThanhToan,
                    BenhNhan = hoaDon.BenhNhan == null ? "" : hoaDon.BenhNhan.HoTen,
                    HinhThucThanhToan = hoaDon.HinhThucThanhToan.ToString(),
                    TongTienThuoc = hoaDon.TongTienThuoc,
                    TienKham = hoaDon.TienKham,
                    GiamGia = hoaDon.GiamGia,
                    TongThanhToan = hoaDon.TongThanhToan
                });
            }

            var phieuNhaps = new XPQuery<PhieuNhapKho>(Session)
                .Where(x =>
                    x.TrangThai == TrangThaiPhieuNhapKho.DaNhap &&
                    x.NgayNhap >= tu &&
                    x.NgayNhap <= den
                )
                .OrderBy(x => x.NgayNhap)
                .ToList();

            foreach (PhieuNhapKho phieuNhap in phieuNhaps)
            {
                PhieuNhaps.Add(new ChiTietBaoCaoPhieuNhapKho(Session)
                {
                    BaoCao = this,
                    PhieuNhap = phieuNhap,
                    MaPhieuNhap = phieuNhap.MaPhieuNhap,
                    NgayNhap = phieuNhap.NgayNhap,
                    NhaCungCap = phieuNhap.NhaCungCap,
                    TongTienNhap = phieuNhap.TongTien
                });
            }

            TongSoHoaDon = hoaDons.Count;
            TongDoanhThu = hoaDons.Sum(x => x.TongThanhToan);
            TongTienThuoc = hoaDons.Sum(x => x.TongTienThuoc);
            TongTienKham = hoaDons.Sum(x => x.TienKham);
            TongGiamGia = hoaDons.Sum(x => x.GiamGia);

            DoanhThuTienMat = hoaDons
                .Where(x => x.HinhThucThanhToan == HinhThucThanhToanHoaDon.TienMat)
                .Sum(x => x.TongThanhToan);

            DoanhThuChuyenKhoan = hoaDons
                .Where(x => x.HinhThucThanhToan == HinhThucThanhToanHoaDon.ChuyenKhoan)
                .Sum(x => x.TongThanhToan);

            DoanhThuKhac = hoaDons
                .Where(x => x.HinhThucThanhToan == HinhThucThanhToanHoaDon.Khac)
                .Sum(x => x.TongThanhToan);

            TongSoPhieuNhap = phieuNhaps.Count;
            TongChiPhiNhapKho = phieuNhaps.Sum(x => x.TongTien);

            LoiNhuanTamTinh = TongDoanhThu - TongChiPhiNhapKho;

            GhiChu =
                $"Doanh thu từ hóa đơn: {TongDoanhThu:n0}. " +
                $"Chi phí nhập kho: {TongChiPhiNhapKho:n0}. " +
                $"Lợi nhuận tạm tính: {LoiNhuanTamTinh:n0}.";
        }

        private void XoaChiTietCu()
        {
            foreach (var item in HoaDons.ToList())
                item.Delete();

            foreach (var item in PhieuNhaps.ToList())
                item.Delete();
        }

        public override string ToString()
        {
            return TenBaoCao;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết hóa đơn báo cáo")]
    public class ChiTietBaoCaoHoaDon : XPObject
    {
        public ChiTietBaoCaoHoaDon(Session session) : base(session) { }

        private BaoCaoDoanhThu baoCao;

        [Association("BaoCaoDoanhThu-HoaDons")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public BaoCaoDoanhThu BaoCao
        {
            get => baoCao;
            set => SetPropertyValue(nameof(BaoCao), ref baoCao, value);
        }

        private HoaDonBenhNhan hoaDon;

        [XafDisplayName("Hóa đơn")]
        public HoaDonBenhNhan HoaDon
        {
            get => hoaDon;
            set => SetPropertyValue(nameof(HoaDon), ref hoaDon, value);
        }

        private string maHoaDon;

        [Size(100)]
        [XafDisplayName("Mã hóa đơn")]
        public string MaHoaDon
        {
            get => maHoaDon;
            set => SetPropertyValue(nameof(MaHoaDon), ref maHoaDon, value);
        }

        private DateTime? ngayThanhToan;

        [XafDisplayName("Ngày thanh toán")]
        public DateTime? NgayThanhToan
        {
            get => ngayThanhToan;
            set => SetPropertyValue(nameof(NgayThanhToan), ref ngayThanhToan, value);
        }

        private string benhNhan;

        [Size(255)]
        [XafDisplayName("Bệnh nhân")]
        public string BenhNhan
        {
            get => benhNhan;
            set => SetPropertyValue(nameof(BenhNhan), ref benhNhan, value);
        }

        private string hinhThucThanhToan;

        [Size(100)]
        [XafDisplayName("Hình thức thanh toán")]
        public string HinhThucThanhToan
        {
            get => hinhThucThanhToan;
            set => SetPropertyValue(nameof(HinhThucThanhToan), ref hinhThucThanhToan, value);
        }

        private decimal tongTienThuoc;

        [XafDisplayName("Tiền thuốc")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal TongTienThuoc
        {
            get => tongTienThuoc;
            set => SetPropertyValue(nameof(TongTienThuoc), ref tongTienThuoc, value);
        }

        private decimal tienKham;

        [XafDisplayName("Tiền khám")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal TienKham
        {
            get => tienKham;
            set => SetPropertyValue(nameof(TienKham), ref tienKham, value);
        }

        private decimal giamGia;

        [XafDisplayName("Giảm giá")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal GiamGia
        {
            get => giamGia;
            set => SetPropertyValue(nameof(GiamGia), ref giamGia, value);
        }

        private decimal tongThanhToan;

        [XafDisplayName("Tổng thanh toán")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal TongThanhToan
        {
            get => tongThanhToan;
            set => SetPropertyValue(nameof(TongThanhToan), ref tongThanhToan, value);
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết phiếu nhập kho báo cáo")]
    public class ChiTietBaoCaoPhieuNhapKho : XPObject
    {
        public ChiTietBaoCaoPhieuNhapKho(Session session) : base(session) { }

        private BaoCaoDoanhThu baoCao;

        [Association("BaoCaoDoanhThu-PhieuNhaps")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public BaoCaoDoanhThu BaoCao
        {
            get => baoCao;
            set => SetPropertyValue(nameof(BaoCao), ref baoCao, value);
        }

        private PhieuNhapKho phieuNhap;

        [XafDisplayName("Phiếu nhập kho")]
        public PhieuNhapKho PhieuNhap
        {
            get => phieuNhap;
            set => SetPropertyValue(nameof(PhieuNhap), ref phieuNhap, value);
        }

        private string maPhieuNhap;

        [Size(100)]
        [XafDisplayName("Mã phiếu nhập")]
        public string MaPhieuNhap
        {
            get => maPhieuNhap;
            set => SetPropertyValue(nameof(MaPhieuNhap), ref maPhieuNhap, value);
        }

        private DateTime ngayNhap;

        [XafDisplayName("Ngày nhập")]
        public DateTime NgayNhap
        {
            get => ngayNhap;
            set => SetPropertyValue(nameof(NgayNhap), ref ngayNhap, value);
        }

        private string nhaCungCap;

        [Size(255)]
        [XafDisplayName("Nhà cung cấp")]
        public string NhaCungCap
        {
            get => nhaCungCap;
            set => SetPropertyValue(nameof(NhaCungCap), ref nhaCungCap, value);
        }

        private decimal tongTienNhap;

        [XafDisplayName("Tổng tiền nhập")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal TongTienNhap
        {
            get => tongTienNhap;
            set => SetPropertyValue(nameof(TongTienNhap), ref tongTienNhap, value);
        }
    }
}