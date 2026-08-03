using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    public enum TrangThaiPhieuNhapKho
    {
        NhapTam = 0,
        DaNhap = 1,
        Huy = 2
    }

    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.KhoDuoc)]
    [XafDisplayName("Phiếu nhập kho")]
    public class PhieuNhapKho : XPObject
    {
        public PhieuNhapKho(Session session) : base(session) { }

        private string maPhieuNhap;

        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã phiếu nhập")]
        [ModelDefault("AllowEdit", "False")]
        public string MaPhieuNhap
        {
            get => maPhieuNhap;
            set => SetPropertyValue(nameof(MaPhieuNhap), ref maPhieuNhap, value);
        }

        private Nhanvien nhanVien;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn nhân viên nhập kho.")]
        [XafDisplayName("Nhân viên")]
        public Nhanvien NhanVien
        {
            get => nhanVien;
            set => SetPropertyValue(nameof(NhanVien), ref nhanVien, value);
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

        private string ghiChu;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        private decimal tongTien;

        [XafDisplayName("Tổng tiền")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongTien
        {
            get => tongTien;
            set => SetPropertyValue(nameof(TongTien), ref tongTien, value);
        }

        private TrangThaiPhieuNhapKho trangThai;

        [XafDisplayName("Trạng thái")]
        public TrangThaiPhieuNhapKho TrangThai
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

        [Association("PhieuNhapKho-ChiTietPhieuNhap")]
        [XpoAggregated]
        [XafDisplayName("Chi tiết nhập kho")]
        public XPCollection<ChiTietPhieuNhap> ChiTietPhieuNhaps
        {
            get => GetCollection<ChiTietPhieuNhap>(nameof(ChiTietPhieuNhaps));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            NgayNhap = DateTime.Now;
            TrangThai = TrangThaiPhieuNhapKho.NhapTam;
            DaCapNhatTon = false;
            MaPhieuNhap = TaoMaPhieuNhap();
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                CapNhatTongTien();

                if (TrangThai == TrangThaiPhieuNhapKho.DaNhap && !DaCapNhatTon)
                {
                    KiemTraHopLeTruocKhiNhap();
                    CapNhatTonKho();
                    DaCapNhatTon = true;
                }
            }

            base.OnSaving();
        }

        [Action(Caption = "Xác nhận nhập kho", ImageName = "Action_Grant", AutoCommit = true)]
        public void XacNhanNhapKho()
        {
            if (TrangThai == TrangThaiPhieuNhapKho.Huy)
                throw new UserFriendlyException("Phiếu nhập đã hủy, không thể xác nhận nhập kho.");

            if (DaCapNhatTon)
                throw new UserFriendlyException("Phiếu nhập này đã được cập nhật tồn kho trước đó.");

            KiemTraHopLeTruocKhiNhap();

            TrangThai = TrangThaiPhieuNhapKho.DaNhap;
            CapNhatTongTien();
            CapNhatTonKho();
            DaCapNhatTon = true;
        }

        [Action(Caption = "Hủy phiếu nhập", ImageName = "Action_Deny", AutoCommit = true)]
        public void HuyPhieuNhap()
        {
            if (DaCapNhatTon)
                throw new UserFriendlyException("Phiếu nhập đã cập nhật tồn kho, không thể hủy trực tiếp.");

            TrangThai = TrangThaiPhieuNhapKho.Huy;
        }

        public void CapNhatTongTien()
        {
            if (IsLoading || IsDeleted)
                return;

            try
            {
                TongTien = ChiTietPhieuNhaps
                    .Where(x => !x.IsDeleted)
                    .Sum(x => x.ThanhTien);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("GetObjectsNonReentrant"))
            {
                // Bỏ qua khi XPO đang load collection.
            }
        }

        private void KiemTraHopLeTruocKhiNhap()
        {
            if (ChiTietPhieuNhaps.Count == 0)
                throw new UserFriendlyException("Phiếu nhập phải có ít nhất một dòng chi tiết.");

            foreach (ChiTietPhieuNhap dong in ChiTietPhieuNhaps)
            {
                if (dong.DuocLieu == null)
                    throw new UserFriendlyException("Vui lòng chọn dược liệu cho tất cả dòng chi tiết.");

                if (dong.SoLuong <= 0)
                    throw new UserFriendlyException("Số lượng nhập phải lớn hơn 0.");

                if (dong.GiaNhap < 0)
                    throw new UserFriendlyException("Giá nhập không hợp lệ.");
            }
        }

        private void CapNhatTonKho()
        {
            foreach (ChiTietPhieuNhap dong in ChiTietPhieuNhaps)
            {
                dong.DuocLieu.SoLuongTon += dong.SoLuong;
                dong.DuocLieu.GiaNhapGanNhat = dong.GiaNhap;
            }
        }

        private string TaoMaPhieuNhap()
        {
            return $"PN{DateTime.Now:yyyyMMddHHmmss}";
        }

        public override string ToString()
        {
            return MaPhieuNhap;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết phiếu nhập")]
    [DefaultProperty(nameof(DuocLieu))]
    public class ChiTietPhieuNhap : XPObject
    {
        public ChiTietPhieuNhap(Session session) : base(session) { }

        private PhieuNhapKho phieuNhap;

        [Association("PhieuNhapKho-ChiTietPhieuNhap")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuNhapKho PhieuNhap
        {
            get => phieuNhap;
            set => SetPropertyValue(nameof(PhieuNhap), ref phieuNhap, value);
        }

        private DuocLieu duocLieu;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn dược liệu.")]
        [XafDisplayName("Dược liệu")]
        [ImmediatePostData]
        public DuocLieu DuocLieu
        {
            get => duocLieu;
            set
            {
                if (SetPropertyValue(nameof(DuocLieu), ref duocLieu, value))
                {
                    if (!IsLoading && duocLieu != null)
                    {
                        DonViTinh = duocLieu.DonViTinh;
                        GiaNhap = duocLieu.GiaNhapGanNhat;
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
            "ChiTietPhieuNhap_SoLuong_GreaterThanZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThan,
            0,
            CustomMessageTemplate = "Số lượng nhập phải lớn hơn 0."
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

        private decimal giaNhap;

        [XafDisplayName("Giá nhập")]
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [RuleValueComparison(
            "ChiTietPhieuNhap_GiaNhap_GreaterThanOrEqualZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Giá nhập không hợp lệ."
        )]
        public decimal GiaNhap
        {
            get => giaNhap;
            set
            {
                if (SetPropertyValue(nameof(GiaNhap), ref giaNhap, value))
                {
                    if (!IsLoading)
                        CapNhatThanhTien();
                }
            }
        }

        private decimal thanhTien;

        [XafDisplayName("Thành tiền")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal ThanhTien
        {
            get => thanhTien;
            set => SetPropertyValue(nameof(ThanhTien), ref thanhTien, value);
        }

        private string soLo;

        [Size(100)]
        [XafDisplayName("Số lô")]
        public string SoLo
        {
            get => soLo;
            set => SetPropertyValue(nameof(SoLo), ref soLo, value);
        }

        private DateTime? hanSuDung;

        [XafDisplayName("Hạn sử dụng")]
        public DateTime? HanSuDung
        {
            get => hanSuDung;
            set => SetPropertyValue(nameof(HanSuDung), ref hanSuDung, value);
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

            ThanhTien = SoLuong * GiaNhap;

            if (!IsSaving && PhieuNhap != null)
                PhieuNhap.CapNhatTongTien();
        }

        public override string ToString()
        {
            return DuocLieu == null ? base.ToString() : DuocLieu.ToString();
        }
    }
}