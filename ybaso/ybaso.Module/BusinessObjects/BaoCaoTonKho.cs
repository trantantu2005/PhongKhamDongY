using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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
    [XafDisplayName("Báo cáo tồn kho")]
    public class BaoCaoTonKho : XPObject
    {
        public BaoCaoTonKho(Session session) : base(session) { }

        private string tenBaoCao;

        [Size(255)]
        [XafDisplayName("Tên báo cáo")]
        public string TenBaoCao
        {
            get => tenBaoCao;
            set => SetPropertyValue(nameof(TenBaoCao), ref tenBaoCao, value);
        }

        private string tuKhoa;

        [Size(255)]
        [XafDisplayName("Từ khóa lọc")]
        [ModelDefault("NullText", "Nhập mã thang thuốc hoặc tên thang thuốc...")]
        public string TuKhoa
        {
            get => tuKhoa;
            set => SetPropertyValue(nameof(TuKhoa), ref tuKhoa, value);
        }

        private bool chiHienThuocSapHet;

        [XafDisplayName("Chỉ hiện thang thuốc sắp hết/hết")]
        public bool ChiHienThuocSapHet
        {
            get => chiHienThuocSapHet;
            set => SetPropertyValue(nameof(ChiHienThuocSapHet), ref chiHienThuocSapHet, value);
        }

        private DateTime ngayLap;

        [XafDisplayName("Ngày lập")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime NgayLap
        {
            get => ngayLap;
            set => SetPropertyValue(nameof(NgayLap), ref ngayLap, value);
        }

        private int tongSoThuoc;

        [XafDisplayName("Tổng số thang thuốc")]
        [ModelDefault("AllowEdit", "False")]
        public int TongSoThuoc
        {
            get => tongSoThuoc;
            set => SetPropertyValue(nameof(TongSoThuoc), ref tongSoThuoc, value);
        }

        private int soThuocSapHet;

        [XafDisplayName("Số thang thuốc sắp hết/hết")]
        [ModelDefault("AllowEdit", "False")]
        public int SoThuocSapHet
        {
            get => soThuocSapHet;
            set => SetPropertyValue(nameof(SoThuocSapHet), ref soThuocSapHet, value);
        }

        private decimal tongGiaTriTon;

        [XafDisplayName("Tổng giá trị tồn")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        [ModelDefault("AllowEdit", "False")]
        public decimal TongGiaTriTon
        {
            get => tongGiaTriTon;
            set => SetPropertyValue(nameof(TongGiaTriTon), ref tongGiaTriTon, value);
        }

        private string ghiChu;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Ghi chú")]
        public string GhiChu
        {
            get => ghiChu;
            set => SetPropertyValue(nameof(GhiChu), ref ghiChu, value);
        }

        [Association("BaoCaoTonKho-ChiTiets")]
        [XpoAggregated]
        [XafDisplayName("Chi tiết tồn kho")]
        public XPCollection<ChiTietBaoCaoTonKho> ChiTiets
        {
            get => GetCollection<ChiTietBaoCaoTonKho>(nameof(ChiTiets));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            NgayLap = DateTime.Now;
            TenBaoCao = $"Báo cáo tồn kho - {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        [Action(Caption = "Tạo báo cáo", ImageName = "Action_Refresh", AutoCommit = true)]
        public void TaoBaoCao()
        {
            XoaChiTietCu();

            List<DanhMucThuoc> danhSachThuoc = new XPQuery<DanhMucThuoc>(Session)
                .Where(x => x.SuDung)
                .ToList();

            string key = ChuanHoaTuKhoa(TuKhoa);

            if (!string.IsNullOrWhiteSpace(key))
            {
                danhSachThuoc = danhSachThuoc
                    .Where(x =>
                        ChuanHoaTuKhoa(x.MaThuoc).Contains(key) ||
                        ChuanHoaTuKhoa(x.TenThuoc).Contains(key)
                    )
                    .ToList();
            }

            if (ChiHienThuocSapHet)
            {
                danhSachThuoc = danhSachThuoc
                    .Where(x => x.SoLuongTon <= x.TonToiThieu)
                    .ToList();
            }

            foreach (DanhMucThuoc thuoc in danhSachThuoc.OrderBy(x => x.TenThuoc))
            {
                decimal giaVon = thuoc.GiaTinhTuDuocLieu > 0 ? thuoc.GiaTinhTuDuocLieu : thuoc.GiaBan;
                decimal giaTriTon = thuoc.SoLuongTon * giaVon;

                ChiTiets.Add(new ChiTietBaoCaoTonKho(Session)
                {
                    BaoCao = this,
                    Thuoc = thuoc,
                    MaThuoc = thuoc.MaThuoc,
                    TenThuoc = thuoc.TenThuoc,
                    DonViTinh = thuoc.DonViTinh == null ? "" : thuoc.DonViTinh.ToString(),
                    SoLuongTon = thuoc.SoLuongTon,
                    TonToiThieu = thuoc.TonToiThieu,
                    GiaNhapGanNhat = giaVon,
                    GiaBan = thuoc.GiaBan,
                    GiaTriTon = giaTriTon,
                    TinhTrang = LayTinhTrangTonKho(thuoc)
                });
            }

            TongSoThuoc = danhSachThuoc.Count;
            SoThuocSapHet = danhSachThuoc.Count(x => x.SoLuongTon <= x.TonToiThieu);
            TongGiaTriTon = danhSachThuoc.Sum(x => x.SoLuongTon * (x.GiaTinhTuDuocLieu > 0 ? x.GiaTinhTuDuocLieu : x.GiaBan));

            GhiChu = TongSoThuoc == 0
                ? "Không có dữ liệu tồn kho phù hợp."
                : $"Đã tổng hợp {TongSoThuoc} thang thuốc.";
        }

        private string LayTinhTrangTonKho(DanhMucThuoc thuoc)
        {
            if (thuoc.SoLuongTon <= 0)
                return "Hết hàng";

            if (thuoc.SoLuongTon <= thuoc.TonToiThieu)
                return "Sắp hết";

            return "Còn hàng";
        }

        private void XoaChiTietCu()
        {
            foreach (var item in ChiTiets.ToList())
                item.Delete();
        }

        private static string ChuanHoaTuKhoa(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Trim().ToLowerInvariant();
            text = text.Replace("đ", "d");

            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        public override string ToString()
        {
            return TenBaoCao;
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chi tiết báo cáo tồn kho")]
    public class ChiTietBaoCaoTonKho : XPObject
    {
        public ChiTietBaoCaoTonKho(Session session) : base(session) { }

        private BaoCaoTonKho baoCao;

        [Association("BaoCaoTonKho-ChiTiets")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public BaoCaoTonKho BaoCao
        {
            get => baoCao;
            set => SetPropertyValue(nameof(BaoCao), ref baoCao, value);
        }

        private DanhMucThuoc thuoc;

        [XafDisplayName("Thang thuốc")]
        public DanhMucThuoc Thuoc
        {
            get => thuoc;
            set => SetPropertyValue(nameof(Thuoc), ref thuoc, value);
        }

        private string maThuoc;

        [Size(100)]
        [XafDisplayName("Mã thuốc")]
        public string MaThuoc
        {
            get => maThuoc;
            set => SetPropertyValue(nameof(MaThuoc), ref maThuoc, value);
        }

        private string tenThuoc;

        [Size(255)]
        [XafDisplayName("Tên thuốc")]
        public string TenThuoc
        {
            get => tenThuoc;
            set => SetPropertyValue(nameof(TenThuoc), ref tenThuoc, value);
        }

        private string donViTinh;

        [Size(100)]
        [XafDisplayName("Đơn vị tính")]
        public string DonViTinh
        {
            get => donViTinh;
            set => SetPropertyValue(nameof(DonViTinh), ref donViTinh, value);
        }

        private decimal soLuongTon;

        [XafDisplayName("Số lượng tồn")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal SoLuongTon
        {
            get => soLuongTon;
            set => SetPropertyValue(nameof(SoLuongTon), ref soLuongTon, value);
        }

        private decimal tonToiThieu;

        [XafDisplayName("Tồn tối thiểu")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal TonToiThieu
        {
            get => tonToiThieu;
            set => SetPropertyValue(nameof(TonToiThieu), ref tonToiThieu, value);
        }

        private decimal giaNhapGanNhat;

        [XafDisplayName("Giá tính từ dược liệu")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal GiaNhapGanNhat
        {
            get => giaNhapGanNhat;
            set => SetPropertyValue(nameof(GiaNhapGanNhat), ref giaNhapGanNhat, value);
        }

        private decimal giaBan;

        [XafDisplayName("Giá bán")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal GiaBan
        {
            get => giaBan;
            set => SetPropertyValue(nameof(GiaBan), ref giaBan, value);
        }

        private decimal giaTriTon;

        [XafDisplayName("Giá trị tồn")]
        [ModelDefault("DisplayFormat", "{0:n0}")]
        public decimal GiaTriTon
        {
            get => giaTriTon;
            set => SetPropertyValue(nameof(GiaTriTon), ref giaTriTon, value);
        }

        private string tinhTrang;

        [Size(100)]
        [XafDisplayName("Tình trạng")]
        public string TinhTrang
        {
            get => tinhTrang;
            set => SetPropertyValue(nameof(TinhTrang), ref tinhTrang, value);
        }
    }
}