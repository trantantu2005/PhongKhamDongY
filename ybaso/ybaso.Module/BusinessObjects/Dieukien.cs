using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace ybaso.Module.BusinessObjects
{
    public enum KieuGiaTriDieuKien
    {
        LechTrungBinh = 0,
        GiaTriTrai = 1,
        GiaTriPhai = 2,
        DanhGiaTrai = 3,
        DanhGiaPhai = 4,
        ChenhTraiPhai = 5
    }

    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.PhanTichChanDoan)]
    [XafDisplayName("Điều kiện chẩn đoán")]
    public class Dieukien : XPObject
    {
        public Dieukien(Session session) : base(session) { }

        private Benhdanh ben;
        [Association("Benhdanh-Dieukien")]
        [XafDisplayName("Bệnh danh")]
        public Benhdanh Ben
        {
            get => ben;
            set => SetPropertyValue(nameof(Ben), ref ben, value);
        }

        private DanhMucHuyet huyet;
        [Association("DanhMucHuyet-Dieukien")]
        [XafDisplayName("Huyệt")]
        public DanhMucHuyet Huyet
        {
            get => huyet;
            set => SetPropertyValue(nameof(Huyet), ref huyet, value);
        }

        private int thuTu;
        [XafDisplayName("Thứ tự")]
        public int ThuTu
        {
            get => thuTu;
            set => SetPropertyValue(nameof(ThuTu), ref thuTu, value);
        }

        private int nhomDieuKien = 1;
        [XafDisplayName("Nhóm điều kiện")]
        public int NhomDieuKien
        {
            get => nhomDieuKien;
            set => SetPropertyValue(nameof(NhomDieuKien), ref nhomDieuKien, value);
        }

        private bool batBuoc = true;
        [XafDisplayName("Bắt buộc")]
        public bool BatBuoc
        {
            get => batBuoc;
            set => SetPropertyValue(nameof(BatBuoc), ref batBuoc, value);
        }

        private KieuGiaTriDieuKien kieugiatri;
        [XafDisplayName("Kiểu giá trị")]
        public KieuGiaTriDieuKien Kieugiatri
        {
            get => kieugiatri;
            set => SetPropertyValue(nameof(Kieugiatri), ref kieugiatri, value);
        }

        private string sosanh;
        [Size(50)]
        [XafDisplayName("So sánh")]
        public string Sosanh
        {
            get => sosanh;
            set => SetPropertyValue(nameof(Sosanh), ref sosanh, value);
        }

        private string giatri;
        [Size(255)]
        [XafDisplayName("Giá trị")]
        public string Giatri
        {
            get => giatri;
            set => SetPropertyValue(nameof(Giatri), ref giatri, value);
        }

        private double giaTriSo;
        [XafDisplayName("Giá trị số")]
        public double GiaTriSo
        {
            get => giaTriSo;
            set => SetPropertyValue(nameof(GiaTriSo), ref giaTriSo, value);
        }

        private string congThucGoc;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Công thức gốc Excel")]
        public string CongThucGoc
        {
            get => congThucGoc;
            set => SetPropertyValue(nameof(CongThucGoc), ref congThucGoc, value);
        }

        private string dienGiai;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Diễn giải")]
        public string DienGiai
        {
            get => dienGiai;
            set => SetPropertyValue(nameof(DienGiai), ref dienGiai, value);
        }

        public override string ToString()
        {
            string tenHuyet = Huyet == null ? "" : Huyet.TenHuyet;
            return $"{tenHuyet} {Sosanh} {Giatri}";
        }
    }
}