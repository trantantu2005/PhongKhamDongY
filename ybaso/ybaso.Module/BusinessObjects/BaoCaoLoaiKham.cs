using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.BaoCao)]
    [XafDisplayName("Báo cáo theo loại khám")]
    public class BaoCaoLoaiKham : XPObject
    {
        public BaoCaoLoaiKham(Session session) : base(session) { }

        private string tenBaoCao;

        [Size(255)]
        [XafDisplayName("Tên báo cáo")]
        public string TenBaoCao
        {
            get => tenBaoCao;
            set => SetPropertyValue(nameof(TenBaoCao), ref tenBaoCao, value);
        }

        private DateTime? tuNgay;

        [XafDisplayName("Từ ngày")]
        public DateTime? TuNgay
        {
            get => tuNgay;
            set => SetPropertyValue(nameof(TuNgay), ref tuNgay, value);
        }

        private DateTime? denNgay;

        [XafDisplayName("Đến ngày")]
        public DateTime? DenNgay
        {
            get => denNgay;
            set => SetPropertyValue(nameof(DenNgay), ref denNgay, value);
        }

        private DateTime ngayLap;

        [XafDisplayName("Ngày lập")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime NgayLap
        {
            get => ngayLap;
            set => SetPropertyValue(nameof(NgayLap), ref ngayLap, value);
        }

        private int tongLuotKham;

        [XafDisplayName("Tổng lượt khám")]
        [ModelDefault("AllowEdit", "False")]
        public int TongLuotKham
        {
            get => tongLuotKham;
            set => SetPropertyValue(nameof(TongLuotKham), ref tongLuotKham, value);
        }

        private int soBenhNhanMoi;

        [XafDisplayName("Bệnh nhân mới")]
        [ModelDefault("AllowEdit", "False")]
        public int SoBenhNhanMoi
        {
            get => soBenhNhanMoi;
            set => SetPropertyValue(nameof(SoBenhNhanMoi), ref soBenhNhanMoi, value);
        }

        private int soBenhNhanCu;

        [XafDisplayName("Bệnh nhân cũ")]
        [ModelDefault("AllowEdit", "False")]
        public int SoBenhNhanCu
        {
            get => soBenhNhanCu;
            set => SetPropertyValue(nameof(SoBenhNhanCu), ref soBenhNhanCu, value);
        }

        private int soTaiKham;

        [XafDisplayName("Tái khám")]
        [ModelDefault("AllowEdit", "False")]
        public int SoTaiKham
        {
            get => soTaiKham;
            set => SetPropertyValue(nameof(SoTaiKham), ref soTaiKham, value);
        }

        private string ghiChu;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        [Association("BaoCaoLoaiKham-ChiTiets")]
        [XpoAggregated]
        [XafDisplayName("Chi tiết báo cáo")]
        public XPCollection<ChiTietBaoCaoLoaiKham> ChiTiets
        {
            get => GetCollection<ChiTietBaoCaoLoaiKham>(nameof(ChiTiets));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            NgayLap = DateTime.Now;
            TenBaoCao = $"Báo cáo theo loại khám - {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        [Action(Caption = "Tạo báo cáo", ImageName = "Action_Refresh", AutoCommit = true)]
        public void TaoBaoCao()
        {
            XoaChiTietCu();

            IQueryable<Cakham> query = new XPQuery<Cakham>(Session);

            if (TuNgay != null)
            {
                DateTime from = TuNgay.Value.Date;
                query = query.Where(x => x.NgayCT >= from);
            }

            if (DenNgay != null)
            {
                DateTime to = DenNgay.Value.Date.AddDays(1);
                query = query.Where(x => x.NgayCT < to);
            }

            var danhSachCa = query.ToList();

            TongLuotKham = danhSachCa.Count;
            SoBenhNhanMoi = danhSachCa.Count(x => x.LoaiLuotKham == LoaiLuotKham.BenhNhanMoi);
            SoBenhNhanCu = danhSachCa.Count(x => x.LoaiLuotKham == LoaiLuotKham.BenhNhanCu);
            SoTaiKham = danhSachCa.Count(x => x.LoaiLuotKham == LoaiLuotKham.TaiKham);

            ThemDong(LoaiLuotKham.BenhNhanMoi, "Bệnh nhân mới", SoBenhNhanMoi);
            ThemDong(LoaiLuotKham.BenhNhanCu, "Bệnh nhân cũ", SoBenhNhanCu);
            ThemDong(LoaiLuotKham.TaiKham, "Tái khám", SoTaiKham);

            GhiChu = TongLuotKham == 0
                ? "Không có dữ liệu khám phù hợp với điều kiện lọc."
                : $"Đã tổng hợp {TongLuotKham} lượt khám.";
        }

        private void ThemDong(LoaiLuotKham loai, string tenLoai, int soLuot)
        {
            decimal tyLe = TongLuotKham == 0
                ? 0
                : Math.Round((decimal)soLuot * 100 / TongLuotKham, 2);

            ChiTiets.Add(new ChiTietBaoCaoLoaiKham(Session)
            {
                BaoCao = this,
                LoaiLuotKham = loai,
                TenLoai = tenLoai,
                SoLuot = soLuot,
                TyLe = tyLe
            });
        }

        private void XoaChiTietCu()
        {
            foreach (var item in ChiTiets.ToList())
                item.Delete();
        }

        public override string ToString()
        {
            return TenBaoCao;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết báo cáo loại khám")]
    public class ChiTietBaoCaoLoaiKham : XPObject
    {
        public ChiTietBaoCaoLoaiKham(Session session) : base(session) { }

        private BaoCaoLoaiKham baoCao;

        [Association("BaoCaoLoaiKham-ChiTiets")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public BaoCaoLoaiKham BaoCao
        {
            get => baoCao;
            set => SetPropertyValue(nameof(BaoCao), ref baoCao, value);
        }

        private LoaiLuotKham loaiLuotKham;

        [XafDisplayName("Loại lượt khám")]
        public LoaiLuotKham LoaiLuotKham
        {
            get => loaiLuotKham;
            set => SetPropertyValue(nameof(LoaiLuotKham), ref loaiLuotKham, value);
        }

        private string tenLoai;

        [Size(255)]
        [XafDisplayName("Tên loại")]
        public string TenLoai
        {
            get => tenLoai;
            set => SetPropertyValue(nameof(TenLoai), ref tenLoai, value);
        }

        private int soLuot;

        [XafDisplayName("Số lượt")]
        public int SoLuot
        {
            get => soLuot;
            set => SetPropertyValue(nameof(SoLuot), ref soLuot, value);
        }

        private decimal tyLe;

        [XafDisplayName("Tỷ lệ (%)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        public decimal TyLe
        {
            get => tyLe;
            set => SetPropertyValue(nameof(TyLe), ref tyLe, value);
        }
    }
}