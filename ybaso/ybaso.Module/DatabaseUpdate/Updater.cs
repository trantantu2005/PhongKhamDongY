using System;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.DatabaseUpdate
{
    /// <summary>
    /// Cập nhật cơ sở dữ liệu, tạo tài khoản mặc định,
    /// phân quyền và nhập dữ liệu ban đầu.
    /// </summary>
    public class Updater : ModuleUpdater
    {
        public Updater(
            IObjectSpace objectSpace,
            Version currentDBVersion
        )
            : base(objectSpace, currentDBVersion)
        {
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            TaoTaiKhoanVaPhanQuyenMacDinh();

            // Import dữ liệu Đông y từ Excel.
            string excelPath = Path.Combine(
                AppContext.BaseDirectory,
                "DatabaseUpdate",
                "Data",
                "Mohinh-Phaptri.xlsx"
            );

            if (File.Exists(excelPath))
            {
                DongYExcelImporter importer =
                    new DongYExcelImporter(ObjectSpace);

                importer.Import(excelPath);
            }

            // Tạo dữ liệu mẫu cho kho dược.
            KhoDuocTestSeeder.Seed(ObjectSpace);
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }

        #region Tạo tài khoản và vai trò

        private void TaoTaiKhoanVaPhanQuyenMacDinh()
        {
            PermissionPolicyRole adminRole =
                TaoHoacLayRole("Admin");

            PermissionPolicyRole bacSiRole =
                TaoHoacLayRole("BacSi");

            PermissionPolicyRole nhanVienRole =
                TaoHoacLayRole("NhanVien");

            PermissionPolicyRole benhNhanRole =
                TaoHoacLayRole("BenhNhan");

            /*
             * Chỉ vai trò Admin mới được quyền quản trị.
             * Ba vai trò còn lại bắt buộc không phải Admin.
             */
            adminRole.IsAdministrative = true;

            bacSiRole.IsAdministrative = false;
            nhanVienRole.IsAdministrative = false;
            benhNhanRole.IsAdministrative = false;

            /*
             * Nếu không có quyền rõ ràng thì mặc định từ chối.
             */
            bacSiRole.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            nhanVienRole.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            benhNhanRole.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            CapNhatQuyenBacSi(bacSiRole);
            CapNhatQuyenNhanVien(nhanVienRole);
            CapNhatQuyenBenhNhan(benhNhanRole);

            ApplicationUser admin =
                TaoHoacLayUser("admin", "admin");

            GanRoleChoUser(
                admin,
                adminRole
            );

            ApplicationUser bacSi =
                TaoHoacLayUser("bacsi", "123");

            GanRoleChoUser(
                bacSi,
                bacSiRole
            );

            ApplicationUser nhanVien =
                TaoHoacLayUser("nhanvien", "123");

            GanRoleChoUser(
                nhanVien,
                nhanVienRole
            );

            ObjectSpace.CommitChanges();
        }

        private PermissionPolicyRole TaoHoacLayRole(
            string roleName
        )
        {
            PermissionPolicyRole role =
                ObjectSpace.FirstOrDefault<PermissionPolicyRole>(
                    item => item.Name == roleName
                );

            if (role != null)
            {
                return role;
            }

            role =
                ObjectSpace.CreateObject<PermissionPolicyRole>();

            role.Name = roleName;

            return role;
        }

        private ApplicationUser TaoHoacLayUser(
            string userName,
            string password
        )
        {
            ApplicationUser user =
                ObjectSpace.FirstOrDefault<ApplicationUser>(
                    item => item.UserName == userName
                );

            if (user != null)
            {
                return user;
            }

            user =
                ObjectSpace.CreateObject<ApplicationUser>();

            user.UserName = userName;
            user.SetPassword(password);

            /*
             * Phải lưu tài khoản trước để Oid được tạo,
             * sau đó mới tạo thông tin đăng nhập.
             */
            ObjectSpace.CommitChanges();

            ((ISecurityUserWithLoginInfo)user)
                .CreateUserLoginInfo(
                    SecurityDefaults.PasswordAuthentication,
                    ObjectSpace.GetKeyValueAsString(user)
                );

            return user;
        }

        private void GanRoleChoUser(
            ApplicationUser user,
            PermissionPolicyRole role
        )
        {
            if (user == null || role == null)
            {
                return;
            }

            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
            }
        }

        #endregion

        #region Quyền Bác sĩ

        private void CapNhatQuyenBacSi(
            PermissionPolicyRole role
        )
        {
            if (role == null || role.IsAdministrative)
            {
                return;
            }

            role.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            ChoPhepDoc<Benhnhan>(role);
            ChoPhepDoc<Nhanvien>(role);

            ChoPhepCRUD<Cakham>(role);
            ChoPhepCRUD<CakhamSoDoChiTiet>(role);
            ChoPhepCRUD<CakhamChanDoanTuDong>(role);
            ChoPhepCRUD<CakhamNguTaTuDong>(role);
            ChoPhepCRUD<CakhamBenhChungTuDong>(role);
            ChoPhepCRUD<CakhamPhapTriTuDong>(role);

            ChoPhepCRUD<DonThuoc>(role);
            ChoPhepCRUD<ChiTietDonThuoc>(role);

            ChoPhepDoc<DanhMucThuoc>(role);
            ChoPhepDoc<ThanhPhanThuoc>(role);
            ChoPhepDoc<DuocLieu>(role);
            ChoPhepDoc<ListDvt>(role);

            ChoPhepDoc<DanhMucHuyet>(role);
            ChoPhepDoc<Benhdanh>(role);
            ChoPhepDoc<Dieukien>(role);
            ChoPhepDoc<PhacDoDieuTri>(role);
        }

        #endregion

        #region Quyền Nhân viên

        private void CapNhatQuyenNhanVien(
    PermissionPolicyRole role
)
        {
            if (role == null || role.IsAdministrative)
            {
                return;
            }

            role.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            DatQuyenDocTheoTaiKhoan<ApplicationUser>(
                role,
                "[Oid] = CurrentUserId()"
            );

            ChoPhepCRUD<Benhnhan>(role);
            ChoPhepDoc<Nhanvien>(role);

            ChoPhepCRUD<Cakham>(role);

            ChoPhepDoc<DonThuoc>(role);
            ChoPhepDoc<ChiTietDonThuoc>(role);

            ChoPhepCRUD<DanhMucThuoc>(role);
            ChoPhepCRUD<ThanhPhanThuoc>(role);
            ChoPhepCRUD<DuocLieu>(role);

            ChoPhepDoc<ListDvt>(role);

            ChoPhepCRUD<PhieuNhapKho>(role);
            ChoPhepCRUD<ChiTietPhieuNhap>(role);

            ChoPhepCRUD<PhieuXuatKho>(role);
            ChoPhepCRUD<ChiTietPhieuXuat>(role);

            ChoPhepCRUD<HoaDonBenhNhan>(role);
            ChoPhepCRUD<ChiTietHoaDonBenhNhan>(role);
        }

        #endregion

        #region Quyền Bệnh nhân

        private void CapNhatQuyenBenhNhan(
            PermissionPolicyRole role
        )
        {
            if (role == null || role.IsAdministrative)
            {
                return;
            }

            /*
             * Mặc định từ chối mọi dữ liệu chưa được cấp quyền.
             */
            role.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            /*
             * Bệnh nhân chỉ được đọc chính tài khoản
             * đang đăng nhập.
             */
            DatQuyenDocTheoTaiKhoan<ApplicationUser>(
                role,
                "[Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc hồ sơ bệnh nhân được liên kết
             * với tài khoản hiện tại.
             */
            DatQuyenDocTheoTaiKhoan<Benhnhan>(
                role,
                "[TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );
            /*
             * Cho phép bệnh nhân cập nhật đúng hồ sơ
             * được liên kết với tài khoản hiện tại.
             */
            role.AddObjectPermission<Benhnhan>(
                SecurityOperations.Write,   
                "[TaiKhoanDangNhap.Oid] = CurrentUserId()",
                SecurityPermissionState.Allow
            );

            /*
             * Các trường bệnh nhân được phép tự cập nhật.
             */
            role.AddMemberPermission<Benhnhan>(
                SecurityOperations.Write,
                "Dienthoai;Email;Diachi",
                "[TaiKhoanDangNhap.Oid] = CurrentUserId()",
                SecurityPermissionState.Allow
            );

            /*
             * Các trường quan trọng chỉ phòng khám được cập nhật.
             */
            role.AddMemberPermission<Benhnhan>(
                SecurityOperations.Write,
                "Ho;Ten;Ngaysinh;Gioitinh;" +
                "SoBHYT;SoCMND;Ngaycap;Noicap;" +
                "DpTinh;DpHuyen;DpXa;Tiensu;" +
                "TaiKhoanDangNhap;TuKhoaTimKiem;CaKhams",
                "[TaiKhoanDangNhap.Oid] = CurrentUserId()",
                SecurityPermissionState.Deny
            );

            /*
             * Chỉ đọc ca khám thuộc chính bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<Cakham>(
                role,
                "[BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chi tiết số đo thuộc ca khám của bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<CakhamSoDoChiTiet>(
                role,
                "[Cakham.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Kết quả chẩn đoán tự động thuộc ca khám.
             */
            DatQuyenDocTheoTaiKhoan<CakhamChanDoanTuDong>(
                role,
                "[Cakham.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Kết quả ngũ tà thuộc ca khám.
             */
            DatQuyenDocTheoTaiKhoan<CakhamNguTaTuDong>(
                role,
                "[Cakham.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Kết quả bệnh chứng thuộc ca khám.
             */
            DatQuyenDocTheoTaiKhoan<CakhamBenhChungTuDong>(
                role,
                "[Cakham.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Kết quả pháp trị thuộc ca khám.
             */
            DatQuyenDocTheoTaiKhoan<CakhamPhapTriTuDong>(
                role,
                "[Cakham.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc đơn thuốc thuộc chính bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<DonThuoc>(
                role,
                "[BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc dòng thuốc thuộc đơn thuốc
             * của chính bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<ChiTietDonThuoc>(
                role,
                "[DonThuoc.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc phiếu xuất thuốc thuộc chính bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<PhieuXuatKho>(
                role,
                "[BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc chi tiết phiếu xuất của bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<ChiTietPhieuXuat>(
                role,
                "[PhieuXuat.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc hóa đơn thuộc chính bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<HoaDonBenhNhan>(
                role,
                "[BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Chỉ đọc chi tiết hóa đơn của bệnh nhân.
             */
            DatQuyenDocTheoTaiKhoan<ChiTietHoaDonBenhNhan>(
                role,
                "[HoaDon.BenhNhan.TaiKhoanDangNhap.Oid] = CurrentUserId()"
            );

            /*
             * Dữ liệu danh mục dùng để hiển thị thông tin.
             * Đây không phải dữ liệu riêng của bệnh nhân.
             */
            ChoPhepDoc<Nhanvien>(role);
            ChoPhepDoc<DanhMucThuoc>(role);
            ChoPhepDoc<ListDvt>(role);

            ChoPhepDoc<Benhdanh>(role);
            ChoPhepDoc<DanhMucHuyet>(role);
            ChoPhepDoc<Dieukien>(role);
            ChoPhepDoc<PhacDoDieuTri>(role);
        }

        /// <summary>
        /// Từ chối đọc toàn bộ loại dữ liệu,
        /// sau đó chỉ cho đọc những bản ghi thỏa criteria.
        /// </summary>
        private void DatQuyenDocTheoTaiKhoan<T>(
    PermissionPolicyRole role,
    string criteria
)
    where T : class
        {
            if (role == null)
            {
                return;
            }

            /*
             * Xóa quyền cũ để loại bỏ quyền đọc toàn bộ
             * đã được tạo từ những phiên bản trước.
             */
            XoaQuyenTheoLoai<T>(role);

            /*
             * Role đã dùng DenyAllByDefault, do đó chỉ cần
             * cho phép đọc những bản ghi thỏa điều kiện.
             * Các bản ghi còn lại tự động bị từ chối.
             */
            role.AddObjectPermission<T>(
                SecurityOperations.Read,
                criteria,
                SecurityPermissionState.Allow
            );
        }
        private void XoaQuyenTheoLoai<T>(
            PermissionPolicyRole role
        )
        {
            if (role == null)
            {
                return;
            }

            Type targetType = typeof(T);

            var permissions =
                role.TypePermissions
                    .Where(item =>
                        item.TargetType == targetType
                    )
                    .ToList();

            foreach (
                PermissionPolicyTypePermissionObject permission
                in permissions
            )
            {
                /*
                 * Phải bỏ khỏi collection trước để hàm
                 * AddTypePermission không tìm lại quyền cũ.
                 */
                role.TypePermissions.Remove(permission);

                ObjectSpace.Delete(permission);
            }
        }

        #endregion

        #region Hàm hỗ trợ phân quyền

        private void ChoPhepDoc<T>(
            PermissionPolicyRole role
        )
        {
            if (role == null)
            {
                return;
            }

            PermissionPolicyTypePermissionObject permission =
                LayHoacTaoQuyenTheoLoai<T>(role);

            permission.ReadState =
                SecurityPermissionState.Allow;

            permission.NavigateState =
                SecurityPermissionState.Allow;
        }

        private void ChoPhepCRUD<T>(
            PermissionPolicyRole role
        )
        {
            if (role == null)
            {
                return;
            }

            PermissionPolicyTypePermissionObject permission =
                LayHoacTaoQuyenTheoLoai<T>(role);

            permission.ReadState =
                SecurityPermissionState.Allow;

            permission.WriteState =
                SecurityPermissionState.Allow;

            permission.CreateState =
                SecurityPermissionState.Allow;

            permission.DeleteState =
                SecurityPermissionState.Allow;

            permission.NavigateState =
                SecurityPermissionState.Allow;
        }

        private PermissionPolicyTypePermissionObject
            LayHoacTaoQuyenTheoLoai<T>(
                PermissionPolicyRole role
            )
        {
            Type targetType = typeof(T);

            PermissionPolicyTypePermissionObject permission =
                role.TypePermissions
                    .FirstOrDefault(item =>
                        item.TargetType == targetType
                    );

            if (permission != null)
            {
                return permission;
            }

            permission =
                ObjectSpace.CreateObject<
                    PermissionPolicyTypePermissionObject
                >();

            permission.TargetType = targetType;

            role.TypePermissions.Add(permission);

            return permission;
        }

        #endregion
    }
}