using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    public enum TrangThaiDonThuoc
    {
        ChuaXuat = 0,
        DaXuat = 1,
        Huy = 2
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Đơn thuốc")]
    public class DonThuoc : XPObject
    {
        public DonThuoc(Session session) : base(session) { }

        private string maDonThuoc;
        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã đơn thuốc")]
        [ModelDefault("AllowEdit", "False")]
        public string MaDonThuoc
        {
            get => maDonThuoc;
            set => SetPropertyValue(nameof(MaDonThuoc), ref maDonThuoc, value);
        }

        private Cakham ca;
        [Association("Cakham-DonThuocs")]
        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn ca khám.")]
        [XafDisplayName("Ca khám")]
        [ImmediatePostData]
        [ModelDefault("AllowEdit", "False")]
        public Cakham Ca
        {
            get => ca;
            set
            {
                if (SetPropertyValue(nameof(Ca), ref ca, value))
                    CapNhatThongTinTuCaKham();
            }
        }

        private Benhnhan benhNhan;
        [XafDisplayName("Bệnh nhân")]
        [ModelDefault("AllowEdit", "False")]
        public Benhnhan BenhNhan
        {
            get => benhNhan;
            set => SetPropertyValue(nameof(BenhNhan), ref benhNhan, value);
        }

        private Nhanvien bacSi;
        [XafDisplayName("Bác sĩ")]
        [ModelDefault("AllowEdit", "False")]
        public Nhanvien BacSi
        {
            get => bacSi;
            set => SetPropertyValue(nameof(BacSi), ref bacSi, value);
        }

        private DateTime ngayKeDon;
        [XafDisplayName("Ngày kê đơn")]
        public DateTime NgayKeDon
        {
            get => ngayKeDon;
            set => SetPropertyValue(nameof(NgayKeDon), ref ngayKeDon, value);
        }

        private string chanDoan;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Chẩn đoán")]
        [ModelDefault("AllowEdit", "False")]
        public string ChanDoan
        {
            get => chanDoan;
            set => SetPropertyValue(nameof(ChanDoan), ref chanDoan, value);
        }


        private string ghiChu;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        private TrangThaiDonThuoc trangThai;
        [XafDisplayName("Trạng thái")]
        [ModelDefault("AllowEdit", "False")]
        public TrangThaiDonThuoc TrangThai
        {
            get => trangThai;
            set => SetPropertyValue(nameof(TrangThai), ref trangThai, value);
        }

        private decimal tongTien;
        [XafDisplayName("Tổng tiền dự kiến")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongTien
        {
            get => tongTien;
            set => SetPropertyValue(nameof(TongTien), ref tongTien, value);
        }

        [Association("DonThuoc-ChiTietDonThuocs")]
        [XpoAggregated]
        [XafDisplayName("Chi tiết đơn thuốc")]
        public XPCollection<ChiTietDonThuoc> ChiTietDonThuocs
        {
            get => GetCollection<ChiTietDonThuoc>(nameof(ChiTietDonThuocs));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            MaDonThuoc = TaoMaDonThuoc();
            NgayKeDon = DateTime.Now;
            TrangThai = TrangThaiDonThuoc.ChuaXuat;
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                CapNhatThongTinTuCaKham();
                KiemTraHopLe();
                CapNhatTongTien();
            }

            base.OnSaving();
        }

        public void CapNhatTongTien()
        {
            if (IsLoading || IsDeleted)
                return;

            try
            {
                TongTien = ChiTietDonThuocs
                    .Where(x => !x.IsDeleted)
                    .Sum(x => x.ThanhTien);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("GetObjectsNonReentrant"))
            {
                // Bỏ qua khi XPO đang load collection để tránh lỗi reentrant.
                // Khi lưu hoặc chỉnh chi tiết, tổng tiền sẽ được tính lại.
            }
        }
        private void KiemTraHopLe()
        {
            if (Ca == null)
                throw new UserFriendlyException("Đơn thuốc phải gắn với một ca khám.");

            if (TrangThai == TrangThaiDonThuoc.Huy)
                return;

            if (ChiTietDonThuocs.Count == 0)
                throw new UserFriendlyException("Đơn thuốc phải có ít nhất một dòng thuốc.");

            foreach (ChiTietDonThuoc dong in ChiTietDonThuocs)
            {
                if (dong.Thuoc == null)
                    throw new UserFriendlyException("Vui lòng chọn thuốc/thang thuốc trong chi tiết đơn thuốc.");
            }
        }

        private void CapNhatThongTinTuCaKham()
        {
            if (Ca == null)
                return;

            BenhNhan = Ca.BenhNhan;
            BacSi = Ca.Bacsi;

            if (string.IsNullOrWhiteSpace(ChanDoan))
            {
                var noiDungChanDoan = new[]
                {
            string.IsNullOrWhiteSpace(Ca.Amduong) ? null : $"Âm/Dương: {Ca.Amduong}",
            string.IsNullOrWhiteSpace(Ca.Huthuc) ? null : $"Hư/Thực: {Ca.Huthuc}",
            string.IsNullOrWhiteSpace(Ca.Khihuyet) ? null : $"Khí/Huyết: {Ca.Khihuyet}",
            string.IsNullOrWhiteSpace(Ca.Benhchung) ? null : $"Bệnh chứng: {Ca.Benhchung}"
        };

                ChanDoan = string.Join(
                    Environment.NewLine,
                    noiDungChanDoan.Where(x => !string.IsNullOrWhiteSpace(x))
                );
            }
        }

        private string TaoMaDonThuoc()
        {
            return $"DT{DateTime.Now:yyyyMMddHHmmss}";
        }

        public override string ToString()
        {
            if (BenhNhan != null)
                return $"{MaDonThuoc} - {BenhNhan}";

            return MaDonThuoc;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết đơn thuốc")]
    public class ChiTietDonThuoc : XPObject
    {
        public ChiTietDonThuoc(Session session) : base(session) { }

        private DonThuoc donThuoc;
        [Association("DonThuoc-ChiTietDonThuocs")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public DonThuoc DonThuoc
        {
            get => donThuoc;
            set => SetPropertyValue(nameof(DonThuoc), ref donThuoc, value);
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
                        DonGia = thuoc.GiaBan;
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
            "ChiTietDonThuoc_SoLuong_GreaterThanZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThan,
            0,
            CustomMessageTemplate = "Số lượng thuốc phải lớn hơn 0."
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

        private decimal donGia;

        [XafDisplayName("Đơn giá")]
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [RuleValueComparison(
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Đơn giá không hợp lệ."
        )]
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
        private decimal thanhTien;
        [XafDisplayName("Thành tiền")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal ThanhTien
        {
            get => thanhTien;
            set => SetPropertyValue(nameof(ThanhTien), ref thanhTien, value);
        }

        private string lieuDung;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Liều dùng/Cách dùng")]
        public string LieuDung
        {
            get => lieuDung;
            set => SetPropertyValue(nameof(LieuDung), ref lieuDung, value);
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
            CapNhatThanhTien();
            base.OnSaving();
        }

        private void CapNhatThanhTien()
        {
            if (IsLoading || IsDeleted)
                return;

            ThanhTien = SoLuong * DonGia;

            if (!IsSaving && DonThuoc != null)
                DonThuoc.CapNhatTongTien();
        }

        public override string ToString()
        {
            if (Thuoc != null)
                return $"{Thuoc} - {SoLuong}";

            return base.ToString();
        }
    }
}