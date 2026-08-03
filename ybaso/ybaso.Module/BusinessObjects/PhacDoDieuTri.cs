using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.PhanTichChanDoan)]
    [XafDisplayName("Phác đồ điều trị")]
    public class PhacDoDieuTri : XPObject
    {
        public PhacDoDieuTri(Session session) : base(session) { }

        // Giữ lại association cũ với Cakham để không lỗi với Cakham.cs hiện tại.
        private Cakham ca;
        [Association("Cakham-PhacDoDieuTri")]
        [XafDisplayName("Ca khám")]
        [VisibleInListView(false)]
        public Cakham Ca
        {
            get => ca;
            set => SetPropertyValue(nameof(Ca), ref ca, value);
        }

        private Benhdanh benh;
        [Association("Benhdanh-PhacDoDieuTri")]
        [XafDisplayName("Bệnh danh")]
        public Benhdanh Benh
        {
            get => benh;
            set => SetPropertyValue(nameof(Benh), ref benh, value);
        }

        private string phapDieuTri;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Pháp điều trị")]
        public string PhapDieuTri
        {
            get => phapDieuTri;
            set => SetPropertyValue(nameof(PhapDieuTri), ref phapDieuTri, value);
        }

        private string phuongDuoc;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Phương dược")]
        public string PhuongDuoc
        {
            get => phuongDuoc;
            set => SetPropertyValue(nameof(PhuongDuoc), ref phuongDuoc, value);
        }

        private string phuongHuyet;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Phương huyệt")]
        public string PhuongHuyet
        {
            get => phuongHuyet;
            set => SetPropertyValue(nameof(PhuongHuyet), ref phuongHuyet, value);
        }

        private string loiDan;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Lời dặn")]
        public string LoiDan
        {
            get => loiDan;
            set => SetPropertyValue(nameof(LoiDan), ref loiDan, value);
        }

        private DateTime ngayLap;
        [XafDisplayName("Ngày lập")]
        public DateTime NgayLap
        {
            get => ngayLap;
            set => SetPropertyValue(nameof(NgayLap), ref ngayLap, value);
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
            NgayLap = DateTime.Now;
            SuDung = true;
        }

        public override string ToString()
        {
            if (Benh != null)
                return Benh.ToString();

            return PhapDieuTri;
        }
    }
}