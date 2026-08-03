using System;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;
using XpoNonPersistent = DevExpress.Xpo.NonPersistentAttribute;

namespace ybaso.Module.BusinessObjects
{
    public enum NhomChiDo
    {
        ChiTren = 0,
        ChiDuoi = 1
    }

    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.TiepNhanKham)]
    [XafDisplayName("Ca khám")]
    public class Cakham : XPObject
    {
        public Cakham(Session session) : base(session) { }

        private int soTT;
        [XafDisplayName("Số thứ tự")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public int SoTT
        {
            get => soTT;
            set => SetPropertyValue(nameof(SoTT), ref soTT, value);
        }

        private DateTime ngayCT;
        [DetailViewLayout(LayoutColumnPosition.Right, "01. Thông tin cơ bản", 0)]
        [Index(2)]
        [XafDisplayName("Ngày khám")]
        [ImmediatePostData]
        [ModelDefault("Width", "140px")]
        public DateTime NgayCT
        {
            get => ngayCT;
            set
            {
                if (SetPropertyValue(nameof(NgayCT), ref ngayCT, value))
                    CapNhatThongTinBenhNhan();
            }
        }

        private int tuoi;
        [DetailViewLayout(LayoutColumnPosition.Right, "01. Thông tin cơ bản", 0)]
        [Index(3)]
        [XafDisplayName("Tuổi")]
        [ModelDefault("Width", "70px")]
        public int Tuoi
        {
            get => tuoi;
            set => SetPropertyValue(nameof(Tuoi), ref tuoi, value);
        }

        private Nhanvien bacsi;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [Association("Bacsi-Cakham")]
        [XafDisplayName("Bác sĩ")]
        [ModelDefault("Width", "140px")]
        public Nhanvien Bacsi
        {
            get => bacsi;
            set => SetPropertyValue(nameof(Bacsi), ref bacsi, value);
        }

        private Benhnhan benhNhan;

        [Association("Benhnhan-Cakham")]
        [DetailViewLayout(LayoutColumnPosition.Left, "01. Thông tin cơ bản", 0)]
        [Index(1)]
        [XafDisplayName("Họ tên")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        public Benhnhan BenhNhan
        {
            get => benhNhan;
            set
            {
                if (SetPropertyValue(nameof(BenhNhan), ref benhNhan, value))
                    CapNhatThongTinBenhNhan();
            }
        }

        [XafDisplayName("Tên ca khám")]
        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        public string TenCaKham
        {
            get
            {
                string tenBenhNhan = BenhNhan == null
                    ? "Chưa chọn bệnh nhân"
                    : BenhNhan.HoTen;

                if (NgayCT == DateTime.MinValue)
                    return tenBenhNhan;

                return $"{tenBenhNhan} - {NgayCT:dd/MM/yyyy HH:mm}";
            }
        }
        [XafDisplayName("Bệnh nhân")]
        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        public string TenBenhNhanGon
        {
            get
            {
                if (BenhNhan == null)
                    return "";

                return BenhNhan.HoTen;
            }
        }

        private DateTime? ngaySinhBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Ngày sinh")]
        public DateTime? NgaySinhBenhNhan
        {
            get
            {
                if (BenhNhan == null || BenhNhan.Ngaysinh == DateTime.MinValue)
                    return ngaySinhBenhNhan;
                return BenhNhan.Ngaysinh;
            }
            set => ngaySinhBenhNhan = value;
        }

        private string dienThoaiBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [XafDisplayName("Điện thoại")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string DienThoaiBenhNhan
        {
            get => BenhNhan == null ? dienThoaiBenhNhan : BenhNhan.Dienthoai;
            set => dienThoaiBenhNhan = value;
        }

        private string gioiTinhBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [XafDisplayName("Giới tính")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string GioiTinhBenhNhan
        {
            get
            {
                if (BenhNhan == null) return gioiTinhBenhNhan;
                return BenhNhan.Gioitinh == 1 ? "Nam" : BenhNhan.Gioitinh == 2 ? "Nữ" : "";
            }
            set => gioiTinhBenhNhan = value;
        }

        private string diaChiBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [XafDisplayName("Địa chỉ")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string DiaChiBenhNhan
        {
            get => BenhNhan == null ? diaChiBenhNhan : BenhNhan.Diachi;
            set => diaChiBenhNhan = value;
        }

        private string soBHYTBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [XafDisplayName("Số BHYT")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string SoBHYTBenhNhan
        {
            get => BenhNhan == null ? soBHYTBenhNhan : BenhNhan.SoBHYT;
            set => soBHYTBenhNhan = value;
        }

        private string soCMNDBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [XafDisplayName("Số CMND/CCCD")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string SoCMNDBenhNhan
        {
            get => BenhNhan == null ? soCMNDBenhNhan : BenhNhan.SoCMND;
            set => soCMNDBenhNhan = value;
        }
        private LoaiLuotKham loaiLuotKham;

        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Loại lượt khám")]
        [ModelDefault("Width", "140px")]
        public LoaiLuotKham LoaiLuotKham
        {
            get => loaiLuotKham;
            set => SetPropertyValue(nameof(LoaiLuotKham), ref loaiLuotKham, value);
        }

        private string tienSuBenhNhan;
        [XpoNonPersistent]
        [DetailViewLayout("02. Thông tin chi tiết bệnh nhân", 1)]
        [XafDisplayName("Tiền sử bệnh")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string TienSuBenhNhan
        {
            get => BenhNhan == null ? tienSuBenhNhan : BenhNhan.Tiensu;
            set => tienSuBenhNhan = value;
        }

        private double chieucao;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Chiều cao")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double Chieucao
        {
            get => chieucao;
            set
            {
                if (SetPropertyValue(nameof(Chieucao), ref chieucao, value))
                    TinhBMI();
            }
        }

        private double cannang;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Cân nặng")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double Cannang
        {
            get => cannang;
            set
            {
                if (SetPropertyValue(nameof(Cannang), ref cannang, value))
                    TinhBMI();
            }
        }

        private double bmi;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("BMI")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double BMI
        {
            get => bmi;
            set => SetPropertyValue(nameof(BMI), ref bmi, value);
        }

        private double nhietdo;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Nhiệt độ")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double Nhietdo
        {
            get => nhietdo;
            set => SetPropertyValue(nameof(Nhietdo), ref nhietdo, value);
        }

        private double nhietDoMoiTruong;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Nhiệt độ môi trường")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double NhietDoMoiTruong
        {
            get => nhietDoMoiTruong;
            set
            {
                if (SetPropertyValue(nameof(NhietDoMoiTruong), ref nhietDoMoiTruong, value))
                    DanhDauCanPhanTich();
            }
        }

        private string huyetAp;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [Size(255)]
        [XafDisplayName("Huyết áp")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string HuyetAp
        {
            get => huyetAp;
            set => SetPropertyValue(nameof(HuyetAp), ref huyetAp, value);
        }

        private string mach;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [Size(255)]
        [XafDisplayName("Mạch")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Mach
        {
            get => mach;
            set => SetPropertyValue(nameof(Mach), ref mach, value);
        }

        private string lydoKham;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Lý do khám")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string LydoKham
        {
            get => lydoKham;
            set => SetPropertyValue(nameof(LydoKham), ref lydoKham, value);
        }

        private string amduong;
        [VisibleInDetailView(false)]
        [Size(255)]
        [XafDisplayName("Âm dương")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Amduong
        {
            get => amduong;
            set => SetPropertyValue(nameof(Amduong), ref amduong, value);
        }

        private string huthuc;
        [VisibleInDetailView(false)]
        [Size(255)]
        [XafDisplayName("Hư thực")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Huthuc
        {
            get => huthuc;
            set => SetPropertyValue(nameof(Huthuc), ref huthuc, value);
        }

        private string khihuyet;
        [VisibleInDetailView(false)]
        [Size(255)]
        [XafDisplayName("Khí huyết")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Khihuyet
        {
            get => khihuyet;
            set => SetPropertyValue(nameof(Khihuyet), ref khihuyet, value);
        }

        private string benhchung;
        [VisibleInDetailView(false)]
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Bệnh chứng")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Benhchung
        {
            get => benhchung;
            set => SetPropertyValue(nameof(Benhchung), ref benhchung, value);
        }

        private string dieutri;
        [VisibleInDetailView(false)]
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Điều trị")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Dieutri
        {
            get => dieutri;
            set => SetPropertyValue(nameof(Dieutri), ref dieutri, value);
        }

        private LoaiCaKham loaiCakham;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Loại ca khám")]
        [ModelDefault("Width", "120px")]
        public LoaiCaKham LoaiCakham
        {
            get => loaiCakham;
            set => SetPropertyValue(nameof(LoaiCakham), ref loaiCakham, value);
        }

        private TrangThaiCaKham trangThai;
        [DetailViewLayout("03. Thông tin ca khám", 2)]
        [XafDisplayName("Trạng thái")]
        [ModelDefault("Width", "120px")]
        public TrangThaiCaKham TrangThai
        {
            get => trangThai;
            set => SetPropertyValue(nameof(TrangThai), ref trangThai, value);
        }

        private bool daPhanTich;
        [XafDisplayName("Đã phân tích")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public bool DaPhanTich
        {
            get => daPhanTich;
            set => SetPropertyValue(nameof(DaPhanTich), ref daPhanTich, value);
        }

        [Association("Cakham-SoDoChiTiets")]
        [XpoAggregated]
        [DetailViewLayout("04. Kết quả khám", LayoutGroupType.TabbedGroup, 3)]
        [XafDisplayName("Số đo")]
        public XPCollection<CakhamSoDoChiTiet> SoDoChiTiets
        {
            get => GetCollection<CakhamSoDoChiTiet>(nameof(SoDoChiTiets));
        }

        [Association("Cakham-DonThuocs")]
        [XpoAggregated]
        [DetailViewLayout("04. Kết quả khám", LayoutGroupType.TabbedGroup, 3)]
        [XafDisplayName("Đơn thuốc")]
        public XPCollection<DonThuoc> DonThuocs
        {
            get => GetCollection<DonThuoc>(nameof(DonThuocs));
        }

        [Association("Cakham-ChanDoanTuDongs")]
        [XpoAggregated]
        [DetailViewLayout("04. Kết quả khám", LayoutGroupType.TabbedGroup, 3)]
        [XafDisplayName("Chẩn đoán")]
        public XPCollection<CakhamChanDoanTuDong> ChanDoanTuDongs
        {
            get => GetCollection<CakhamChanDoanTuDong>(nameof(ChanDoanTuDongs));
        }

        [Association("Cakham-NguTaTuDongs")]
        [XpoAggregated]
        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public XPCollection<CakhamNguTaTuDong> NguTaTuDongs
        {
            get => GetCollection<CakhamNguTaTuDong>(nameof(NguTaTuDongs));
        }
        private string nguTaSoDo;

        [XpoNonPersistent]
        [DetailViewLayout("04. Kết quả khám", LayoutGroupType.TabbedGroup, 3)]
        [XafDisplayName("Ngũ tà")]
        [EditorAlias("NguTaDiagramPropertyEditor")]
        [ModelDefault("AllowEdit", "False")]
        public string NguTaSoDo
        {
            get
            {
                var data = LayDuLieuNguTaSoDo();
                return $"{data.Tren}|{data.Trai}|{data.Giua}|{data.Phai}|{data.Duoi}";
            }
            set => nguTaSoDo = value;
        }
        [Association("Cakham-BenhChungTuDongs")]
        [XpoAggregated]
        [DetailViewLayout("04. Kết quả khám", LayoutGroupType.TabbedGroup, 3)]
        [XafDisplayName("Bệnh chứng")]
        public XPCollection<CakhamBenhChungTuDong> BenhChungTuDongs
        {
            get => GetCollection<CakhamBenhChungTuDong>(nameof(BenhChungTuDongs));
        }


        [Association("Cakham-PhapTriTuDongs")]
        [XpoAggregated]
        [DetailViewLayout("04. Kết quả khám", LayoutGroupType.TabbedGroup, 3)]
        [XafDisplayName("Pháp trị")]
        public XPCollection<CakhamPhapTriTuDong> PhapTriTuDongs
        {
            get => GetCollection<CakhamPhapTriTuDong>(nameof(PhapTriTuDongs));
        }

        // Giữ lại các association cũ để khớp với các class Bieudogoc, Chandoan, Nguta, PhacDoDieuTri hiện có trong project.
        // Các collection này đang ẩn khỏi giao diện vì giao diện mới dùng các tab tự động phía trên.
        [Association("Cakham-Bieudogoc")]
        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public XPCollection<Bieudogoc> BieuDoGocs
        {
            get => GetCollection<Bieudogoc>(nameof(BieuDoGocs));
        }

        [Association("Cakham-Chandoan")]
        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public XPCollection<Chandoan> ChanDoans
        {
            get => GetCollection<Chandoan>(nameof(ChanDoans));
        }

        [Association("Cakham-Nguta")]
        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public XPCollection<Nguta> NguTas
        {
            get => GetCollection<Nguta>(nameof(NguTas));
        }

        [Association("Cakham-PhacDoDieuTri")]
        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public XPCollection<PhacDoDieuTri> PhacDoDieuTris
        {
            get => GetCollection<PhacDoDieuTri>(nameof(PhacDoDieuTris));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NgayCT = DateTime.Now;
            TrangThai = TrangThaiCaKham.ChoKham;
            LoaiCakham = LoaiCaKham.TrucTiep;
            LoaiLuotKham = LoaiLuotKham.BenhNhanMoi;
            NhietDoMoiTruong = 26.3;
            TaoDongSoDoMacDinh();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            // Chỉ nạp dữ liệu hiển thị, không sửa object,
            // không tạo dòng số đo để tránh XAF báo Unsaved changes.
            NapThongTinBenhNhanKhongDanhDauSua();
        }

        protected override void OnSaving()
        {
            if (!IsDeleted)
            {
                CapNhatThongTinBenhNhan();
                TaoDongSoDoMacDinh();

                if (!DaPhanTich)
                    CapNhatPhanTich();
            }

            base.OnSaving();
        }

        [Action(Caption = "Phân tích số đo", ImageName = "Action_Refresh", AutoCommit = true)]
        public void CapNhatPhanTich()
        {
            TaoDongSoDoMacDinh();

            var tren = TinhNhomChi(NhomChiDo.ChiTren, new[] { "Tiểu", "Tâm", "Tam", "Bào", "Đại", "Phế" });
            var duoi = TinhNhomChi(NhomChiDo.ChiDuoi, new[] { "Bàng", "Thận", "Đảm", "Vị", "Can", "Tỳ" });

            if (tren == null || duoi == null)
                return;

            var e = SoDoChiTiets.ToDictionary(x => x.TinhHuyet, x => x.LechTrungBinh);
            double E(string ten) => e.ContainsKey(ten) ? e[ten] : 0;

            Amduong = E("Đảm") < 0 ? "Dương hư" : E("Đảm") > 0 ? "Âm hư" : "";

            string khi = TinhKhiHoacHuyet(new[] { "Tiểu", "Tâm", "Tam", "Bào", "Đại", "Phế" }, "Khí");
            string huyet = TinhKhiHoacHuyet(new[] { "Bàng", "Thận", "Đảm", "Vị", "Can", "Tỳ" }, "Huyết");
            Khihuyet = NoiChuoi(khi, huyet);
            Huthuc = Khihuyet;

            XoaKetQuaCu();

            // 1. Chẩn đoán tổng quát vẫn tính trực tiếp từ số đo.
            ThemChanDoan("Âm/Dương", Amduong);
            ThemChanDoan("Hư/Thực", Huthuc);
            ThemChanDoan("Khí/Huyết", Khihuyet);

            // 2. Ngũ tà vẫn tính trực tiếp từ dấu + / - / 0 của từng huyệt.
            ThemNguTaTuSoDo();

            // 3. Bệnh chứng và pháp trị KHÔNG hard-code nữa.
            //    Hệ thống đọc từ bảng Benhdanh + Dieukien + PhacDoDieuTri.
            var benhDanhPhuHop = TimBenhDanhPhuHopTuDatabase();

            foreach (Benhdanh benh in benhDanhPhuHop)
            {
                ThemBenhChungVaPhapTriTuBenhDanh(benh);
            }

            Benhchung = string.Join(", ", benhDanhPhuHop.Select(x => x.Tenbenh).Distinct());
            Dieutri = string.Join(
                Environment.NewLine,
                PhapTriTuDongs.Select(x => $"- {x.BenhChung}: {x.PhapDieuTri}")
            );

            DaPhanTich = true;
        }

        private List<Benhdanh> TimBenhDanhPhuHopTuDatabase()
        {
            var ketQua = new List<Benhdanh>();

            var danhSachBenh = new XPQuery<Benhdanh>(Session)
                .Where(x => x.SuDung)
                .ToList();

            foreach (Benhdanh benh in danhSachBenh)
            {
                if (benh.DieuKiens == null || benh.DieuKiens.Count == 0)
                    continue;

                if (KiemTraBenhDanh(benh))
                    ketQua.Add(benh);
            }

            return ketQua
                .GroupBy(x => x.Oid)
                .Select(x => x.First())
                .ToList();
        }

        private bool KiemTraBenhDanh(Benhdanh benh)
        {
            var dieuKiens = benh.DieuKiens
                .Where(x => x.Huyet != null)
                .OrderBy(x => x.NhomDieuKien)
                .ThenBy(x => x.ThuTu)
                .ToList();

            if (dieuKiens.Count == 0)
                return false;

            // Một bệnh có thể có nhiều nhóm điều kiện.
            // Trong cùng một nhóm: các điều kiện bắt buộc được hiểu là AND.
            // Giữa các nhóm: chỉ cần một nhóm đúng là bệnh danh được chọn, tức OR.
            var nhomDieuKiens = dieuKiens.GroupBy(x => x.NhomDieuKien <= 0 ? 1 : x.NhomDieuKien);

            foreach (var nhom in nhomDieuKiens)
            {
                bool nhomDung = true;

                foreach (Dieukien dk in nhom)
                {
                    if (!dk.BatBuoc)
                        continue;

                    if (!KiemTraMotDieuKien(dk))
                    {
                        nhomDung = false;
                        break;
                    }
                }

                if (nhomDung)
                    return true;
            }

            return false;
        }

        private bool KiemTraMotDieuKien(Dieukien dieuKien)
        {
            if (dieuKien == null || dieuKien.Huyet == null)
                return false;

            CakhamSoDoChiTiet soDo = TimDongSoDoTheoHuyet(dieuKien.Huyet);

            if (soDo == null)
                return false;

            if (dieuKien.Kieugiatri == KieuGiaTriDieuKien.DanhGiaTrai)
                return SoSanhChuoi(soDo.TraiDanhGia, dieuKien.Sosanh, dieuKien.Giatri);

            if (dieuKien.Kieugiatri == KieuGiaTriDieuKien.DanhGiaPhai)
                return SoSanhChuoi(soDo.PhaiDanhGia, dieuKien.Sosanh, dieuKien.Giatri);

            double giaTriThucTe = LayGiaTriSoDoTheoKieu(soDo, dieuKien.Kieugiatri);
            double giaTriSoSanh = LayGiaTriSoSanh(dieuKien, soDo);

            return SoSanhSo(giaTriThucTe, dieuKien.Sosanh, giaTriSoSanh);
        }

        private CakhamSoDoChiTiet TimDongSoDoTheoHuyet(DanhMucHuyet huyet)
        {
            if (huyet == null)
                return null;

            string tenHuyet = ChuanHoaChuoi(huyet.TenHuyet);
            string maHuyet = ChuanHoaChuoi(huyet.MaHuyet);

            return SoDoChiTiets.FirstOrDefault(x =>
                ChuanHoaChuoi(x.TinhHuyet) == tenHuyet ||
                ChuanHoaChuoi(x.TinhHuyet) == maHuyet
            );
        }

        private double LayGiaTriSoDoTheoKieu(CakhamSoDoChiTiet soDo, KieuGiaTriDieuKien kieu)
        {
            switch (kieu)
            {
                case KieuGiaTriDieuKien.GiaTriTrai:
                    return soDo.TraiDoC;

                case KieuGiaTriDieuKien.GiaTriPhai:
                    return soDo.PhaiDoC;

                case KieuGiaTriDieuKien.ChenhTraiPhai:
                    return soDo.ChenhTraiPhai;

                case KieuGiaTriDieuKien.LechTrungBinh:
                default:
                    return soDo.LechTrungBinh;
            }
        }

        private double LayGiaTriSoSanh(Dieukien dieuKien, CakhamSoDoChiTiet soDo)
        {
            if (dieuKien == null)
                return 0;

            string raw = dieuKien.Giatri ?? string.Empty;
            string text = raw.Trim();

            // Trong Excel, (5) hoặc (5') thường được dùng như ký hiệu ngưỡng lệch.
            // Với mỗi dòng số đo, ngưỡng lệch = (Ngưỡng trên - Ngưỡng dưới) / 2.
            if (text.Contains("(5"))
            {
                if (soDo != null)
                    return Math.Abs((soDo.NguongTren - soDo.NguongDuoi) / 2.0);

                return 0;
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                string clean = text
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("'", "")
                    .Trim();

                if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double valueInvariant))
                    return valueInvariant;

                string textInvariant = clean.Replace(",", ".");
                if (double.TryParse(textInvariant, NumberStyles.Any, CultureInfo.InvariantCulture, out double valueFromComma))
                    return valueFromComma;

                if (double.TryParse(clean, NumberStyles.Any, CultureInfo.CurrentCulture, out double valueCurrent))
                    return valueCurrent;
            }

            return dieuKien.GiaTriSo;
        }

        private bool SoSanhSo(double giaTriThucTe, string phepSoSanh, double giaTriSoSanh)
        {
            string op = string.IsNullOrWhiteSpace(phepSoSanh)
                ? "="
                : phepSoSanh.Trim().ToLower();

            switch (op)
            {
                case ">":
                    return giaTriThucTe > giaTriSoSanh;

                case ">=":
                    return giaTriThucTe >= giaTriSoSanh;

                case "<":
                    return giaTriThucTe < giaTriSoSanh;

                case "<=":
                    return giaTriThucTe <= giaTriSoSanh;

                case "!=":
                case "<>":
                    return Math.Abs(giaTriThucTe - giaTriSoSanh) > 0.0001;

                case "abs>":
                    return Math.Abs(giaTriThucTe) > giaTriSoSanh;

                case "abs>=":
                    return Math.Abs(giaTriThucTe) >= giaTriSoSanh;

                case "abs<":
                    return Math.Abs(giaTriThucTe) < giaTriSoSanh;

                case "abs<=":
                    return Math.Abs(giaTriThucTe) <= giaTriSoSanh;

                case "=":
                case "==":
                default:
                    return Math.Abs(giaTriThucTe - giaTriSoSanh) <= 0.0001;
            }
        }

        private bool SoSanhChuoi(string giaTriThucTe, string phepSoSanh, string giaTriSoSanh)
        {
            string actual = (giaTriThucTe ?? "").Trim();
            string expected = (giaTriSoSanh ?? "").Trim();

            string op = string.IsNullOrWhiteSpace(phepSoSanh)
                ? "="
                : phepSoSanh.Trim();

            if (op == "!=" || op == "<>")
                return !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);

            return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
        }

        private string ChuanHoaChuoi(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string text = input.Trim().ToLowerInvariant();
            text = text.Replace("đ", "d");
            string normalized = text.Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void ThemBenhChungVaPhapTriTuBenhDanh(Benhdanh benh)
        {
            if (benh == null)
                return;

            BenhChungTuDongs.Add(new CakhamBenhChungTuDong(Session)
            {
                Cakham = this,
                TenBenhChung = benh.Tenbenh,
                TrieuChung = NoiChuoiKhacRong(benh.Trieuchung, benh.Benhcanh)
            });

            PhacDoDieuTri phacDo = benh.PhacDoDieuTris
                .Where(x => x.SuDung)
                .FirstOrDefault();

            string phapDieuTri = phacDo != null && !string.IsNullOrWhiteSpace(phacDo.PhapDieuTri)
                ? phacDo.PhapDieuTri
                : benh.Phapdieutri;

            string phuongDuoc = phacDo != null && !string.IsNullOrWhiteSpace(phacDo.PhuongDuoc)
                ? phacDo.PhuongDuoc
                : benh.Phuongduoc;

            string phuongHuyet = phacDo != null && !string.IsNullOrWhiteSpace(phacDo.PhuongHuyet)
                ? phacDo.PhuongHuyet
                : benh.Phuonghuyet;

            PhapTriTuDongs.Add(new CakhamPhapTriTuDong(Session)
            {
                Cakham = this,
                BenhChung = benh.Tenbenh,
                PhapDieuTri = phapDieuTri,
                PhuongDuoc = phuongDuoc,
                Huyet = phuongHuyet
            });
        }

        private static string NoiChuoiKhacRong(params string[] parts)
        {
            return string.Join(
                Environment.NewLine,
                parts.Where(x => !string.IsNullOrWhiteSpace(x))
            );
        }

        public void DanhDauCanPhanTich()
        {
            if (!IsLoading && !IsSaving)
                DaPhanTich = false;
        }

        private void TinhBMI()
        {
            if (Chieucao > 0 && Cannang > 0)
            {
                double caoMet = Chieucao / 100.0;
                BMI = Math.Round(Cannang / (caoMet * caoMet), 2);
            }
        }

        private void TaoDongSoDoMacDinh()
        {
            var macDinh = new[]
            {
                new { TT = 1, Ten = "Tiểu", Nhom = NhomChiDo.ChiTren },
                new { TT = 2, Ten = "Tâm", Nhom = NhomChiDo.ChiTren },
                new { TT = 3, Ten = "Tam", Nhom = NhomChiDo.ChiTren },
                new { TT = 4, Ten = "Bào", Nhom = NhomChiDo.ChiTren },
                new { TT = 5, Ten = "Đại", Nhom = NhomChiDo.ChiTren },
                new { TT = 6, Ten = "Phế", Nhom = NhomChiDo.ChiTren },
                new { TT = 7, Ten = "Bàng", Nhom = NhomChiDo.ChiDuoi },
                new { TT = 8, Ten = "Thận", Nhom = NhomChiDo.ChiDuoi },
                new { TT = 9, Ten = "Đảm", Nhom = NhomChiDo.ChiDuoi },
                new { TT = 10, Ten = "Vị", Nhom = NhomChiDo.ChiDuoi },
                new { TT = 11, Ten = "Can", Nhom = NhomChiDo.ChiDuoi },
                new { TT = 12, Ten = "Tỳ", Nhom = NhomChiDo.ChiDuoi }
            };

            foreach (var item in macDinh)
            {
                var dong = SoDoChiTiets.FirstOrDefault(x => x.TinhHuyet == item.Ten);
                if (dong == null)
                {
                    dong = new CakhamSoDoChiTiet(Session)
                    {
                        Cakham = this,
                        TinhHuyet = item.Ten
                    };
                    SoDoChiTiets.Add(dong);
                }

                dong.TT = item.TT;
                dong.NhomChi = item.Nhom;
            }
        }

        private void CapNhatThongTinBenhNhan()
        {
            if (BenhNhan == null)
            {
                Tuoi = 0;
                ngaySinhBenhNhan = null;
                return;
            }

            ngaySinhBenhNhan = BenhNhan.Ngaysinh == DateTime.MinValue ? null : BenhNhan.Ngaysinh;
            Tuoi = TinhTuoi(ngaySinhBenhNhan);
            dienThoaiBenhNhan = BenhNhan.Dienthoai;
            gioiTinhBenhNhan = BenhNhan.Gioitinh == 1 ? "Nam" : BenhNhan.Gioitinh == 2 ? "Nữ" : "";
            diaChiBenhNhan = BenhNhan.Diachi;
            soBHYTBenhNhan = BenhNhan.SoBHYT;
            soCMNDBenhNhan = BenhNhan.SoCMND;
            tienSuBenhNhan = BenhNhan.Tiensu;
        }
        private void NapThongTinBenhNhanKhongDanhDauSua()
        {
            if (BenhNhan == null)
            {
                ngaySinhBenhNhan = null;
                dienThoaiBenhNhan = string.Empty;
                gioiTinhBenhNhan = string.Empty;
                diaChiBenhNhan = string.Empty;
                soBHYTBenhNhan = string.Empty;
                soCMNDBenhNhan = string.Empty;
                tienSuBenhNhan = string.Empty;
                return;
            }

            ngaySinhBenhNhan = BenhNhan.Ngaysinh == DateTime.MinValue ? null : BenhNhan.Ngaysinh;
            dienThoaiBenhNhan = BenhNhan.Dienthoai;
            gioiTinhBenhNhan = BenhNhan.Gioitinh == 1 ? "Nam" : BenhNhan.Gioitinh == 2 ? "Nữ" : "";
            diaChiBenhNhan = BenhNhan.Diachi;
            soBHYTBenhNhan = BenhNhan.SoBHYT;
            soCMNDBenhNhan = BenhNhan.SoCMND;
            tienSuBenhNhan = BenhNhan.Tiensu;
        }


        private int TinhTuoi(DateTime? ngaySinh)
        {
            if (ngaySinh == null || ngaySinh.Value == DateTime.MinValue)
                return 0;

            DateTime ngayTinh = NgayCT == DateTime.MinValue ? DateTime.Today : NgayCT.Date;
            int tuoiTinh = ngayTinh.Year - ngaySinh.Value.Year;
            if (ngaySinh.Value.Date > ngayTinh.AddYears(-tuoiTinh))
                tuoiTinh--;

            return Math.Max(tuoiTinh, 0);
        }

        private NhomTinhResult TinhNhomChi(NhomChiDo nhom, string[] tenHuyet)
        {
            var dongs = SoDoChiTiets
                .Where(x => tenHuyet.Contains(x.TinhHuyet))
                .OrderBy(x => x.TT)
                .ToList();

            var giaTri = dongs
                .SelectMany(x => new[] { x.TraiDoC, x.PhaiDoC })
                .Where(x => x > 0)
                .ToList();

            if (giaTri.Count == 0)
                return null;

            double max = giaTri.Max();
            double min = giaTri.Min();
            double trungBinh = (max + min) / 2.0;
            double nguongLech = (max - min) / 6.0;
            double nguongTren = trungBinh + nguongLech;
            double nguongDuoi = trungBinh - nguongLech;

            foreach (var dong in dongs)
            {
                dong.NguongTren = Math.Round(nguongTren, 2);
                dong.NguongDuoi = Math.Round(nguongDuoi, 2);
                dong.TraiDanhGia = DanhGia(dong.TraiDoC, nguongTren, nguongDuoi);
                dong.PhaiDanhGia = DanhGia(dong.PhaiDoC, nguongTren, nguongDuoi);
                dong.LechTrungBinh = Math.Round(((dong.TraiDoC + dong.PhaiDoC) / 2.0) - trungBinh, 2);
                dong.ChenhTraiPhai = Math.Round(Math.Abs(dong.TraiDoC - dong.PhaiDoC), 2);
            }

            return new NhomTinhResult
            {
                Max = max,
                Min = min,
                TrungBinh = trungBinh,
                NguongLech = nguongLech,
                NguongTren = nguongTren,
                NguongDuoi = nguongDuoi
            };
        }

        private static string DanhGia(double value, double nguongTren, double nguongDuoi)
        {
            if (value <= 0) return "";
            if (value > nguongTren) return "+";
            if (value < nguongDuoi) return "-";
            return "0";
        }

        private string TinhKhiHoacHuyet(string[] tenHuyet, string tenNhom)
        {
            var values = SoDoChiTiets.Where(x => tenHuyet.Contains(x.TinhHuyet)).Select(x => x.LechTrungBinh).ToList();
            if (values.All(x => x == 0)) return "";

            int soAm = values.Count(x => x < 0);
            double tong = values.Sum();

            if (soAm > 3) return tenNhom + " hư";
            if (soAm == 3)
            {
                if (tong < 0) return tenNhom + " hư";
                if (tong == 0) return "";
                return tenNhom + " thịnh";
            }
            return tenNhom + " thịnh";
        }
        private (string Tren, string Trai, string Giua, string Phai, string Duoi) LayDuLieuNguTaSoDo()
        {
            /*
             * Mô hình mẫu khách hàng đưa:
             *
             *          Đảm
             *
             *   Bàng   Tiểu   Vị
             *
             *          Đại
             *
             * Sau này khi có đủ bảng quy luật chắp kinh của 12 huyệt,
             * chỉ cần bổ sung trong LayBoChapKinh().
             */

            string trungTam = ChonHuyetTrungTamNguTa();

            return LayBoChapKinh(trungTam);
        }

        private string ChonHuyetTrungTamNguTa()
        {
            /*
             * Tạm thời ưu tiên Tiểu để khớp mẫu khách hàng đang đưa.
             * Nếu muốn tự động chọn huyệt lệch mạnh nhất thì mở đoạn phía dưới.
             */
            return "Tiểu";

            /*
            var dongManhNhat = SoDoChiTiets
                .Where(x => x.TraiDoC > 0 || x.PhaiDoC > 0)
                .OrderByDescending(x => Math.Abs(x.LechTrungBinh))
                .ThenBy(x => x.TT)
                .FirstOrDefault();

            return dongManhNhat?.TinhHuyet ?? "Tiểu";
            */
        }

        private (string Tren, string Trai, string Giua, string Phai, string Duoi) LayBoChapKinh(string trungTam)
        {
            var map = new Dictionary<string, (string Tren, string Trai, string Phai, string Duoi)>(StringComparer.OrdinalIgnoreCase)
            {
                // Dòng này khớp đúng hình khách gửi.
                ["Tiểu"] = ("Đảm", "Bàng", "Vị", "Đại")

                /*
                 * Khi khách hàng cung cấp đầy đủ quy luật ngũ tà/chắp kinh,
                 * bổ sung tiếp dạng:
                 *
                 * ["Tâm"] = ("...", "...", "...", "..."),
                 * ["Tam"] = ("...", "...", "...", "..."),
                 * ["Bào"] = ("...", "...", "...", "..."),
                 * ["Đại"] = ("...", "...", "...", "..."),
                 * ["Phế"] = ("...", "...", "...", "..."),
                 * ["Bàng"] = ("...", "...", "...", "..."),
                 * ["Thận"] = ("...", "...", "...", "..."),
                 * ["Đảm"] = ("...", "...", "...", "..."),
                 * ["Vị"] = ("...", "...", "...", "..."),
                 * ["Can"] = ("...", "...", "...", "..."),
                 * ["Tỳ"] = ("...", "...", "...", "..."),
                 */
            };

            if (!map.TryGetValue(trungTam, out var item))
                item = map["Tiểu"];

            return (
                Tren: item.Tren,
                Trai: item.Trai,
                Giua: trungTam,
                Phai: item.Phai,
                Duoi: item.Duoi
            );
        }
        private void ThemNguTaTuSoDo()
        {
            var data = LayDuLieuNguTaSoDo();

            NguTaTuDongs.Add(new CakhamNguTaTuDong(Session)
            {
                Cakham = this,
                TT = 1,
                CotTrai = data.Trai,
                CotGiua = data.Giua,
                CotPhai = data.Phai,
                Nhom = "Sơ đồ ngũ tà",
                KetQua = $"{data.Tren}|{data.Trai}|{data.Giua}|{data.Phai}|{data.Duoi}"
            });
        }


        private static int ChuyenDanhGia(string value)
        {
            if (value == "+") return 1;
            if (value == "-") return -1;
            return 0;
        }

        private static string NoiChuoi(params string[] parts)
        {
            return string.Join(", ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private void XoaKetQuaCu()
        {
            foreach (var item in ChanDoanTuDongs.ToList()) item.Delete();
            foreach (var item in NguTaTuDongs.ToList()) item.Delete();
            foreach (var item in BenhChungTuDongs.ToList()) item.Delete();
            foreach (var item in PhapTriTuDongs.ToList()) item.Delete();
        }

        private void ThemChanDoan(string nhom, string ketQua)
        {
            if (string.IsNullOrWhiteSpace(ketQua)) return;
            ChanDoanTuDongs.Add(new CakhamChanDoanTuDong(Session)
            {
                Cakham = this,
                Nhom = nhom,
                KetQua = ketQua
            });
        }

        private void ThemNguTa(string nhom, string ketQua)
        {
            if (string.IsNullOrWhiteSpace(ketQua)) return;
            NguTaTuDongs.Add(new CakhamNguTaTuDong(Session)
            {
                Cakham = this,
                Nhom = nhom,
                KetQua = ketQua
            });
        }

        private class NhomTinhResult
        {
            public double Max { get; set; }
            public double Min { get; set; }
            public double TrungBinh { get; set; }
            public double NguongLech { get; set; }
            public double NguongTren { get; set; }
            public double NguongDuoi { get; set; }
        }

    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Số đo")]
    [DefaultProperty(nameof(TinhHuyet))]
    public class CakhamSoDoChiTiet : XPObject
    {
        public CakhamSoDoChiTiet(Session session) : base(session) { }

        private Cakham cakham;
        [Association("Cakham-SoDoChiTiets")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public Cakham Cakham
        {
            get => cakham;
            set => SetPropertyValue(nameof(Cakham), ref cakham, value);
        }

        private int tt;
        [XafDisplayName("TT")]
        [Index(0)]
        [ModelDefault("Width", "60px")]
        public int TT
        {
            get => tt;
            set => SetPropertyValue(nameof(TT), ref tt, value);
        }

        private string tinhHuyet;
        [Size(100)]
        [XafDisplayName("Tinh huyệt")]
        [Index(1)]
        [ModelDefault("Width", "160px")]
        public string TinhHuyet
        {
            get => tinhHuyet;
            set => SetPropertyValue(nameof(TinhHuyet), ref tinhHuyet, value);
        }

        private NhomChiDo nhomChi;
        [XafDisplayName("Nhóm chi")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public NhomChiDo NhomChi
        {
            get => nhomChi;
            set => SetPropertyValue(nameof(NhomChi), ref nhomChi, value);
        }

        private double trai;
        [XafDisplayName("Trái")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double Trai
        {
            get => trai;
            set
            {
                if (SetPropertyValue(nameof(Trai), ref trai, value))
                    Cakham?.DanhDauCanPhanTich();
            }
        }

        private double phai;
        [XafDisplayName("Phải")]
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public double Phai
        {
            get => phai;
            set
            {
                if (SetPropertyValue(nameof(Phai), ref phai, value))
                    Cakham?.DanhDauCanPhanTich();
            }
        }

        private double traiDoC;
        [XpoNonPersistent]
        [XafDisplayName("Trái")]
        [ModelDefault("DisplayFormat", "{0:n1}")]
        [ModelDefault("EditMask", "n1")]
        [Index(2)]
        [ModelDefault("Width", "110px")]
        public double TraiDoC
        {
            get => Trai > 0
                ? Math.Round(
                    Trai > 100
                        ? Trai / 10.0
                        : Trai,
                    2)
                : 0;

            set
            {
                traiDoC = value;

                // Quan trọng:
                // ghi vào thuộc tính Persistent thật
                Trai = value;
            }
        }

        private double phaiDoC;
        [XpoNonPersistent]
        [XafDisplayName("Phải")]
        [ModelDefault("DisplayFormat", "{0:n1}")]
        [ModelDefault("EditMask", "n1")]
        [Index(3)]
        [ModelDefault("Width", "110px")]
        public double PhaiDoC
        {
            get => Phai > 0
                ? Math.Round(
                    Phai > 100
                        ? Phai / 10.0
                        : Phai,
                    2)
                : 0;

            set
            {
                phaiDoC = value;

                // Quan trọng:
                // ghi vào thuộc tính Persistent thật
                Phai = value;
            }
        }

        private double nguongTren;
        [XafDisplayName("Ngưỡng trên")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public double NguongTren
        {
            get => nguongTren;
            set => SetPropertyValue(nameof(NguongTren), ref nguongTren, value);
        }

        private double nguongDuoi;
        [XafDisplayName("Ngưỡng dưới")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public double NguongDuoi
        {
            get => nguongDuoi;
            set => SetPropertyValue(nameof(NguongDuoi), ref nguongDuoi, value);
        }

        private string traiDanhGia;
        [Size(10)]
        [XafDisplayName("Trái +/-")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public string TraiDanhGia
        {
            get => traiDanhGia;
            set => SetPropertyValue(nameof(TraiDanhGia), ref traiDanhGia, value);
        }

        private string phaiDanhGia;
        [Size(10)]
        [XafDisplayName("Phải +/-")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public string PhaiDanhGia
        {
            get => phaiDanhGia;
            set => SetPropertyValue(nameof(PhaiDanhGia), ref phaiDanhGia, value);
        }

        private double lechTrungBinh;
        [XafDisplayName("Lệch TB")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public double LechTrungBinh
        {
            get => lechTrungBinh;
            set => SetPropertyValue(nameof(LechTrungBinh), ref lechTrungBinh, value);
        }

        private double chenhTraiPhai;
        [XafDisplayName("Chênh trái/phải")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public double ChenhTraiPhai
        {
            get => chenhTraiPhai;
            set => SetPropertyValue(nameof(ChenhTraiPhai), ref chenhTraiPhai, value);
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Chẩn đoán")]
    public class CakhamChanDoanTuDong : XPObject
    {
        public CakhamChanDoanTuDong(Session session) : base(session) { }

        private Cakham cakham;
        [Association("Cakham-ChanDoanTuDongs")]
        public Cakham Cakham { get => cakham; set => SetPropertyValue(nameof(Cakham), ref cakham, value); }

        private string nhom;
        [Size(255)]
        [XafDisplayName("Nhóm")]
        public string Nhom { get => nhom; set => SetPropertyValue(nameof(Nhom), ref nhom, value); }

        private string ketQua;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Kết quả")]
        public string KetQua { get => ketQua; set => SetPropertyValue(nameof(KetQua), ref ketQua, value); }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Ngũ tà")]
    public class CakhamNguTaTuDong : XPObject
    {
        public CakhamNguTaTuDong(Session session) : base(session) { }

        private Cakham cakham;
        [Association("Cakham-NguTaTuDongs")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public Cakham Cakham
        {
            get => cakham;
            set => SetPropertyValue(nameof(Cakham), ref cakham, value);
        }

        private int tt;
        [XafDisplayName("TT")]
        [VisibleInListView(false)]
        public int TT
        {
            get => tt;
            set => SetPropertyValue(nameof(TT), ref tt, value);
        }

        private string cotTrai;
        [Size(255)]
        [XafDisplayName("")]
        public string CotTrai
        {
            get => cotTrai;
            set => SetPropertyValue(nameof(CotTrai), ref cotTrai, value);
        }

        private string cotGiua;
        [Size(255)]
        [XafDisplayName("")]
        public string CotGiua
        {
            get => cotGiua;
            set => SetPropertyValue(nameof(CotGiua), ref cotGiua, value);
        }

        private string cotPhai;
        [Size(255)]
        [XafDisplayName("")]
        public string CotPhai
        {
            get => cotPhai;
            set => SetPropertyValue(nameof(CotPhai), ref cotPhai, value);
        }

        // Giữ lại 2 field cũ để không phá code/export cũ.
        private string nhom;
        [Size(255)]
        [XafDisplayName("Nhóm")]
        [VisibleInListView(false)]
        public string Nhom
        {
            get => nhom;
            set => SetPropertyValue(nameof(Nhom), ref nhom, value);
        }

        private string ketQua;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Kết quả")]
        [VisibleInListView(false)]
        public string KetQua
        {
            get => ketQua;
            set => SetPropertyValue(nameof(KetQua), ref ketQua, value);
        }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Bệnh chứng")]
    public class CakhamBenhChungTuDong : XPObject
    {
        public CakhamBenhChungTuDong(Session session) : base(session) { }

        private Cakham cakham;
        [Association("Cakham-BenhChungTuDongs")]
        public Cakham Cakham { get => cakham; set => SetPropertyValue(nameof(Cakham), ref cakham, value); }

        private string tenBenhChung;
        [Size(255)]
        [XafDisplayName("Bệnh chứng")]
        public string TenBenhChung { get => tenBenhChung; set => SetPropertyValue(nameof(TenBenhChung), ref tenBenhChung, value); }

        private string trieuChung;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Triệu chứng")]
        public string TrieuChung { get => trieuChung; set => SetPropertyValue(nameof(TrieuChung), ref trieuChung, value); }
    }

    [DefaultClassOptions]
    [NavigationItem(false)]
    [XafDisplayName("Pháp trị")]
    public class CakhamPhapTriTuDong : XPObject
    {
        public CakhamPhapTriTuDong(Session session) : base(session) { }

        private Cakham cakham;
        [Association("Cakham-PhapTriTuDongs")]
        public Cakham Cakham { get => cakham; set => SetPropertyValue(nameof(Cakham), ref cakham, value); }

        private string benhChung;
        [Size(255)]
        [XafDisplayName("Bệnh chứng")]
        public string BenhChung { get => benhChung; set => SetPropertyValue(nameof(BenhChung), ref benhChung, value); }

        private string phapDieuTri;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Pháp điều trị")]
        public string PhapDieuTri { get => phapDieuTri; set => SetPropertyValue(nameof(PhapDieuTri), ref phapDieuTri, value); }

        private string phuongDuoc;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Phương dược")]
        public string PhuongDuoc { get => phuongDuoc; set => SetPropertyValue(nameof(PhuongDuoc), ref phuongDuoc, value); }

        private string huyet;
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Huyệt")]
        public string Huyet { get => huyet; set => SetPropertyValue(nameof(Huyet), ref huyet, value); }
    }

    [DomainComponent]
    [XafDisplayName("Nhập số đo")]
    public class CakhamNhapSoDoForm
    {
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi trên", 0)]
        [XafDisplayName("Tiểu - Trái")] public double TieuTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi trên", 0)]
        [XafDisplayName("Tiểu - Phải")] public double TieuPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi trên", 0)]
        [XafDisplayName("Tâm - Trái")] public double TamTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi trên", 0)]
        [XafDisplayName("Tâm - Phải")] public double TamPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi trên", 0)]
        [XafDisplayName("Tam - Trái")] public double TamTieuTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi trên", 0)]
        [XafDisplayName("Tam - Phải")] public double TamTieuPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi trên", 0)]
        [XafDisplayName("Bào - Trái")] public double BaoTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi trên", 0)]
        [XafDisplayName("Bào - Phải")] public double BaoPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi trên", 0)]
        [XafDisplayName("Đại - Trái")] public double DaiTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi trên", 0)]
        [XafDisplayName("Đại - Phải")] public double DaiPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi trên", 0)]
        [XafDisplayName("Phế - Trái")] public double PheTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi trên", 0)]
        [XafDisplayName("Phế - Phải")] public double PhePhai { get; set; }

        [DetailViewLayout(LayoutColumnPosition.Left, "Chi dưới", 1)]
        [XafDisplayName("Bàng - Trái")] public double BangTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi dưới", 1)]
        [XafDisplayName("Bàng - Phải")] public double BangPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi dưới", 1)]
        [XafDisplayName("Thận - Trái")] public double ThanTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi dưới", 1)]
        [XafDisplayName("Thận - Phải")] public double ThanPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi dưới", 1)]
        [XafDisplayName("Đảm - Trái")] public double DamTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi dưới", 1)]
        [XafDisplayName("Đảm - Phải")] public double DamPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi dưới", 1)]
        [XafDisplayName("Vị - Trái")] public double ViTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi dưới", 1)]
        [XafDisplayName("Vị - Phải")] public double ViPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi dưới", 1)]
        [XafDisplayName("Can - Trái")] public double CanTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi dưới", 1)]
        [XafDisplayName("Can - Phải")] public double CanPhai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Left, "Chi dưới", 1)]
        [XafDisplayName("Tỳ - Trái")] public double TyTrai { get; set; }
        [DetailViewLayout(LayoutColumnPosition.Right, "Chi dưới", 1)]
        [XafDisplayName("Tỳ - Phải")] public double TyPhai { get; set; }

        public void NapTuCaKham(Cakham ca)
        {
            if (ca == null) return;
            TieuTrai = LayTrai(ca, "Tiểu"); TieuPhai = LayPhai(ca, "Tiểu");
            TamTrai = LayTrai(ca, "Tâm"); TamPhai = LayPhai(ca, "Tâm");
            TamTieuTrai = LayTrai(ca, "Tam"); TamTieuPhai = LayPhai(ca, "Tam");
            BaoTrai = LayTrai(ca, "Bào"); BaoPhai = LayPhai(ca, "Bào");
            DaiTrai = LayTrai(ca, "Đại"); DaiPhai = LayPhai(ca, "Đại");
            PheTrai = LayTrai(ca, "Phế"); PhePhai = LayPhai(ca, "Phế");
            BangTrai = LayTrai(ca, "Bàng"); BangPhai = LayPhai(ca, "Bàng");
            ThanTrai = LayTrai(ca, "Thận"); ThanPhai = LayPhai(ca, "Thận");
            DamTrai = LayTrai(ca, "Đảm"); DamPhai = LayPhai(ca, "Đảm");
            ViTrai = LayTrai(ca, "Vị"); ViPhai = LayPhai(ca, "Vị");
            CanTrai = LayTrai(ca, "Can"); CanPhai = LayPhai(ca, "Can");
            TyTrai = LayTrai(ca, "Tỳ"); TyPhai = LayPhai(ca, "Tỳ");
        }

        public void GhiVaoCaKham(Cakham ca)
        {
            if (ca == null) return;
            Ghi(ca, "Tiểu", TieuTrai, TieuPhai);
            Ghi(ca, "Tâm", TamTrai, TamPhai);
            Ghi(ca, "Tam", TamTieuTrai, TamTieuPhai);
            Ghi(ca, "Bào", BaoTrai, BaoPhai);
            Ghi(ca, "Đại", DaiTrai, DaiPhai);
            Ghi(ca, "Phế", PheTrai, PhePhai);
            Ghi(ca, "Bàng", BangTrai, BangPhai);
            Ghi(ca, "Thận", ThanTrai, ThanPhai);
            Ghi(ca, "Đảm", DamTrai, DamPhai);
            Ghi(ca, "Vị", ViTrai, ViPhai);
            Ghi(ca, "Can", CanTrai, CanPhai);
            Ghi(ca, "Tỳ", TyTrai, TyPhai);
            ca.DanhDauCanPhanTich();
            ca.CapNhatPhanTich();
        }

        private static double LayTrai(Cakham ca, string ten)
        {
            var dong = ca.SoDoChiTiets.FirstOrDefault(x => x.TinhHuyet == ten);
            return dong == null ? 0 : dong.TraiDoC;
        }

        private static double LayPhai(Cakham ca, string ten)
        {
            var dong = ca.SoDoChiTiets.FirstOrDefault(x => x.TinhHuyet == ten);
            return dong == null ? 0 : dong.PhaiDoC;
        }

        private static void Ghi(Cakham ca, string ten, double trai, double phai)
        {
            var dong = ca.SoDoChiTiets.FirstOrDefault(x => x.TinhHuyet == ten);
            if (dong == null) return;
            dong.Trai = trai;
            dong.Phai = phai;
        }
    }

}