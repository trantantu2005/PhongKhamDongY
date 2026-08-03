using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    [NavigationItem(false)]
    [XafDisplayName("Phiếu khám xuất file")]
    [DefaultProperty(nameof(TenPhieu))]
    public class PhieuKhamXuatFile : XPObject
    {
        public PhieuKhamXuatFile(Session session) : base(session) { }

        private string tenPhieu;
        [Size(255)]
        [XafDisplayName("Tên phiếu")]
        public string TenPhieu
        {
            get => tenPhieu;
            set => SetPropertyValue(nameof(TenPhieu), ref tenPhieu, value);
        }

        private DateTime ngayXuat;
        [XafDisplayName("Ngày xuất")]
        public DateTime NgayXuat
        {
            get => ngayXuat;
            set => SetPropertyValue(nameof(NgayXuat), ref ngayXuat, value);
        }

        private Cakham caKham;
        [XafDisplayName("Ca khám")]
        public Cakham CaKham
        {
            get => caKham;
            set => SetPropertyValue(nameof(CaKham), ref caKham, value);
        }

        private FileData tepTin;
        [XpoAggregated]
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("File phiếu khám")]
        public FileData TepTin
        {
            get => tepTin;
            set => SetPropertyValue(nameof(TepTin), ref tepTin, value);
        }

        [Association("PhieuKham-SoDoPreviews")]
        [XpoAggregated]
        [DetailViewLayout("02. Xem trước phiếu khám", LayoutGroupType.TabbedGroup, 10)]
        [XafDisplayName("Số đo")]
        public XPCollection<PhieuKhamSoDoPreview> SoDoPreviews
        {
            get => GetCollection<PhieuKhamSoDoPreview>(nameof(SoDoPreviews));
        }

        [Association("PhieuKham-ChanDoanPreviews")]
        [XpoAggregated]
        [DetailViewLayout("02. Xem trước phiếu khám", LayoutGroupType.TabbedGroup, 10)]
        [XafDisplayName("Chẩn đoán")]
        public XPCollection<PhieuKhamChanDoanPreview> ChanDoanPreviews
        {
            get => GetCollection<PhieuKhamChanDoanPreview>(nameof(ChanDoanPreviews));
        }

        [Association("PhieuKham-NguTaPreviews")]
        [XpoAggregated]
        [DetailViewLayout("02. Xem trước phiếu khám", LayoutGroupType.TabbedGroup, 10)]
        [XafDisplayName("Ngũ tà")]
        public XPCollection<PhieuKhamNguTaPreview> NguTaPreviews
        {
            get => GetCollection<PhieuKhamNguTaPreview>(nameof(NguTaPreviews));
        }

        [Association("PhieuKham-BenhChungPreviews")]
        [XpoAggregated]
        [DetailViewLayout("02. Xem trước phiếu khám", LayoutGroupType.TabbedGroup, 10)]
        [XafDisplayName("Bệnh chứng")]
        public XPCollection<PhieuKhamBenhChungPreview> BenhChungPreviews
        {
            get => GetCollection<PhieuKhamBenhChungPreview>(nameof(BenhChungPreviews));
        }

        [Association("PhieuKham-PhapTriPreviews")]
        [XpoAggregated]
        [DetailViewLayout("02. Xem trước phiếu khám", LayoutGroupType.TabbedGroup, 10)]
        [XafDisplayName("Pháp trị")]
        public XPCollection<PhieuKhamPhapTriPreview> PhapTriPreviews
        {
            get => GetCollection<PhieuKhamPhapTriPreview>(nameof(PhapTriPreviews));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NgayXuat = DateTime.Now;
        }

        public override string ToString()
        {
            return TenPhieu;
        }
    }

    [NavigationItem(false)]
    [XafDisplayName("Xem trước số đo")]
    public class PhieuKhamSoDoPreview : XPObject
    {
        public PhieuKhamSoDoPreview(Session session) : base(session) { }

        private PhieuKhamXuatFile phieuKham;
        [Association("PhieuKham-SoDoPreviews")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuKhamXuatFile PhieuKham
        {
            get => phieuKham;
            set => SetPropertyValue(nameof(PhieuKham), ref phieuKham, value);
        }

        private int tt;
        [Index(0)]
        [XafDisplayName("TT")]
        [ModelDefault("Width", "60px")]
        public int TT
        {
            get => tt;
            set => SetPropertyValue(nameof(TT), ref tt, value);
        }

        private string nhomChi;
        [Size(50)]
        [Index(1)]
        [XafDisplayName("Nhóm chi")]
        public string NhomChi
        {
            get => nhomChi;
            set => SetPropertyValue(nameof(NhomChi), ref nhomChi, value);
        }

        private string tinhHuyet;
        [Size(100)]
        [Index(2)]
        [XafDisplayName("Tinh huyệt")]
        public string TinhHuyet
        {
            get => tinhHuyet;
            set => SetPropertyValue(nameof(TinhHuyet), ref tinhHuyet, value);
        }

        private double trai;
        [Index(3)]
        [XafDisplayName("Trái")]
        public double Trai
        {
            get => trai;
            set => SetPropertyValue(nameof(Trai), ref trai, value);
        }

        private string traiDanhGia;
        [Size(10)]
        [Index(4)]
        [XafDisplayName("Trái +/-")]
        public string TraiDanhGia
        {
            get => traiDanhGia;
            set => SetPropertyValue(nameof(TraiDanhGia), ref traiDanhGia, value);
        }

        private double phai;
        [Index(5)]
        [XafDisplayName("Phải")]
        public double Phai
        {
            get => phai;
            set => SetPropertyValue(nameof(Phai), ref phai, value);
        }

        private string phaiDanhGia;
        [Size(10)]
        [Index(6)]
        [XafDisplayName("Phải +/-")]
        public string PhaiDanhGia
        {
            get => phaiDanhGia;
            set => SetPropertyValue(nameof(PhaiDanhGia), ref phaiDanhGia, value);
        }

        private double lechTrungBinh;
        [Index(7)]
        [XafDisplayName("Lệch TB")]
        public double LechTrungBinh
        {
            get => lechTrungBinh;
            set => SetPropertyValue(nameof(LechTrungBinh), ref lechTrungBinh, value);
        }

        private double chenhTraiPhai;
        [Index(8)]
        [XafDisplayName("Chênh trái/phải")]
        public double ChenhTraiPhai
        {
            get => chenhTraiPhai;
            set => SetPropertyValue(nameof(ChenhTraiPhai), ref chenhTraiPhai, value);
        }

        private string nguong;
        [Size(100)]
        [Index(9)]
        [XafDisplayName("Ngưỡng")]
        public string Nguong
        {
            get => nguong;
            set => SetPropertyValue(nameof(Nguong), ref nguong, value);
        }
    }

    [NavigationItem(false)]
    [XafDisplayName("Xem trước chẩn đoán")]
    public class PhieuKhamChanDoanPreview : XPObject
    {
        public PhieuKhamChanDoanPreview(Session session) : base(session) { }

        private PhieuKhamXuatFile phieuKham;
        [Association("PhieuKham-ChanDoanPreviews")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuKhamXuatFile PhieuKham
        {
            get => phieuKham;
            set => SetPropertyValue(nameof(PhieuKham), ref phieuKham, value);
        }

        private string nhom;
        [Size(255)]
        [XafDisplayName("Nhóm")]
        public string Nhom
        {
            get => nhom;
            set => SetPropertyValue(nameof(Nhom), ref nhom, value);
        }

        private string ketQua;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Kết quả")]
        public string KetQua
        {
            get => ketQua;
            set => SetPropertyValue(nameof(KetQua), ref ketQua, value);
        }
    }

    [NavigationItem(false)]
    [XafDisplayName("Xem trước ngũ tà")]
    public class PhieuKhamNguTaPreview : XPObject
    {
        public PhieuKhamNguTaPreview(Session session) : base(session) { }

        private PhieuKhamXuatFile phieuKham;
        [Association("PhieuKham-NguTaPreviews")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuKhamXuatFile PhieuKham
        {
            get => phieuKham;
            set => SetPropertyValue(nameof(PhieuKham), ref phieuKham, value);
        }

        private string nhom;
        [Size(255)]
        [XafDisplayName("Nhóm")]
        public string Nhom
        {
            get => nhom;
            set => SetPropertyValue(nameof(Nhom), ref nhom, value);
        }

        private string ketQua;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Kết quả")]
        public string KetQua
        {
            get => ketQua;
            set => SetPropertyValue(nameof(KetQua), ref ketQua, value);
        }
    }

    [NavigationItem(false)]
    [XafDisplayName("Xem trước bệnh chứng")]
    public class PhieuKhamBenhChungPreview : XPObject
    {
        public PhieuKhamBenhChungPreview(Session session) : base(session) { }

        private PhieuKhamXuatFile phieuKham;
        [Association("PhieuKham-BenhChungPreviews")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuKhamXuatFile PhieuKham
        {
            get => phieuKham;
            set => SetPropertyValue(nameof(PhieuKham), ref phieuKham, value);
        }

        private string tenBenhChung;
        [Size(255)]
        [XafDisplayName("Bệnh chứng")]
        public string TenBenhChung
        {
            get => tenBenhChung;
            set => SetPropertyValue(nameof(TenBenhChung), ref tenBenhChung, value);
        }

        private string trieuChung;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Triệu chứng")]
        public string TrieuChung
        {
            get => trieuChung;
            set => SetPropertyValue(nameof(TrieuChung), ref trieuChung, value);
        }
    }

    [NavigationItem(false)]
    [XafDisplayName("Xem trước pháp trị")]
    public class PhieuKhamPhapTriPreview : XPObject
    {
        public PhieuKhamPhapTriPreview(Session session) : base(session) { }

        private PhieuKhamXuatFile phieuKham;
        [Association("PhieuKham-PhapTriPreviews")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public PhieuKhamXuatFile PhieuKham
        {
            get => phieuKham;
            set => SetPropertyValue(nameof(PhieuKham), ref phieuKham, value);
        }

        private string benhChung;
        [Size(255)]
        [Index(0)]
        [XafDisplayName("Bệnh chứng")]
        public string BenhChung
        {
            get => benhChung;
            set => SetPropertyValue(nameof(BenhChung), ref benhChung, value);
        }

        private string phapDieuTri;
        [Size(SizeAttribute.Unlimited)]
        [Index(1)]
        [XafDisplayName("Pháp điều trị")]
        public string PhapDieuTri
        {
            get => phapDieuTri;
            set => SetPropertyValue(nameof(PhapDieuTri), ref phapDieuTri, value);
        }

        private string phuongDuoc;
        [Size(SizeAttribute.Unlimited)]
        [Index(2)]
        [XafDisplayName("Phương dược")]
        public string PhuongDuoc
        {
            get => phuongDuoc;
            set => SetPropertyValue(nameof(PhuongDuoc), ref phuongDuoc, value);
        }

        private string huyet;
        [Size(SizeAttribute.Unlimited)]
        [Index(3)]
        [XafDisplayName("Huyệt")]
        public string Huyet
        {
            get => huyet;
            set => SetPropertyValue(nameof(Huyet), ref huyet, value);
        }
    }
}