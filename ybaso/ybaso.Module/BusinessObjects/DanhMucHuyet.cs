using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.PhanTichChanDoan)]
    [XafDisplayName("Danh mục huyệt")]
    public class DanhMucHuyet : XPObject
    {
        public DanhMucHuyet(Session session) : base(session) { }

        private int thuTu;
        [XafDisplayName("Thứ tự")]
        public int ThuTu
        {
            get => thuTu;
            set => SetPropertyValue(nameof(ThuTu), ref thuTu, value);
        }

        private string maHuyet;
        [Size(100)]
        [Indexed(Unique = true)]
        [XafDisplayName("Mã huyệt")]
        public string MaHuyet
        {
            get => maHuyet;
            set => SetPropertyValue(nameof(MaHuyet), ref maHuyet, value);
        }

        private string tenHuyet;
        [Size(255)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Tên huyệt")]
        public string TenHuyet
        {
            get => tenHuyet;
            set => SetPropertyValue(nameof(TenHuyet), ref tenHuyet, value);
        }

        private NhomChiDo nhomChi;
        [XafDisplayName("Nhóm chi")]
        public NhomChiDo NhomChi
        {
            get => nhomChi;
            set => SetPropertyValue(nameof(NhomChi), ref nhomChi, value);
        }

        private string nhom;
        [Size(255)]
        [XafDisplayName("Nhóm")]
        public string Nhom
        {
            get => nhom;
            set => SetPropertyValue(nameof(Nhom), ref nhom, value);
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

        [Association("DanhMucHuyet-Dieukien")]
        [XafDisplayName("Điều kiện sử dụng huyệt")]
        public XPCollection<Dieukien> DieuKiens
        {
            get => GetCollection<Dieukien>(nameof(DieuKiens));
        }

        public override string ToString()
        {
            return TenHuyet;
        }
    }
}