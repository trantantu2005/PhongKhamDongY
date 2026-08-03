using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.QuanTri)]
    [ImageName("BO_Employee")]
    [XafDisplayName("Nhân viên")]
    [DefaultProperty(nameof(HoTen))]
    public class Nhanvien : XPObject
    {
        public Nhanvien(Session session) : base(session) { }

        private string hodem;

        [Size(100)]
        [XafDisplayName("Họ đệm")]
        public string Hodem
        {
            get => hodem;
            set => SetPropertyValue(nameof(Hodem), ref hodem, value);
        }

        private string ten;

        [Size(100)]
        [XafDisplayName("Tên")]
        public string Ten
        {
            get => ten;
            set => SetPropertyValue(nameof(Ten), ref ten, value);
        }

        [XafDisplayName("Họ tên")]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [VisibleInDetailView(false)]
        [ModelDefault("AllowEdit", "False")]
        public string HoTen => $"{Hodem} {Ten}".Trim();

        private bool nam;

        [XafDisplayName("Nam")]
        public bool Nam
        {
            get => nam;
            set => SetPropertyValue(nameof(Nam), ref nam, value);
        }

        private DateTime ngaysinh;

        [XafDisplayName("Ngày sinh")]
        public DateTime Ngaysinh
        {
            get => ngaysinh;
            set => SetPropertyValue(nameof(Ngaysinh), ref ngaysinh, value);
        }

        private string diachi;

        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Địa chỉ")]
        public string Diachi
        {
            get => diachi;
            set => SetPropertyValue(nameof(Diachi), ref diachi, value);
        }

        private string dienthoai;

        [Size(50)]
        [XafDisplayName("Điện thoại")]
        public string Dienthoai
        {
            get => dienthoai;
            set => SetPropertyValue(nameof(Dienthoai), ref dienthoai, value);
        }

        private string email;

        [Size(255)]
        [XafDisplayName("Email")]
        public string Email
        {
            get => email;
            set => SetPropertyValue(nameof(Email), ref email, value);
        }

        private string chucVu;

        [Size(100)]
        [XafDisplayName("Chức vụ")]
        public string ChucVu
        {
            get => chucVu;
            set => SetPropertyValue(nameof(ChucVu), ref chucVu, value);
        }

        private string chuyenMon;

        [Size(255)]
        [XafDisplayName("Chuyên môn")]
        public string ChuyenMon
        {
            get => chuyenMon;
            set => SetPropertyValue(nameof(ChuyenMon), ref chuyenMon, value);
        }

        private ApplicationUser taiKhoanDangNhap;

        [XafDisplayName("Tài khoản đăng nhập")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public ApplicationUser TaiKhoanDangNhap
        {
            get => taiKhoanDangNhap;
            set => SetPropertyValue(nameof(TaiKhoanDangNhap), ref taiKhoanDangNhap, value);
        }

        [Association("Bacsi-Cakham")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [XafDisplayName("Ca khám")]
        public XPCollection<Cakham> CaKhams
        {
            get => GetCollection<Cakham>(nameof(CaKhams));
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(HoTen))
                return HoTen;

            return base.ToString();
        }
    }
}