using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.KhoDuoc)]
    [XafDisplayName("Dược liệu")]
    [DefaultProperty(nameof(TenDuocLieu))]
    public class DuocLieu : XPObject
    {
        public DuocLieu(Session session) : base(session) { }

        private string maDuocLieu;

        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã dược liệu")]
        public string MaDuocLieu
        {
            get => maDuocLieu;
            set => SetPropertyValue(nameof(MaDuocLieu), ref maDuocLieu, value);
        }

        private string tenDuocLieu;

        [Size(255)]
        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng nhập tên dược liệu.")]
        [XafDisplayName("Tên dược liệu")]
        public string TenDuocLieu
        {
            get => tenDuocLieu;
            set => SetPropertyValue(nameof(TenDuocLieu), ref tenDuocLieu, value);
        }

        private ListDvt donViTinh;

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng chọn đơn vị tính.")]
        [XafDisplayName("Đơn vị tính")]
        public ListDvt DonViTinh
        {
            get => donViTinh;
            set => SetPropertyValue(nameof(DonViTinh), ref donViTinh, value);
        }

        private decimal soLuongTon;

        [XafDisplayName("Số lượng tồn")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "DuocLieu_SoLuongTon_GreaterThanOrEqualZero",
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
            "DuocLieu_TonToiThieu_GreaterThanOrEqualZero",
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

        [XafDisplayName("Giá nhập")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("EditMask", "n0")]
        [RuleValueComparison(
            "DuocLieu_GiaNhapGanNhat_GreaterThanOrEqualZero",
            DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual,
            0,
            CustomMessageTemplate = "Giá nhập không hợp lệ."
        )]
        public decimal GiaNhapGanNhat
        {
            get => giaNhapGanNhat;
            set => SetPropertyValue(nameof(GiaNhapGanNhat), ref giaNhapGanNhat, value);
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

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            SuDung = true;
            SoLuongTon = 0;
            TonToiThieu = 0;
            GiaNhapGanNhat = 0;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(TenDuocLieu)
                ? base.ToString()
                : TenDuocLieu;
        }
    }
}