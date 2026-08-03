using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.TiepNhanKham)]
    [ImageName("BO_Person")]
    [XafDisplayName("Bệnh nhân")]
    [DefaultProperty(nameof(HoTen))]
    public class Benhnhan : XPObject
    {
        public Benhnhan(Session session) : base(session)
        {
        }

        private string ho;

        [Size(100)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Họ")]
        public string Ho
        {
            get => ho;

            set
            {
                if (SetPropertyValue(nameof(Ho), ref ho, value))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        private string ten;

        [Size(100)]
        [RuleRequiredField(
            DefaultContexts.Save,
            CustomMessageTemplate = "Vui lòng nhập tên bệnh nhân."
        )]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Tên")]
        public string Ten
        {
            get => ten;

            set
            {
                if (SetPropertyValue(nameof(Ten), ref ten, value))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Họ tên")]
        [ModelDefault("AllowEdit", "False")]
        public string HoTen => $"{Ho} {Ten}".Trim();

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Thông tin tìm kiếm")]
        [ModelDefault("AllowEdit", "False")]
        public string ThongTinTimKiem
        {
            get
            {
                string hoTen =
                    string.IsNullOrWhiteSpace(HoTen)
                        ? "Chưa có tên"
                        : HoTen;

                string sdt =
                    string.IsNullOrWhiteSpace(Dienthoai)
                        ? "Chưa có SĐT"
                        : Dienthoai;

                string ngaySinh =
                    Ngaysinh == DateTime.MinValue
                        ? "Chưa có ngày sinh"
                        : Ngaysinh.ToString("dd/MM/yyyy");

                return $"{hoTen} - {sdt} - {ngaySinh}";
            }
        }

        private string tuKhoaTimKiem;

        [Size(500)]
        [Indexed]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Từ khóa tìm kiếm")]
        public string TuKhoaTimKiem
        {
            get => tuKhoaTimKiem;

            set => SetPropertyValue(
                nameof(TuKhoaTimKiem),
                ref tuKhoaTimKiem,
                value
            );
        }

        private DateTime ngaysinh;

        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Ngày sinh")]
        public DateTime Ngaysinh
        {
            get => ngaysinh;

            set
            {
                if (SetPropertyValue(
                    nameof(Ngaysinh),
                    ref ngaysinh,
                    value
                ))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        private int gioitinh;

        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Giới tính")]
        public int Gioitinh
        {
            get => gioitinh;

            set => SetPropertyValue(
                nameof(Gioitinh),
                ref gioitinh,
                value
            );
        }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Giới tính")]
        [ModelDefault("AllowEdit", "False")]
        public string GioiTinhText
        {
            get
            {
                if (Gioitinh == 1)
                {
                    return "Nam";
                }

                if (Gioitinh == 2)
                {
                    return "Nữ";
                }

                return string.Empty;
            }
        }

        private string dienthoai;

        [Size(50)]
        [Indexed]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Điện thoại")]
        public string Dienthoai
        {
            get => dienthoai;

            set
            {
                if (SetPropertyValue(
                    nameof(Dienthoai),
                    ref dienthoai,
                    value
                ))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        private string email;

        [Size(255)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Email")]
        public string Email
        {
            get => email;

            set
            {
                if (SetPropertyValue(
                    nameof(Email),
                    ref email,
                    value
                ))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        private string soBHYT;

        [Size(100)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Số BHYT")]
        public string SoBHYT
        {
            get => soBHYT;

            set => SetPropertyValue(
                nameof(SoBHYT),
                ref soBHYT,
                value
            );
        }

        private string soCMND;

        [Size(100)]
        [Indexed]
        [VisibleInListView(false)]
        [VisibleInLookupListView(true)]
        [XafDisplayName("Số CMND/CCCD")]
        public string SoCMND
        {
            get => soCMND;

            set
            {
                if (SetPropertyValue(
                    nameof(SoCMND),
                    ref soCMND,
                    value
                ))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        private DateTime ngaycap;

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Ngày cấp")]
        public DateTime Ngaycap
        {
            get => ngaycap;

            set => SetPropertyValue(
                nameof(Ngaycap),
                ref ngaycap,
                value
            );
        }

        private Diaphuong noicap;

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Nơi cấp")]
        public Diaphuong Noicap
        {
            get => noicap;

            set => SetPropertyValue(
                nameof(Noicap),
                ref noicap,
                value
            );
        }

        private string diachi;

        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Địa chỉ")]
        public string Diachi
        {
            get => diachi;

            set
            {
                if (SetPropertyValue(
                    nameof(Diachi),
                    ref diachi,
                    value
                ))
                {
                    CapNhatTuKhoaTimKiem();
                }
            }
        }

        private Diaphuong dpTinh;

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Tỉnh/Thành")]
        public Diaphuong DpTinh
        {
            get => dpTinh;

            set => SetPropertyValue(
                nameof(DpTinh),
                ref dpTinh,
                value
            );
        }

        private Diaphuong dpHuyen;

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Quận/Huyện")]
        public Diaphuong DpHuyen
        {
            get => dpHuyen;

            set => SetPropertyValue(
                nameof(DpHuyen),
                ref dpHuyen,
                value
            );
        }

        private Diaphuong dpXa;

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Phường/Xã")]
        public Diaphuong DpXa
        {
            get => dpXa;

            set => SetPropertyValue(
                nameof(DpXa),
                ref dpXa,
                value
            );
        }

        private string tiensu;

        [Size(SizeAttribute.Unlimited)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Tiền sử bệnh")]
        public string Tiensu
        {
            get => tiensu;

            set => SetPropertyValue(
                nameof(Tiensu),
                ref tiensu,
                value
            );
        }

        private ApplicationUser taiKhoanDangNhap;

        /*
         * Một tài khoản chỉ được liên kết với một hồ sơ bệnh nhân.
         *
         * SkipNullOrEmptyValues = true:
         * - Cho phép bệnh nhân chưa có tài khoản.
         * - Không cho phép một tài khoản được gán cho hai bệnh nhân.
         */
        [RuleUniqueValue(
            "Benhnhan_TaiKhoanDangNhap_Unique",
            DefaultContexts.Save,
            SkipNullOrEmptyValues = true,
            CustomMessageTemplate =
                "Tài khoản đăng nhập này đã được liên kết với một hồ sơ bệnh nhân khác."
        )]
        [XafDisplayName("Tài khoản đăng nhập")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public ApplicationUser TaiKhoanDangNhap
        {
            get => taiKhoanDangNhap;

            set => SetPropertyValue(
                nameof(TaiKhoanDangNhap),
                ref taiKhoanDangNhap,
                value
            );
        }

        [Association("Benhnhan-Cakham")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Ca khám")]
        public XPCollection<Cakham> CaKhams
        {
            get => GetCollection<Cakham>(
                nameof(CaKhams)
            );
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            CapNhatTuKhoaTimKiem();
        }

        protected override void OnSaving()
        {
            /*
             * Kiểm tra bổ sung để bảo vệ cả trường hợp
             * lưu từ trang Razor tự viết, không chỉ giao diện XAF.
             */
            KiemTraTaiKhoanDangNhapKhongBiTrung();

            CapNhatTuKhoaTimKiem();

            base.OnSaving();
        }

        private void KiemTraTaiKhoanDangNhapKhongBiTrung()
        {
            if (TaiKhoanDangNhap == null)
            {
                return;
            }

            Benhnhan hoSoBiTrung =
                new XPQuery<Benhnhan>(Session)
                    .FirstOrDefault(x =>
                        x.Oid != Oid &&
                        x.TaiKhoanDangNhap != null &&
                        x.TaiKhoanDangNhap ==
                            TaiKhoanDangNhap
                    );

            if (hoSoBiTrung == null)
            {
                return;
            }

            string tenBenhNhan =
                string.IsNullOrWhiteSpace(
                    hoSoBiTrung.HoTen
                )
                    ? $"#{hoSoBiTrung.Oid}"
                    : hoSoBiTrung.HoTen;

            throw new UserFriendlyException(
                "Tài khoản đăng nhập này đã được liên kết với " +
                $"bệnh nhân \"{tenBenhNhan}\". " +
                "Mỗi tài khoản chỉ được liên kết với một hồ sơ bệnh nhân."
            );
        }

        public void CapNhatTuKhoaTimKiem()
        {
            string hoTen =
                $"{Ho} {Ten}".Trim();

            string ngaySinh =
                Ngaysinh == DateTime.MinValue
                    ? string.Empty
                    : Ngaysinh.ToString("dd/MM/yyyy");

            string raw =
                $"{hoTen} " +
                $"{Dienthoai} " +
                $"{SoCMND} " +
                $"{Email} " +
                $"{Diachi} " +
                $"{ngaySinh}";

            TuKhoaTimKiem =
                BoDauTiengViet(raw)
                    .ToLowerInvariant();
        }

        private static string BoDauTiengViet(
            string text
        )
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text =
                text.Trim()
                    .ToLowerInvariant();

            text =
                text.Replace("đ", "d");

            string normalized =
                text.Normalize(
                    NormalizationForm.FormD
                );

            StringBuilder builder =
                new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category =
                    CharUnicodeInfo.GetUnicodeCategory(c);

                if (category !=
                    UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder
                .ToString()
                .Normalize(
                    NormalizationForm.FormC
                );
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(HoTen))
            {
                return HoTen;
            }

            return base.ToString();
        }
    }
}