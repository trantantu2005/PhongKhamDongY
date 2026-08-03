using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;

namespace ybaso.Module.BusinessObjects
{
    [NavigationItem(false)]
    [XafDisplayName("Số đo kinh lạc")]
    public class Bieudogoc : XPObject
    {
        public Bieudogoc(Session session) : base(session) { }

        private Cakham ca;
        [Association("Cakham-Bieudogoc")]
        [XafDisplayName("Ca khám")]
        public Cakham Ca
        {
            get => ca;
            set => SetPropertyValue(nameof(Ca), ref ca, value);
        }

        private int soTT;
        [XafDisplayName("TT")]
        public int SoTT
        {
            get => soTT;
            set => SetPropertyValue(nameof(SoTT), ref soTT, value);
        }

        private DanhMucHuyet huyet;
        [XafDisplayName("Tình huyệt")]
        public DanhMucHuyet Huyet
        {
            get => huyet;
            set => SetPropertyValue(nameof(Huyet), ref huyet, value);
        }

        private float trai;
        [XafDisplayName("Trái")]
        public float Trai
        {
            get => trai;
            set => SetPropertyValue(nameof(Trai), ref trai, value);
        }

        private float phai;
        [XafDisplayName("Phải")]
        public float Phai
        {
            get => phai;
            set => SetPropertyValue(nameof(Phai), ref phai, value);
        }

        private string tinhTrai;
        [Size(10)]
        [XafDisplayName("Tính trái")]
        public string TinhTrai
        {
            get => tinhTrai;
            set => SetPropertyValue(nameof(TinhTrai), ref tinhTrai, value);
        }

        private string tinhPhai;
        [Size(10)]
        [XafDisplayName("Tính phải")]
        public string TinhPhai
        {
            get => tinhPhai;
            set => SetPropertyValue(nameof(TinhPhai), ref tinhPhai, value);
        }

        private float trungBinh;
        [XafDisplayName("Trung bình")]
        public float TrungBinh
        {
            get => trungBinh;
            set => SetPropertyValue(nameof(TrungBinh), ref trungBinh, value);
        }

        private float lechTrungBinh;
        [XafDisplayName("Lệch TB")]
        public float LechTrungBinh
        {
            get => lechTrungBinh;
            set => SetPropertyValue(nameof(LechTrungBinh), ref lechTrungBinh, value);
        }

        private float chenhTraiPhai;
        [XafDisplayName("Chênh trái/phải")]
        public float ChenhTraiPhai
        {
            get => chenhTraiPhai;
            set => SetPropertyValue(nameof(ChenhTraiPhai), ref chenhTraiPhai, value);
        }
    }
}