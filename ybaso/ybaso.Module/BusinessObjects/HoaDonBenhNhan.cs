using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    public enum TrangThaiHoaDonBenhNhan
    {
        ChuaThanhToan = 0,
        DaThanhToan = 1,
        Huy = 2
    }

    public enum HinhThucThanhToanHoaDon
    {
        TienMat = 0,
        ChuyenKhoan = 1,
        Khac = 2
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Hóa đơn bệnh nhân")]
    [DefaultProperty(nameof(MaHoaDon))]
    public class HoaDonBenhNhan : XPObject
    {
        public HoaDonBenhNhan(Session session) : base(session) { }

        private string maHoaDon;
        private DateTime ngayLap;
        private DateTime? ngayThanhToan;
        private Benhnhan benhNhan;
        private Cakham caKham;
        private DonThuoc donThuoc;
        private PhieuXuatKho phieuXuatKho;
        private Nhanvien nhanVienLap;
        private decimal tongTienThuoc;
        private decimal tienKham;
        private decimal giamGia;
        private decimal tongThanhToan;
        private HinhThucThanhToanHoaDon hinhThucThanhToan;
        private TrangThaiHoaDonBenhNhan trangThai;
        private string ghiChu;

        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã hóa đơn")]
        [ModelDefault("AllowEdit", "False")]
        public string MaHoaDon
        {
            get => maHoaDon;
            set => SetPropertyValue(nameof(MaHoaDon), ref maHoaDon, value);
        }

        [XafDisplayName("Ngày lập")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime NgayLap
        {
            get => ngayLap;
            set => SetPropertyValue(nameof(NgayLap), ref ngayLap, value);
        }

        [XafDisplayName("Ngày thanh toán")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime? NgayThanhToan
        {
            get => ngayThanhToan;
            set => SetPropertyValue(nameof(NgayThanhToan), ref ngayThanhToan, value);
        }

        [XafDisplayName("Bệnh nhân")]
        [ModelDefault("AllowEdit", "False")]
        public Benhnhan BenhNhan
        {
            get => benhNhan;
            set => SetPropertyValue(nameof(BenhNhan), ref benhNhan, value);
        }

        [XafDisplayName("Ca khám")]
        [ModelDefault("AllowEdit", "False")]
        public Cakham CaKham
        {
            get => caKham;
            set => SetPropertyValue(nameof(CaKham), ref caKham, value);
        }

        [XafDisplayName("Đơn thuốc")]
        [ModelDefault("AllowEdit", "False")]
        public DonThuoc DonThuoc
        {
            get => donThuoc;
            set => SetPropertyValue(nameof(DonThuoc), ref donThuoc, value);
        }

        [XafDisplayName("Phiếu xuất kho")]
        [ModelDefault("AllowEdit", "False")]
        public PhieuXuatKho PhieuXuatKho
        {
            get => phieuXuatKho;
            set => SetPropertyValue(nameof(PhieuXuatKho), ref phieuXuatKho, value);
        }

        [XafDisplayName("Nhân viên lập")]
        [ModelDefault("AllowEdit", "False")]
        public Nhanvien NhanVienLap
        {
            get => nhanVienLap;
            set => SetPropertyValue(nameof(NhanVienLap), ref nhanVienLap, value);
        }

        [XafDisplayName("Tổng tiền thuốc")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongTienThuoc
        {
            get => tongTienThuoc;
            set
            {
                if (SetPropertyValue(nameof(TongTienThuoc), ref tongTienThuoc, value))
                {
                    if (!IsLoading)
                        CapNhatTongThanhToan();
                }
            }
        }

        [XafDisplayName("Tiền khám")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        public decimal TienKham
        {
            get => tienKham;
            set
            {
                if (SetPropertyValue(nameof(TienKham), ref tienKham, value))
                {
                    if (!IsLoading)
                        CapNhatTongThanhToan();
                }
            }
        }

        [XafDisplayName("Giảm giá")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        public decimal GiamGia
        {
            get => giamGia;
            set
            {
                if (SetPropertyValue(nameof(GiamGia), ref giamGia, value))
                {
                    if (!IsLoading)
                        CapNhatTongThanhToan();
                }
            }
        }

        [XafDisplayName("Tổng thanh toán")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongThanhToan
        {
            get => tongThanhToan;
            set => SetPropertyValue(nameof(TongThanhToan), ref tongThanhToan, value);
        }

        [XafDisplayName("Hình thức thanh toán")]
        public HinhThucThanhToanHoaDon HinhThucThanhToan
        {
            get => hinhThucThanhToan;
            set => SetPropertyValue(nameof(HinhThucThanhToan), ref hinhThucThanhToan, value);
        }

        [XafDisplayName("Trạng thái")]
        [ModelDefault("AllowEdit", "False")]
        public TrangThaiHoaDonBenhNhan TrangThai
        {
            get => trangThai;
            set => SetPropertyValue(nameof(TrangThai), ref trangThai, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Phiếu tính tiền")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [ModelDefault("AllowEdit", "False")]
        public string NoiDungHoaDonIn => TaoNoiDungHoaDonIn();

        [Association("HoaDonBenhNhan-ChiTietHoaDons")]
        [XpoAggregated]
        [XafDisplayName("Chi tiết hóa đơn")]
        public XPCollection<ChiTietHoaDonBenhNhan> ChiTietHoaDons
        {
            get => GetCollection<ChiTietHoaDonBenhNhan>(nameof(ChiTietHoaDons));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            MaHoaDon = TaoMaHoaDon();
            NgayLap = DateTime.Now;
            NgayThanhToan = DateTime.Now;
            TrangThai = TrangThaiHoaDonBenhNhan.DaThanhToan;
            HinhThucThanhToan = HinhThucThanhToanHoaDon.TienMat;
            TongTienThuoc = 0;
            TienKham = 0;
            GiamGia = 0;
            TongThanhToan = 0;
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                TongTienThuoc = ChiTietHoaDons
                    .Where(x => !x.IsDeleted)
                    .Sum(x => x.ThanhTien);

                CapNhatTongThanhToan();
            }

            base.OnSaving();
        }

        public void CapNhatTongThanhToan()
        {
            TongThanhToan = TongTienThuoc + TienKham - GiamGia;

            if (TongThanhToan < 0)
                TongThanhToan = 0;
        }

        private string TaoNoiDungHoaDonIn()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("PHÒNG KHÁM YBASO");
            sb.AppendLine("PHIẾU TÍNH TIỀN");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Mã HĐ: {MaHoaDon}");
            sb.AppendLine($"Ngày: {NgayThanhToan:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Bệnh nhân: {(BenhNhan == null ? "" : BenhNhan.HoTen)}");
            sb.AppendLine("--------------------------------");

            foreach (ChiTietHoaDonBenhNhan ct in ChiTietHoaDons.Where(x => !x.IsDeleted))
            {
                string tenThuoc = ct.Thuoc == null ? "" : ct.Thuoc.TenThuoc;
                sb.AppendLine(tenThuoc);
                sb.AppendLine($"  {ct.SoLuong:n0} x {ct.DonGia:n0} = {ct.ThanhTien:n0}");
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Tiền thuốc: {TongTienThuoc:n0}");
            sb.AppendLine($"Tiền khám : {TienKham:n0}");
            sb.AppendLine($"Giảm giá  : {GiamGia:n0}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"THANH TOÁN: {TongThanhToan:n0}");
            sb.AppendLine($"Thanh toán: {HinhThucThanhToan}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("Xin cảm ơn quý bệnh nhân!");

            return sb.ToString();
        }

        private string TaoMaHoaDon()
        {
            return $"HD{DateTime.Now:yyyyMMddHHmmss}";
        }

        public override string ToString()
        {
            return MaHoaDon;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết hóa đơn")]
    [DefaultProperty(nameof(Thuoc))]
    public class ChiTietHoaDonBenhNhan : XPObject
    {
        public ChiTietHoaDonBenhNhan(Session session) : base(session) { }

        private HoaDonBenhNhan hoaDon;
        private DanhMucThuoc thuoc;
        private decimal soLuong;
        private ListDvt donViTinh;
        private decimal donGia;
        private decimal thanhTien;
        private string ghiChu;

        [Association("HoaDonBenhNhan-ChiTietHoaDons")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public HoaDonBenhNhan HoaDon
        {
            get => hoaDon;
            set => SetPropertyValue(nameof(HoaDon), ref hoaDon, value);
        }

        [XafDisplayName("Thuốc/Thang thuốc")]
        public DanhMucThuoc Thuoc
        {
            get => thuoc;
            set => SetPropertyValue(nameof(Thuoc), ref thuoc, value);
        }

        [XafDisplayName("Số lượng")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        public decimal SoLuong
        {
            get => soLuong;
            set
            {
                if (SetPropertyValue(nameof(SoLuong), ref soLuong, value))
                {
                    if (!IsLoading)
                        CapNhatThanhTien();
                }
            }
        }

        [XafDisplayName("Đơn vị tính")]
        public ListDvt DonViTinh
        {
            get => donViTinh;
            set => SetPropertyValue(nameof(DonViTinh), ref donViTinh, value);
        }

        [XafDisplayName("Đơn giá")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        public decimal DonGia
        {
            get => donGia;
            set
            {
                if (SetPropertyValue(nameof(DonGia), ref donGia, value))
                {
                    if (!IsLoading)
                        CapNhatThanhTien();
                }
            }
        }

        [XafDisplayName("Thành tiền")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ModelDefault("AllowEdit", "False")]
        public decimal ThanhTien
        {
            get => thanhTien;
            set => SetPropertyValue(nameof(ThanhTien), ref thanhTien, value);
        }

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
                CapNhatThanhTien();

            base.OnSaving();
        }

        public void CapNhatThanhTien()
        {
            ThanhTien = SoLuong * DonGia;
        }

        public override string ToString()
        {
            if (Thuoc != null)
                return $"{Thuoc} - {SoLuong:n0}";

            return base.ToString();
        }
    }
}