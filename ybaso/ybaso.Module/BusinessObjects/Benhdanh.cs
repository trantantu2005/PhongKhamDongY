using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.PhanTichChanDoan)]
    [XafDisplayName("Bệnh danh")]
    public class Benhdanh : XPObject
    {
        public Benhdanh(Session session) : base(session) { }

        private int stt;
        [VisibleInListView(true)]
        [XafDisplayName("STT")]
        public int Stt
        {
            get => stt;
            set => SetPropertyValue(nameof(Stt), ref stt, value);
        }

        private string ma;
        [Size(100)]
        [Indexed(Unique = true)]
        [VisibleInListView(true)]
        [XafDisplayName("Mã bệnh")]
        public string Ma
        {
            get => ma;
            set => SetPropertyValue(nameof(Ma), ref ma, value);
        }

        private string tenbenh;
        [Size(255)]
        [RuleRequiredField(DefaultContexts.Save)]
        [VisibleInListView(true)]
        [XafDisplayName("Bệnh danh")]
        public string Tenbenh
        {
            get => tenbenh;
            set => SetPropertyValue(nameof(Tenbenh), ref tenbenh, value);
        }

        private string congThuc;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Công thức gốc Excel")]
        public string CongThuc
        {
            get => congThuc;
            set => SetPropertyValue(nameof(CongThuc), ref congThuc, value);
        }

        private string moHinh;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Mô hình")]
        public string MoHinh
        {
            get => moHinh;
            set => SetPropertyValue(nameof(MoHinh), ref moHinh, value);
        }

        private string lyChung;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Lý chứng")]
        public string LyChung
        {
            get => lyChung;
            set => SetPropertyValue(nameof(LyChung), ref lyChung, value);
        }

        private string bieuChung;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Biểu chứng")]
        public string BieuChung
        {
            get => bieuChung;
            set => SetPropertyValue(nameof(BieuChung), ref bieuChung, value);
        }

        private string trieuchung;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Triệu chứng")]
        public string Trieuchung
        {
            get => trieuchung;
            set => SetPropertyValue(nameof(Trieuchung), ref trieuchung, value);
        }

        private string benhcanh;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Bệnh cảnh YHHĐ")]
        public string Benhcanh
        {
            get => benhcanh;
            set => SetPropertyValue(nameof(Benhcanh), ref benhcanh, value);
        }

        private string phapdieutri;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(true)]
        [XafDisplayName("Pháp điều trị")]
        public string Phapdieutri
        {
            get => phapdieutri;
            set => SetPropertyValue(nameof(Phapdieutri), ref phapdieutri, value);
        }

        private string phuongduoc;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Phương dược")]
        public string Phuongduoc
        {
            get => phuongduoc;
            set => SetPropertyValue(nameof(Phuongduoc), ref phuongduoc, value);
        }

        private string phuonghuyet;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Phương huyệt")]
        public string Phuonghuyet
        {
            get => phuonghuyet;
            set => SetPropertyValue(nameof(Phuonghuyet), ref phuonghuyet, value);
        }

        private bool suDung = true;
        [XafDisplayName("Sử dụng")]
        public bool SuDung
        {
            get => suDung;
            set => SetPropertyValue(nameof(SuDung), ref suDung, value);
        }

        private string ghiChu;
        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        [Association("Benhdanh-Dieukien")]
        [DevExpress.Xpo.Aggregated]
        [XafDisplayName("Điều kiện chẩn đoán")]
        public XPCollection<Dieukien> DieuKiens
        {
            get => GetCollection<Dieukien>(nameof(DieuKiens));
        }

        [Association("Benhdanh-PhacDoDieuTri")]
        [DevExpress.Xpo.Aggregated]
        [XafDisplayName("Phác đồ điều trị")]
        public XPCollection<PhacDoDieuTri> PhacDoDieuTris
        {
            get => GetCollection<PhacDoDieuTri>(nameof(PhacDoDieuTris));
        }

        // Giữ lại association cũ để không lỗi với Chandoan.cs hiện tại.
        [Association("Benhdanh-Chandoan")]
        [XafDisplayName("Các ca chẩn đoán")]
        [VisibleInListView(false)]
        public XPCollection<Chandoan> ChanDoans
        {
            get => GetCollection<Chandoan>(nameof(ChanDoans));
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Ma))
                return $"{Ma} - {Tenbenh}";

            return Tenbenh;
        }
    }
}