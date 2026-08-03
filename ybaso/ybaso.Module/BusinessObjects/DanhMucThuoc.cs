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
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.KhoDuoc)]
    [XafDisplayName("Danh mục thuốc")]
    [DefaultProperty(nameof(TenThuoc))]
    public class DanhMucThuoc : XPObject
    {
        public DanhMucThuoc(Session session) : base(session) { }

        private string maThuoc;

        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã thuốc")]
        public string MaThuoc
        {
            get => maThuoc;
            set => SetPropertyValue(nameof(MaThuoc), ref maThuoc, value);
        }

        private string tenThuoc;

        [Size(255)]
        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng nhập tên thuốc.")]
        [XafDisplayName("Tên thuốc")]
        public string TenThuoc
        {
            get => tenThuoc;
            set => SetPropertyValue(nameof(TenThuoc), ref tenThuoc, value);
        }

        private ListDvt donViTinh;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn đơn vị tính.")]
        [XafDisplayName("Đơn vị tính")]
        public ListDvt DonViTinh
        {
            get => donViTinh;
            set => SetPropertyValue(nameof(DonViTinh), ref donViTinh, value);
        }

        private decimal soLuongBoSung;

        [NonPersistent]
        [XafDisplayName("Số lượng bổ sung")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        public decimal SoLuongBoSung
        {
            get => soLuongBoSung;
            set => SetPropertyValue(nameof(SoLuongBoSung), ref soLuongBoSung, value);
        }

        private decimal soLuongTon;

        [XafDisplayName("Số lượng tồn")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "DanhMucThuoc_SoLuongTon_GreaterThanOrEqualZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Số lượng tồn không được nhỏ hơn 0."
        )]
        public decimal SoLuongTon
        {
            get => soLuongTon;
            set => SetPropertyValue(nameof(SoLuongTon), ref soLuongTon, value);
        }

        private decimal tonToiThieu;

        [XafDisplayName("Tồn tối thiểu")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "DanhMucThuoc_TonToiThieu_GreaterThanOrEqualZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Tồn tối thiểu không được nhỏ hơn 0."
        )]
        public decimal TonToiThieu
        {
            get => tonToiThieu;
            set => SetPropertyValue(nameof(TonToiThieu), ref tonToiThieu, value);
        }

        private decimal giaNhapGanNhat;

        // Giữ lại thuộc tính cũ để không lỗi database cũ.
        // Theo nghiệp vụ mới, giá nhập thuộc về Dược liệu nên không hiển thị ở Danh mục thuốc.
        [Browsable(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Giá nhập gần nhất")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        public decimal GiaNhapGanNhat
        {
            get => giaNhapGanNhat;
            set => SetPropertyValue(nameof(GiaNhapGanNhat), ref giaNhapGanNhat, value);
        }

        private decimal giaBan;

        [XafDisplayName("Giá bán")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "DanhMucThuoc_GiaBan_GreaterThanOrEqualZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Giá bán không hợp lệ."
        )]
        public decimal GiaBan
        {
            get => giaBan;
            set => SetPropertyValue(nameof(GiaBan), ref giaBan, value);
        }

        private bool tuDongTinhGiaBan;

        [XafDisplayName("Tự động tính giá bán từ dược liệu")]
        [VisibleInListView(false)]
        public bool TuDongTinhGiaBan
        {
            get => tuDongTinhGiaBan;
            set => SetPropertyValue(nameof(TuDongTinhGiaBan), ref tuDongTinhGiaBan, value);
        }

        [XafDisplayName("Giá tính từ dược liệu")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        [VisibleInListView(false)]
        public decimal GiaTinhTuDuocLieu
        {
            get => TinhGiaTuThanhPhan();
        }

        private bool daTruDuocLieuKhiTao;

        // Dùng để chống trừ dược liệu nhiều lần.
        // Khi tạo thang thuốc mới và lưu thành công lần đầu, hệ thống trừ dược liệu rồi bật cờ này.
        [Browsable(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public bool DaTruDuocLieuKhiTao
        {
            get => daTruDuocLieuKhiTao;
            set => SetPropertyValue(nameof(DaTruDuocLieuKhiTao), ref daTruDuocLieuKhiTao, value);
        }

        private string moTa;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Mô tả")]
        public string MoTa
        {
            get => moTa;
            set => SetPropertyValue(nameof(MoTa), ref moTa, value);
        }

        private bool suDung = true;

        [XafDisplayName("Sử dụng")]
        public bool SuDung
        {
            get => suDung;
            set => SetPropertyValue(nameof(SuDung), ref suDung, value);
        }

        [Association("DanhMucThuoc-ThanhPhanThuocs")]
        [XpoAggregated]
        [XafDisplayName("Thành phần dược liệu")]
        public XPCollection<ThanhPhanThuoc> ThanhPhanThuocs
        {
            get => GetCollection<ThanhPhanThuoc>(nameof(ThanhPhanThuocs));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            SuDung = true;
            TuDongTinhGiaBan = true;
            SoLuongTon = 0;
            TonToiThieu = 0;
            GiaNhapGanNhat = 0;
            GiaBan = 0;
            DaTruDuocLieuKhiTao = false;
            SoLuongBoSung = 0;
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                // Chỉ tính lại giá bán.
                // Không throw lỗi và không trừ dược liệu ở đây,
                // vì nếu throw trong OnSaving thì Blazor dễ bị Application Error.
                CapNhatGiaBanTuDuocLieuNeuCan();
            }

            base.OnSaving();
        }

        [Action(Caption = "Tính lại giá bán", ImageName = "Action_Refresh", AutoCommit = true)]
        public void TinhLaiGiaBan()
        {
            decimal giaTinhTuDuocLieu = TinhGiaTuThanhPhan();

            if (giaTinhTuDuocLieu <= 0)
                throw new UserFriendlyException("Chưa có thành phần dược liệu hoặc giá dược liệu chưa hợp lệ để tính giá bán.");

            GiaBan = giaTinhTuDuocLieu;
        }

        private void CapNhatGiaBanTuDuocLieuNeuCan()
        {
            if (!TuDongTinhGiaBan)
                return;

            decimal giaTinhTuDuocLieu = TinhGiaTuThanhPhan();

            if (giaTinhTuDuocLieu > 0)
                GiaBan = giaTinhTuDuocLieu;
        }

        public decimal TinhGiaTuThanhPhan()
        {
            if (IsLoading || IsDeleted)
                return 0;

            try
            {
                return ThanhPhanThuocs
                    .Where(x => !x.IsDeleted && x.DuocLieu != null)
                    .Sum(x => x.SoLuong * x.GiaDuocLieu);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("GetObjectsNonReentrant"))
            {
                return 0;
            }
        }

        private bool CoThanhPhanDuocLieu()
        {
            try
            {
                return ThanhPhanThuocs.Any(x => !x.IsDeleted && x.DuocLieu != null);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("GetObjectsNonReentrant"))
            {
                return false;
            }
        }

        private void KiemTraThanhPhanHopLe()
        {
            if (!CoThanhPhanDuocLieu())
                throw new UserFriendlyException("Vui lòng khai báo thành phần dược liệu trước khi tạo tồn thang thuốc.");

            foreach (ThanhPhanThuoc thanhPhan in ThanhPhanThuocs.Where(x => !x.IsDeleted))
            {
                if (thanhPhan.DuocLieu == null)
                    throw new UserFriendlyException("Vui lòng chọn dược liệu cho tất cả dòng thành phần.");

                if (thanhPhan.SoLuong <= 0)
                    throw new UserFriendlyException("Số lượng dược liệu trong thành phần phải lớn hơn 0.");
            }
        }

        private void KiemTraDuocLieuDuTon(decimal soLuongThuocCanTao)
        {
            foreach (ThanhPhanThuoc thanhPhan in ThanhPhanThuocs.Where(x => !x.IsDeleted && x.DuocLieu != null))
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * soLuongThuocCanTao;

                if (thanhPhan.DuocLieu.SoLuongTon < soLuongCanDung)
                {
                    throw new UserFriendlyException(
                        $"Dược liệu '{thanhPhan.DuocLieu.TenDuocLieu}' không đủ tồn kho. " +
                        $"Cần {soLuongCanDung:n0}, hiện còn {thanhPhan.DuocLieu.SoLuongTon:n0}."
                    );
                }
            }
        }

        private void TruDuocLieuTheoSoLuongThuoc(decimal soLuongThuocCanTao)
        {
            foreach (ThanhPhanThuoc thanhPhan in ThanhPhanThuocs.Where(x => !x.IsDeleted && x.DuocLieu != null))
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * soLuongThuocCanTao;
                thanhPhan.DuocLieu.SoLuongTon -= soLuongCanDung;
            }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(TenThuoc)
                ? base.ToString()
                : TenThuoc;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Thành phần thuốc")]
    [DefaultProperty(nameof(DuocLieu))]
    public class ThanhPhanThuoc : XPObject
    {
        public ThanhPhanThuoc(Session session) : base(session) { }

        private DanhMucThuoc thuoc;

        [Association("DanhMucThuoc-ThanhPhanThuocs")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public DanhMucThuoc Thuoc
        {
            get => thuoc;
            set => SetPropertyValue(nameof(Thuoc), ref thuoc, value);
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
                    }

                    if (!IsLoading)
                    {
                        CapNhatThanhTien();
                    }
                }
            }
        }

        private decimal soLuong;

        [XafDisplayName("Số lượng dùng")]
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "ThanhPhanThuoc_SoLuong_GreaterThanZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThan,
            0,
            CustomMessageTemplate = "Số lượng dược liệu phải lớn hơn 0."
        )]
        public decimal SoLuong
        {
            get => soLuong;
            set
            {
                if (SetPropertyValue(nameof(SoLuong), ref soLuong, value))
                {
                    if (!IsLoading)
                    {
                        CapNhatThanhTien();
                    }
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

        [XafDisplayName("Giá dược liệu")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal GiaDuocLieu
        {
            get => DuocLieu == null ? 0 : DuocLieu.GiaNhapGanNhat;
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
            {
                CapNhatThanhTien();
            }

            base.OnSaving();
        }

        public void CapNhatThanhTien()
        {
            if (IsLoading || IsDeleted)
                return;

            ThanhTien = SoLuong * GiaDuocLieu;

            if (!IsSaving && Thuoc != null && Thuoc.TuDongTinhGiaBan)
            {
                Thuoc.GiaBan = Thuoc.TinhGiaTuThanhPhan();
            }
        }

        public override string ToString()
        {
            if (DuocLieu != null)
                return $"{DuocLieu} - {SoLuong:n0}";

            return base.ToString();
        }
    }
}