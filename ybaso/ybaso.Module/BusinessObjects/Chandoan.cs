using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;

namespace ybaso.Module.BusinessObjects
{
    [NavigationItem(false)]
    [XafDisplayName("Chẩn đoán")]
    [DefaultProperty(nameof(TenBenh))]
    public class Chandoan : XPObject
    {
        public Chandoan(Session session) : base(session) { }

        private Cakham ca;

        [Association("Cakham-Chandoan")]
        [RuleRequiredField]
        [XafDisplayName("Ca khám")]
        public Cakham Ca
        {
            get => ca;
            set => SetPropertyValue(nameof(Ca), ref ca, value);
        }

        private Benhdanh ben;

        [Association("Benhdanh-Chandoan")]
        [RuleRequiredField]
        [XafDisplayName("Bệnh danh")]
        public Benhdanh Ben
        {
            get => ben;
            set => SetPropertyValue(nameof(Ben), ref ben, value);
        }

        private string ghiChu;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        private DateTime ngayChanDoan;

        [XafDisplayName("Ngày chẩn đoán")]
        public DateTime NgayChanDoan
        {
            get => ngayChanDoan;
            set => SetPropertyValue(nameof(NgayChanDoan), ref ngayChanDoan, value);
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(true)]
        [XafDisplayName("Tên bệnh")]
        public string TenBenh
        {
            get => Ben != null ? Ben.Tenbenh : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(true)]
        [XafDisplayName("Mô hình")]
        public string MoHinh
        {
            get => Ben != null ? Ben.MoHinh : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(false)]
        [XafDisplayName("Công thức")]
        public string CongThuc
        {
            get => Ben != null ? Ben.CongThuc : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(false)]
        [XafDisplayName("Triệu chứng")]
        public string TrieuChung
        {
            get => Ben != null ? Ben.Trieuchung : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(false)]
        [XafDisplayName("Bệnh cảnh YHHĐ")]
        public string BenhCanh
        {
            get => Ben != null ? Ben.Benhcanh : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(true)]
        [XafDisplayName("Pháp điều trị")]
        public string PhapDieuTri
        {
            get => Ben != null ? Ben.Phapdieutri : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(false)]
        [XafDisplayName("Phương dược")]
        public string PhuongDuoc
        {
            get => Ben != null ? Ben.Phuongduoc : "";
        }

        [DevExpress.Xpo.NonPersistent]
        [VisibleInListView(false)]
        [XafDisplayName("Phương huyệt")]
        public string PhuongHuyet
        {
            get => Ben != null ? Ben.Phuonghuyet : "";
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NgayChanDoan = DateTime.Now;
        }

        public override string ToString()
        {
            return TenBenh;
        }
    }
}