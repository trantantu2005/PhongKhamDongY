using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    public enum TrangThaiPhieuXuatKho
    {
        NhapTam = 0,
        DaXuat = 1,
        Huy = 2
    }

    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.KhoDuoc)]
    [XafDisplayName("Phiếu xuất kho")]
    [DefaultProperty(nameof(MaPhieuXuat))]
    public class PhieuXuatKho : XPObject
    {
        public PhieuXuatKho(Session session) : base(session) { }

        private bool dangDongBoBenhNhanTuDonThuoc;

        private string maPhieuXuat;

        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã phiếu xuất")]
        [ModelDefault("AllowEdit", "False")]
        public string MaPhieuXuat
        {
            get => maPhieuXuat;
            set => SetPropertyValue(nameof(MaPhieuXuat), ref maPhieuXuat, value);
        }

        private Nhanvien nhanVien;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn nhân viên xuất kho.")]
        [XafDisplayName("Nhân viên")]
        public Nhanvien NhanVien
        {
            get => nhanVien;
            set => SetPropertyValue(nameof(NhanVien), ref nhanVien, value);
        }

        private string tuKhoaBenhNhan;

        [NonPersistent]
        [XafDisplayName("Tìm bệnh nhân")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [ModelDefault("NullText", "Nhập tên, số điện thoại, CCCD...")]
        public string TuKhoaBenhNhan
        {
            get => tuKhoaBenhNhan;
            set
            {
                if (SetPropertyValue(nameof(TuKhoaBenhNhan), ref tuKhoaBenhNhan, value))
                {
                    if (!IsLoading && !IsSaving)
                    {
                        BenhNhan = null;
                        DonThuoc = null;
                        CaKham = null;
                        XoaChiTietXuat();
                        TuDongChonBenhNhanNeuChiCoMotNguoi();
                    }
                }
            }
        }

        [Browsable(false)]
        public XPCollection<Benhnhan> BenhNhanChoNhanThuoc
        {
            get
            {
                XPCollection<Benhnhan> result = new XPCollection<Benhnhan>(Session);
                var danhSachBenhNhan = LayDanhSachBenhNhanChoNhanThuoc();

                if (BenhNhan != null && danhSachBenhNhan.All(x => x.Oid != BenhNhan.Oid))
                    danhSachBenhNhan.Add(BenhNhan);

                if (danhSachBenhNhan.Count == 0)
                {
                    result.Criteria = CriteriaOperator.Parse("Oid = ?", -1);
                    return result;
                }

                var ids = danhSachBenhNhan.Select(x => x.Oid).ToList();
                result.Criteria = new InOperator("Oid", ids);

                result.Sorting.Add(
                    new SortProperty(nameof(Benhnhan.Ten), DevExpress.Xpo.DB.SortingDirection.Ascending)
                );

                return result;
            }
        }

        private Benhnhan benhNhan;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn bệnh nhân.")]
        [XafDisplayName("Bệnh nhân")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [DataSourceProperty(nameof(BenhNhanChoNhanThuoc))]
        public Benhnhan BenhNhan
        {
            get => benhNhan;
            set
            {
                if (SetPropertyValue(nameof(BenhNhan), ref benhNhan, value))
                {
                    if (!dangDongBoBenhNhanTuDonThuoc && !IsLoading && !IsSaving)
                    {
                        DonThuoc = null;
                        CaKham = null;
                        XoaChiTietXuat();
                        TuDongChonDonThuocNeuChiCoMotDon();
                    }
                }
            }
        }

        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        [XafDisplayName("Bệnh nhân")]
        public string TenBenhNhan
        {
            get => BenhNhan == null ? "" : BenhNhan.HoTen;
        }

        [Browsable(false)]
        public XPCollection<DonThuoc> DonThuocChoXuatTheoBenhNhan
        {
            get
            {
                XPCollection<DonThuoc> result = new XPCollection<DonThuoc>(Session);

                if (BenhNhan == null)
                {
                    result.Criteria = CriteriaOperator.Parse("Oid = ?", -1);
                    return result;
                }

                result.Criteria = CriteriaOperator.Parse(
                    "TrangThai = ? And BenhNhan = ?",
                    TrangThaiDonThuoc.ChuaXuat,
                    BenhNhan
                );

                result.Sorting.Add(
                    new SortProperty(nameof(DonThuoc.NgayKeDon), DevExpress.Xpo.DB.SortingDirection.Descending)
                );

                return result;
            }
        }

        private DonThuoc donThuoc;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn đơn thuốc cần xuất kho.")]
        [XafDisplayName("Đơn thuốc")]
        [ImmediatePostData]
        [DataSourceProperty(nameof(DonThuocChoXuatTheoBenhNhan))]
        public DonThuoc DonThuoc
        {
            get => donThuoc;
            set
            {
                if (SetPropertyValue(nameof(DonThuoc), ref donThuoc, value))
                {
                    CapNhatThongTinTuDonThuoc();

                    if (!IsLoading && !IsSaving && !DaCapNhatTon && DonThuoc != null)
                    {
                        LayThuocTuDonThuoc();
                    }
                }
            }
        }

        private Cakham caKham;

        [XafDisplayName("Ca khám")]
        [ModelDefault("AllowEdit", "False")]
        public Cakham CaKham
        {
            get => caKham;
            set => SetPropertyValue(nameof(CaKham), ref caKham, value);
        }

        private DateTime ngayXuat;

        [XafDisplayName("Ngày xuất")]
        public DateTime NgayXuat
        {
            get => ngayXuat;
            set => SetPropertyValue(nameof(NgayXuat), ref ngayXuat, value);
        }

        private string ghiChu;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        private decimal tongTien;

        [XafDisplayName("Tổng tiền thuốc")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongTien
        {
            get => tongTien;
            set => SetPropertyValue(nameof(TongTien), ref tongTien, value);
        }

        private decimal tienKham;

        [XafDisplayName("Tiền khám")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ImmediatePostData]
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

        private decimal giamGia;

        [XafDisplayName("Giảm giá")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ImmediatePostData]
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

        private decimal tongThanhToan;

        [XafDisplayName("Tổng thanh toán")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongThanhToan
        {
            get => tongThanhToan;
            set => SetPropertyValue(nameof(TongThanhToan), ref tongThanhToan, value);
        }

        private HinhThucThanhToanHoaDon hinhThucThanhToan;

        [XafDisplayName("Hình thức thanh toán")]
        [ImmediatePostData]
        public HinhThucThanhToanHoaDon HinhThucThanhToan
        {
            get => hinhThucThanhToan;
            set => SetPropertyValue(nameof(HinhThucThanhToan), ref hinhThucThanhToan, value);
        }

        private HoaDonBenhNhan hoaDon;

        [XafDisplayName("Hóa đơn")]
        [ModelDefault("AllowEdit", "False")]
        public HoaDonBenhNhan HoaDon
        {
            get => hoaDon;
            set => SetPropertyValue(nameof(HoaDon), ref hoaDon, value);
        }

        private TrangThaiPhieuXuatKho trangThai;

        [XafDisplayName("Trạng thái")]
        [ModelDefault("AllowEdit", "False")]
        public TrangThaiPhieuXuatKho TrangThai
        {
            get => trangThai;
            set => SetPropertyValue(nameof(TrangThai), ref trangThai, value);
        }

        private bool daCapNhatTon;

        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public bool DaCapNhatTon
        {
            get => daCapNhatTon;
            set => SetPropertyValue(nameof(DaCapNhatTon), ref daCapNhatTon, value);
        }

        [XafDisplayName("Hóa đơn hiển thị cho bệnh nhân")]
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "False")]
        public string NoiDungHoaDon => TaoNoiDungHoaDon();

        [Association("PhieuXuatKho-ChiTietPhieuXuat")]
        [XpoAggregated]
        [XafDisplayName("Chi tiết xuất kho")]
        public XPCollection<ChiTietPhieuXuat> ChiTietPhieuXuats
        {
            get => GetCollection<ChiTietPhieuXuat>(nameof(ChiTietPhieuXuats));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            MaPhieuXuat = TaoMaPhieuXuat();
            NgayXuat = DateTime.Now;
            TrangThai = TrangThaiPhieuXuatKho.NhapTam;
            DaCapNhatTon = false;
            TienKham = 0;
            GiamGia = 0;
            TongThanhToan = 0;
            HinhThucThanhToan = HinhThucThanhToanHoaDon.TienMat;
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                CapNhatThongTinTuDonThuoc();
                CapNhatTongTien();

                if (TrangThai == TrangThaiPhieuXuatKho.DaXuat && !DaCapNhatTon)
                {
                    KiemTraHopLeTruocKhiXuat();
                    CapNhatTonKho();
                    DaCapNhatTon = true;

                    if (DonThuoc != null)
                        DonThuoc.TrangThai = TrangThaiDonThuoc.DaXuat;

                    TaoHoacCapNhatHoaDon();
                }
            }

            base.OnSaving();
        }

        [Action(
            Caption = "Lấy thuốc từ đơn thuốc",
            ImageName = "Action_Refresh",
            AutoCommit = false,
            TargetObjectsCriteria = "DaCapNhatTon = False"
        )]
        public void LayThuocTuDonThuoc()
        {
            if (DonThuoc == null)
                throw new UserFriendlyException("Vui lòng chọn đơn thuốc trước.");

            if (DaCapNhatTon)
                throw new UserFriendlyException("Phiếu đã xuất kho, không thể lấy lại dữ liệu từ đơn thuốc.");

            XoaChiTietXuat();

            foreach (ChiTietDonThuoc dongDon in DonThuoc.ChiTietDonThuocs.ToList())
            {
                var dongXuat = new ChiTietPhieuXuat(Session)
                {
                    PhieuXuat = this,
                    Thuoc = dongDon.Thuoc,
                    SoLuong = dongDon.SoLuong,
                    DonViTinh = dongDon.DonViTinh,
                    GiaXuat = dongDon.DonGia,
                    GhiChu = dongDon.LieuDung
                };

                dongXuat.CapNhatThanhTien();
                ChiTietPhieuXuats.Add(dongXuat);
            }

            CapNhatTongTien();
        }

        public void XacNhanXuatKho()
        {
            CapNhatThongTinTuDonThuoc();

            if (TrangThai == TrangThaiPhieuXuatKho.Huy)
                throw new UserFriendlyException("Phiếu xuất đã hủy, không thể xác nhận xuất kho.");

            if (DaCapNhatTon)
            {
                TrangThai = TrangThaiPhieuXuatKho.DaXuat;

                if (DonThuoc != null)
                    DonThuoc.TrangThai = TrangThaiDonThuoc.DaXuat;

                TaoHoacCapNhatHoaDon();
                return;
            }

            KiemTraHopLeTruocKhiXuat();

            TrangThai = TrangThaiPhieuXuatKho.DaXuat;
            CapNhatTongTien();
            CapNhatTonKho();
            DaCapNhatTon = true;

            if (DonThuoc != null)
                DonThuoc.TrangThai = TrangThaiDonThuoc.DaXuat;

            TaoHoacCapNhatHoaDon();
        }

        [Action(Caption = "Hủy phiếu xuất", ImageName = "Action_Deny", AutoCommit = true)]
        public void HuyPhieuXuat()
        {
            if (DaCapNhatTon)
                throw new UserFriendlyException("Phiếu xuất đã trừ tồn kho, không thể hủy trực tiếp.");

            TrangThai = TrangThaiPhieuXuatKho.Huy;
        }

        private void TaoHoacCapNhatHoaDon()
        {
            CapNhatTongTien();

            HoaDonBenhNhan hoaDonCanLuu = HoaDon;

            if (hoaDonCanLuu == null)
            {
                hoaDonCanLuu = new XPQuery<HoaDonBenhNhan>(Session)
                    .FirstOrDefault(x => x.PhieuXuatKho == this);
            }

            if (hoaDonCanLuu == null)
            {
                hoaDonCanLuu = new HoaDonBenhNhan(Session);
                hoaDonCanLuu.MaHoaDon = TaoMaHoaDonTuPhieuXuat();
                hoaDonCanLuu.NgayLap = DateTime.Now;
            }

            hoaDonCanLuu.PhieuXuatKho = this;
            hoaDonCanLuu.BenhNhan = BenhNhan;
            hoaDonCanLuu.CaKham = CaKham;
            hoaDonCanLuu.DonThuoc = DonThuoc;
            hoaDonCanLuu.NhanVienLap = NhanVien;
            hoaDonCanLuu.NgayThanhToan = DateTime.Now;
            hoaDonCanLuu.TrangThai = TrangThaiHoaDonBenhNhan.DaThanhToan;
            hoaDonCanLuu.HinhThucThanhToan = HinhThucThanhToan;
            hoaDonCanLuu.TienKham = TienKham;
            hoaDonCanLuu.GiamGia = GiamGia;
            hoaDonCanLuu.TongTienThuoc = TongTien;
            hoaDonCanLuu.GhiChu = $"Hóa đơn tạo tự động từ phiếu xuất kho {MaPhieuXuat}.";

            foreach (var item in hoaDonCanLuu.ChiTietHoaDons.ToList())
                item.Delete();

            foreach (ChiTietPhieuXuat dongXuat in ChiTietPhieuXuats.Where(x => !x.IsDeleted))
            {
                var chiTietHoaDon = new ChiTietHoaDonBenhNhan(Session)
                {
                    HoaDon = hoaDonCanLuu,
                    Thuoc = dongXuat.Thuoc,
                    SoLuong = dongXuat.SoLuong,
                    DonViTinh = dongXuat.DonViTinh,
                    DonGia = dongXuat.GiaXuat,
                    GhiChu = dongXuat.GhiChu
                };

                chiTietHoaDon.CapNhatThanhTien();
                hoaDonCanLuu.ChiTietHoaDons.Add(chiTietHoaDon);
            }

            hoaDonCanLuu.CapNhatTongThanhToan();
            HoaDon = hoaDonCanLuu;
        }

        private string TaoNoiDungHoaDon()
        {
            CapNhatTongTien();

            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Mã phiếu xuất: {MaPhieuXuat}");
            builder.AppendLine($"Bệnh nhân: {(BenhNhan == null ? "" : BenhNhan.HoTen)}");
            builder.AppendLine($"Ngày xuất: {NgayXuat:dd/MM/yyyy HH:mm}");
            builder.AppendLine("--------------------------------");

            foreach (ChiTietPhieuXuat dong in ChiTietPhieuXuats.Where(x => !x.IsDeleted))
            {
                builder.AppendLine(
                    $"{dong.Thuoc} - SL: {dong.SoLuong:n0} - Đơn giá: {dong.GiaXuat:n0} - Thành tiền: {dong.ThanhTien:n0}"
                );
            }

            builder.AppendLine("--------------------------------");
            builder.AppendLine($"Tổng tiền thuốc: {TongTien:n0}");
            builder.AppendLine($"Tiền khám: {TienKham:n0}");
            builder.AppendLine($"Giảm giá: {GiamGia:n0}");
            builder.AppendLine($"Tổng thanh toán: {TongThanhToan:n0}");
            builder.AppendLine($"Hình thức thanh toán: {HinhThucThanhToan}");

            return builder.ToString();
        }

        private void CapNhatThongTinTuDonThuoc()
        {
            if (DonThuoc == null)
                return;

            CaKham = DonThuoc.Ca;

            Benhnhan benhNhanTuDon = DonThuoc.BenhNhan;

            if (benhNhanTuDon == null && DonThuoc.Ca != null)
                benhNhanTuDon = DonThuoc.Ca.BenhNhan;

            if (benhNhanTuDon != null && BenhNhan != benhNhanTuDon)
            {
                dangDongBoBenhNhanTuDonThuoc = true;

                try
                {
                    BenhNhan = benhNhanTuDon;
                }
                finally
                {
                    dangDongBoBenhNhanTuDonThuoc = false;
                }
            }
        }

        private List<Benhnhan> LayDanhSachBenhNhanChoNhanThuoc()
        {
            string tuKhoa = ChuanHoaTuKhoa(TuKhoaBenhNhan);

            var danhSachBenhNhan = new XPQuery<DonThuoc>(Session)
                .Where(x =>
                    x.TrangThai == TrangThaiDonThuoc.ChuaXuat &&
                    x.BenhNhan != null
                )
                .Select(x => x.BenhNhan)
                .ToList()
                .GroupBy(x => x.Oid)
                .Select(x => x.First())
                .ToList();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                danhSachBenhNhan = danhSachBenhNhan
                    .Where(x =>
                        ChuanHoaTuKhoa(x.ThongTinTimKiem).Contains(tuKhoa) ||
                        ChuanHoaTuKhoa(x.TuKhoaTimKiem).Contains(tuKhoa) ||
                        ChuanHoaTuKhoa(x.HoTen).Contains(tuKhoa) ||
                        ChuanHoaTuKhoa(x.Dienthoai).Contains(tuKhoa) ||
                        ChuanHoaTuKhoa(x.SoCMND).Contains(tuKhoa)
                    )
                    .ToList();
            }

            return danhSachBenhNhan;
        }

        private void TuDongChonBenhNhanNeuChiCoMotNguoi()
        {
            var danhSachBenhNhan = LayDanhSachBenhNhanChoNhanThuoc();

            if (danhSachBenhNhan.Count == 1)
                BenhNhan = danhSachBenhNhan.First();
        }

        private void XoaChiTietXuat()
        {
            foreach (var item in ChiTietPhieuXuats.ToList())
                item.Delete();

            CapNhatTongTien();
        }

        private void TuDongChonDonThuocNeuChiCoMotDon()
        {
            if (BenhNhan == null)
                return;

            var danhSachDon = new XPQuery<DonThuoc>(Session)
                .Where(x =>
                    x.BenhNhan == BenhNhan &&
                    x.TrangThai == TrangThaiDonThuoc.ChuaXuat
                )
                .OrderByDescending(x => x.NgayKeDon)
                .ToList();

            if (danhSachDon.Count == 1)
                DonThuoc = danhSachDon.First();
        }

        public void CapNhatTongTien()
        {
            if (IsLoading || IsDeleted)
                return;

            try
            {
                TongTien = ChiTietPhieuXuats
                    .Where(x => !x.IsDeleted)
                    .Sum(x => x.ThanhTien);

                CapNhatTongThanhToan();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("GetObjectsNonReentrant"))
            {
                // Khi XPO đang load collection thì bỏ qua.
            }
        }

        public void CapNhatTongThanhToan()
        {
            TongThanhToan = TongTien + TienKham - GiamGia;

            if (TongThanhToan < 0)
                TongThanhToan = 0;
        }

        private void KiemTraHopLeTruocKhiXuat()
        {
            CapNhatThongTinTuDonThuoc();

            if (BenhNhan == null)
                throw new UserFriendlyException("Vui lòng chọn bệnh nhân.");

            if (DonThuoc == null)
                throw new UserFriendlyException("Phiếu xuất kho phải gắn với một đơn thuốc.");

            if (DonThuoc.TrangThai == TrangThaiDonThuoc.DaXuat && !DaCapNhatTon)
                throw new UserFriendlyException("Đơn thuốc này đã được xuất kho trước đó.");

            if (TienKham < 0)
                throw new UserFriendlyException("Tiền khám không hợp lệ.");

            if (GiamGia < 0)
                throw new UserFriendlyException("Giảm giá không hợp lệ.");

            if (ChiTietPhieuXuats.Count == 0)
                throw new UserFriendlyException("Phiếu xuất phải có ít nhất một dòng chi tiết.");

            foreach (ChiTietPhieuXuat dong in ChiTietPhieuXuats)
            {
                if (dong.Thuoc == null)
                    throw new UserFriendlyException("Vui lòng chọn thuốc/thang thuốc cho tất cả dòng chi tiết.");

                if (dong.SoLuong <= 0)
                    throw new UserFriendlyException("Số lượng xuất phải lớn hơn 0.");

                if (dong.GiaXuat < 0)
                    throw new UserFriendlyException("Giá xuất không hợp lệ.");

                if (dong.Thuoc.SoLuongTon < dong.SoLuong)
                {
                    throw new UserFriendlyException(
                        $"Thuốc/thang thuốc '{dong.Thuoc.TenThuoc}' không đủ tồn kho. " +
                        $"Tồn hiện tại: {dong.Thuoc.SoLuongTon}, cần xuất: {dong.SoLuong}."
                    );
                }
            }
        }

        private void CapNhatTonKho()
        {
            foreach (ChiTietPhieuXuat dong in ChiTietPhieuXuats)
                dong.Thuoc.SoLuongTon -= dong.SoLuong;
        }

        private string TaoMaPhieuXuat()
        {
            return $"PX{DateTime.Now:yyyyMMddHHmmss}";
        }

        private string TaoMaHoaDonTuPhieuXuat()
        {
            return $"HD{DateTime.Now:yyyyMMddHHmmss}";
        }

        private static string ChuanHoaTuKhoa(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Trim().ToLowerInvariant();
            text = text.Replace("đ", "d");

            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        public override string ToString()
        {
            return MaPhieuXuat;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết phiếu xuất")]
    [DefaultProperty(nameof(Thuoc))]
    public class ChiTietPhieuXuat : XPObject
    {
        public ChiTietPhieuXuat(Session session) : base(session) { }

        private PhieuXuatKho phieuXuat;

        [Association("PhieuXuatKho-ChiTietPhieuXuat")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuXuatKho PhieuXuat
        {
            get => phieuXuat;
            set => SetPropertyValue(nameof(PhieuXuat), ref phieuXuat, value);
        }

        private DanhMucThuoc thuoc;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn thuốc/thang thuốc.")]
        [XafDisplayName("Thuốc/Thang thuốc")]
        [ImmediatePostData]
        public DanhMucThuoc Thuoc
        {
            get => thuoc;
            set
            {
                if (SetPropertyValue(nameof(Thuoc), ref thuoc, value))
                {
                    if (!IsLoading && thuoc != null)
                    {
                        DonViTinh = thuoc.DonViTinh;
                        GiaXuat = thuoc.GiaBan;
                    }

                    if (!IsLoading)
                        CapNhatThanhTien();
                }
            }
        }

        private decimal soLuong;

        [XafDisplayName("Số lượng")]
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "ChiTietPhieuXuat_SoLuong_GreaterThanZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThan,
            0,
            CustomMessageTemplate = "Số lượng xuất phải lớn hơn 0."
        )]
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

        private ListDvt donViTinh;

        [XafDisplayName("Đơn vị tính")]
        public ListDvt DonViTinh
        {
            get => donViTinh;
            set => SetPropertyValue(nameof(DonViTinh), ref donViTinh, value);
        }

        private decimal giaXuat;

        [XafDisplayName("Giá xuất")]
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "ChiTietPhieuXuat_GiaXuat_GreaterThanOrEqualZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Giá xuất không hợp lệ."
        )]
        public decimal GiaXuat
        {
            get => giaXuat;
            set
            {
                if (SetPropertyValue(nameof(GiaXuat), ref giaXuat, value))
                {
                    if (!IsLoading)
                        CapNhatThanhTien();
                }
            }
        }

        private decimal thanhTien;

        [XafDisplayName("Thành tiền")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [ModelDefault("AllowEdit", "False")]
        public decimal ThanhTien
        {
            get => thanhTien;
            set => SetPropertyValue(nameof(ThanhTien), ref thanhTien, value);
        }

        private string ghiChu;

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
            if (IsLoading || IsDeleted)
                return;

            ThanhTien = SoLuong * GiaXuat;

            if (!IsSaving && PhieuXuat != null)
                PhieuXuat.CapNhatTongTien();
        }

        public override string ToString()
        {
            if (Thuoc != null)
                return $"{Thuoc} - {SoLuong}";

            return base.ToString();
        }
    }
}