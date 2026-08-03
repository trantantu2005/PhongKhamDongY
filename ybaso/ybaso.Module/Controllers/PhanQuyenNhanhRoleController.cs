using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    /// <summary>
    /// Form phân quyền đơn giản cho quản trị viên.
    /// Mỗi checkbox tương ứng với một chức năng nghiệp vụ.
    /// XAF vẫn lưu quyền thật ở PermissionPolicyRole/TypePermissions phía sau.
    /// </summary>
    [DomainComponent]
    [XafDisplayName("Phân quyền chức năng")]
    public class PhanQuyenNhanhForm
    {
        [Category("01. Tiếp nhận & Khám")]
        [XafDisplayName("Quản lý bệnh nhân")]
        public bool QuanLyBenhNhan { get; set; }

        [Category("01. Tiếp nhận & Khám")]
        [XafDisplayName("Quản lý ca khám")]
        public bool QuanLyCaKham { get; set; }

        [Category("02. Phân tích & Chẩn đoán")]
        [XafDisplayName("Bệnh danh")]
        public bool BenhDanh { get; set; }

        [Category("02. Phân tích & Chẩn đoán")]
        [XafDisplayName("Điều kiện chẩn đoán")]
        public bool DieuKienChanDoan { get; set; }

        [Category("02. Phân tích & Chẩn đoán")]
        [XafDisplayName("Phác đồ điều trị")]
        public bool PhacDoDieuTri { get; set; }

        [Category("02. Phân tích & Chẩn đoán")]
        [XafDisplayName("Danh mục huyệt")]
        public bool DanhMucHuyet { get; set; }

        [Category("03. Đơn thuốc")]
        [XafDisplayName("Danh mục thuốc")]
        public bool DanhMucThuoc { get; set; }

        [Category("03. Đơn thuốc")]
        [XafDisplayName("Đơn thuốc")]
        public bool DonThuoc { get; set; }

        [Category("04. Kho dược")]
        [XafDisplayName("Dược liệu")]
        public bool DuocLieu { get; set; }

        [Category("04. Kho dược")]
        [XafDisplayName("Nhập kho")]
        public bool NhapKho { get; set; }

        [Category("04. Kho dược")]
        [XafDisplayName("Xuất kho")]
        public bool XuatKho { get; set; }

        [Category("04. Kho dược")]
        [XafDisplayName("Hóa đơn thanh toán")]
        public bool HoaDonThanhToan { get; set; }

        [Category("05. Báo cáo")]
        [XafDisplayName("Báo cáo doanh thu")]
        public bool BaoCaoDoanhThu { get; set; }

        [Category("05. Báo cáo")]
        [XafDisplayName("Báo cáo loại khám")]
        public bool BaoCaoLoaiKham { get; set; }

        [Category("05. Báo cáo")]
        [XafDisplayName("Báo cáo tồn kho")]
        public bool BaoCaoTonKho { get; set; }

        [Category("06. Quản trị")]
        [XafDisplayName("Quản lý nhân viên")]
        public bool QuanLyNhanVien { get; set; }

        [Category("06. Quản trị")]
        [XafDisplayName("Quản lý tài khoản")]
        public bool QuanLyTaiKhoan { get; set; }

        [Category("06. Quản trị")]
        [XafDisplayName("Quản lý Role & phân quyền")]
        public bool QuanLyRole { get; set; }
    }

    /// <summary>
    /// Bổ sung nút "Phân quyền nhanh" ngay trên màn hình chi tiết Role mặc định của XAF.
    /// Người quản trị chỉ cần tick chức năng thay vì thao tác Allow/Deny/Any thủ công.
    /// </summary>
    public class PhanQuyenNhanhRoleController
        : ObjectViewController<DetailView, PermissionPolicyRole>
    {
        private readonly PopupWindowShowAction phanQuyenNhanhAction;

        public PhanQuyenNhanhRoleController()
        {
            phanQuyenNhanhAction = new PopupWindowShowAction(
                this,
                "Role_PhanQuyenNhanh",
                PredefinedCategory.Edit)
            {
                Caption = "Phân quyền nhanh",
                ImageName = "Action_Grant",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            phanQuyenNhanhAction.CustomizePopupWindowParams +=
                PhanQuyenNhanhAction_CustomizePopupWindowParams;

            phanQuyenNhanhAction.Execute +=
                PhanQuyenNhanhAction_Execute;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            CapNhatTrangThaiAction();
            View.CurrentObjectChanged += View_CurrentObjectChanged;
        }

        protected override void OnDeactivated()
        {
            View.CurrentObjectChanged -= View_CurrentObjectChanged;
            base.OnDeactivated();
        }

        private void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            CapNhatTrangThaiAction();
        }

        private void CapNhatTrangThaiAction()
        {
            PermissionPolicyRole role = ViewCurrentObject;

            bool choPhep =
                role != null &&
                !role.IsAdministrative &&
                !LaRoleBenhNhanHeThong(role);

            phanQuyenNhanhAction.Active["RoleSupported"] = choPhep;
        }

        private static bool LaRoleBenhNhanHeThong(PermissionPolicyRole role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
            {
                return false;
            }

            string name = role.Name.Trim();

            return name.Equals("BenhNhan", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("Benhnhan", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("Bệnh nhân", StringComparison.OrdinalIgnoreCase);
        }

        private void PhanQuyenNhanhAction_CustomizePopupWindowParams(
            object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            PermissionPolicyRole role = ViewCurrentObject;

            if (role == null)
            {
                throw new UserFriendlyException("Không tìm thấy Role cần phân quyền.");
            }

            IObjectSpace popupObjectSpace =
                Application.CreateObjectSpace(typeof(PhanQuyenNhanhForm));

            PhanQuyenNhanhForm form =
                popupObjectSpace.CreateObject<PhanQuyenNhanhForm>();

            NapQuyenHienTai(role, form);

            DetailView detailView =
                Application.CreateDetailView(popupObjectSpace, form);

            detailView.ViewEditMode = ViewEditMode.Edit;
            e.View = detailView;
        }

        private void PhanQuyenNhanhAction_Execute(
            object sender,
            PopupWindowShowActionExecuteEventArgs e)
        {
            PermissionPolicyRole role = ViewCurrentObject;

            if (role == null)
            {
                throw new UserFriendlyException("Không tìm thấy Role cần phân quyền.");
            }

            if (role.IsAdministrative)
            {
                throw new UserFriendlyException(
                    "Role quản trị hệ thống luôn có toàn quyền và không cần phân quyền bằng checkbox.");
            }

            if (LaRoleBenhNhanHeThong(role))
            {
                throw new UserFriendlyException(
                    "Role Bệnh nhân đang dùng quyền theo từng bản ghi để bảo vệ dữ liệu cá nhân. " +
                    "Không nên sửa Role này bằng chế độ phân quyền nhanh.");
            }

            PhanQuyenNhanhForm form =
                e.PopupWindowViewCurrentObject as PhanQuyenNhanhForm;

            if (form == null)
            {
                throw new UserFriendlyException("Không đọc được danh sách chức năng đã chọn.");
            }

            bool phaiChuyenSangCheDoTick =
                role.PermissionPolicy != SecurityPermissionPolicy.DenyAllByDefault;

            if (phaiChuyenSangCheDoTick)
            {
                /*
                 * Với giao diện tick, quy tắc rõ ràng nhất là:
                 * - Không tick: không có quyền.
                 * - Có tick: cấp quyền cho chức năng tương ứng.
                 * Vì vậy chuyển role về DenyAllByDefault và dựng lại các quyền được quản lý ở đây.
                 */
                XoaTatCaQuyenChucNangDuocQuanLy(role);
            }

            role.PermissionPolicy =
                SecurityPermissionPolicy.DenyAllByDefault;

            ApDungQuyen(
                role,
                form.QuanLyBenhNhan,
                CoQuyenTruyCap<Benhnhan>(role),
                () => CapQuyenQuanLyBenhNhan(role),
                () => XoaQuyenQuanLyBenhNhan(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.QuanLyCaKham,
                CoQuyenTruyCap<Cakham>(role),
                () => CapQuyenQuanLyCaKham(role),
                () => XoaQuyenQuanLyCaKham(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.BenhDanh,
                CoQuyenTruyCap<Benhdanh>(role),
                () => ChoPhepCRUD<Benhdanh>(role),
                () => XoaQuyenTheoLoai<Benhdanh>(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.DieuKienChanDoan,
                CoQuyenTruyCap<Dieukien>(role),
                () => ChoPhepCRUD<Dieukien>(role),
                () => XoaQuyenTheoLoai<Dieukien>(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.PhacDoDieuTri,
                CoQuyenTruyCap<PhacDoDieuTri>(role),
                () => ChoPhepCRUD<PhacDoDieuTri>(role),
                () => XoaQuyenTheoLoai<PhacDoDieuTri>(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.DanhMucHuyet,
                CoQuyenTruyCap<DanhMucHuyet>(role),
                () => ChoPhepCRUD<DanhMucHuyet>(role),
                () => XoaQuyenTheoLoai<DanhMucHuyet>(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.DanhMucThuoc,
                CoQuyenTruyCap<DanhMucThuoc>(role),
                () => CapQuyenDanhMucThuoc(role),
                () => XoaQuyenDanhMucThuoc(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.DonThuoc,
                CoQuyenTruyCap<DonThuoc>(role),
                () => CapQuyenDonThuoc(role),
                () => XoaQuyenDonThuoc(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.DuocLieu,
                CoQuyenTruyCap<DuocLieu>(role),
                () => CapQuyenDuocLieu(role),
                () => XoaQuyenTheoLoai<DuocLieu>(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.NhapKho,
                CoQuyenTruyCap<PhieuNhapKho>(role),
                () => CapQuyenNhapKho(role),
                () => XoaQuyenNhapKho(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.XuatKho,
                CoQuyenTruyCap<PhieuXuatKho>(role),
                () => CapQuyenXuatKho(role),
                () => XoaQuyenXuatKho(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.HoaDonThanhToan,
                CoQuyenTruyCap<HoaDonBenhNhan>(role),
                () => CapQuyenHoaDon(role),
                () => XoaQuyenHoaDon(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.BaoCaoDoanhThu,
                CoQuyenTruyCap<BaoCaoDoanhThu>(role),
                () => CapQuyenBaoCaoDoanhThu(role),
                () => XoaQuyenBaoCaoDoanhThu(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.BaoCaoLoaiKham,
                CoQuyenTruyCap<BaoCaoLoaiKham>(role),
                () => CapQuyenBaoCaoLoaiKham(role),
                () => XoaQuyenBaoCaoLoaiKham(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.BaoCaoTonKho,
                CoQuyenTruyCap<BaoCaoTonKho>(role),
                () => CapQuyenBaoCaoTonKho(role),
                () => XoaQuyenBaoCaoTonKho(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.QuanLyNhanVien,
                CoQuyenTruyCap<Nhanvien>(role),
                () => ChoPhepCRUD<Nhanvien>(role),
                () => XoaQuyenTheoLoai<Nhanvien>(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.QuanLyTaiKhoan,
                CoQuyenTruyCap<ApplicationUser>(role),
                () => CapQuyenQuanLyTaiKhoan(role),
                () => XoaQuyenQuanLyTaiKhoan(role),
                phaiChuyenSangCheDoTick);

            ApDungQuyen(
                role,
                form.QuanLyRole,
                CoQuyenTruyCap<PermissionPolicyRole>(role),
                () => CapQuyenQuanLyRole(role),
                () => XoaQuyenQuanLyRole(role),
                phaiChuyenSangCheDoTick);

            /*
             * Sau khi bật/tắt các chức năng chính, cấp lại quyền đọc tối thiểu
             * cho những dữ liệu tham chiếu mà các chức năng đang được tick cần dùng.
             * Các quyền này KHÔNG có Navigate nên sẽ không tự làm checkbox của
             * chức năng khác bị hiểu nhầm là đang bật.
             */
            DamBaoQuyenThamChieuChoChucNangDaChon(role, form);

            ObjectSpace.SetModified(role);
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            Application.ShowViewStrategy.ShowMessage(
                $"Đã lưu phân quyền chức năng cho Role '{role.Name}'.",
                InformationType.Success,
                4000,
                InformationPosition.Top);
        }

        private static void ApDungQuyen(
            PermissionPolicyRole role,
            bool duocChon,
            bool dangCoQuyen,
            Action capQuyen,
            Action xoaQuyen,
            bool batBuocDungLai)
        {
            if (role == null)
            {
                return;
            }

            if (batBuocDungLai)
            {
                if (duocChon)
                {
                    capQuyen();
                }

                return;
            }

            if (duocChon && !dangCoQuyen)
            {
                capQuyen();
            }
            else if (!duocChon && dangCoQuyen)
            {
                xoaQuyen();
            }
        }

        private void NapQuyenHienTai(
            PermissionPolicyRole role,
            PhanQuyenNhanhForm form)
        {
            form.QuanLyBenhNhan = CoQuyenTruyCap<Benhnhan>(role);
            form.QuanLyCaKham = CoQuyenTruyCap<Cakham>(role);
            form.BenhDanh = CoQuyenTruyCap<Benhdanh>(role);
            form.DieuKienChanDoan = CoQuyenTruyCap<Dieukien>(role);
            form.PhacDoDieuTri = CoQuyenTruyCap<PhacDoDieuTri>(role);
            form.DanhMucHuyet = CoQuyenTruyCap<DanhMucHuyet>(role);
            form.DanhMucThuoc = CoQuyenTruyCap<DanhMucThuoc>(role);
            form.DonThuoc = CoQuyenTruyCap<DonThuoc>(role);
            form.DuocLieu = CoQuyenTruyCap<DuocLieu>(role);
            form.NhapKho = CoQuyenTruyCap<PhieuNhapKho>(role);
            form.XuatKho = CoQuyenTruyCap<PhieuXuatKho>(role);
            form.HoaDonThanhToan = CoQuyenTruyCap<HoaDonBenhNhan>(role);
            form.BaoCaoDoanhThu = CoQuyenTruyCap<BaoCaoDoanhThu>(role);
            form.BaoCaoLoaiKham = CoQuyenTruyCap<BaoCaoLoaiKham>(role);
            form.BaoCaoTonKho = CoQuyenTruyCap<BaoCaoTonKho>(role);
            form.QuanLyNhanVien = CoQuyenTruyCap<Nhanvien>(role);
            form.QuanLyTaiKhoan = CoQuyenTruyCap<ApplicationUser>(role);
            form.QuanLyRole = CoQuyenTruyCap<PermissionPolicyRole>(role);
        }

        private static bool CoQuyenTruyCap<T>(PermissionPolicyRole role)
            where T : class
        {
            if (role == null)
            {
                return false;
            }

            if (role.IsAdministrative)
            {
                return true;
            }

            PermissionPolicyTypePermissionObject permission =
                role.TypePermissions.FirstOrDefault(
                    item => item.TargetType == typeof(T));

            if (permission != null)
            {
                /*
                 * ReadState đơn thuần có thể chỉ là quyền đọc dữ liệu tham chiếu
                 * để một màn hình khác hoạt động. Không coi quyền tham chiếu đó
                 * là "đã bật chức năng". Một chức năng được xem là bật khi
                 * có Navigate hoặc có quyền thay đổi dữ liệu.
                 */
                if (permission.NavigateState == SecurityPermissionState.Allow ||
                    permission.CreateState == SecurityPermissionState.Allow ||
                    permission.WriteState == SecurityPermissionState.Allow ||
                    permission.DeleteState == SecurityPermissionState.Allow)
                {
                    return true;
                }

                if (permission.NavigateState == SecurityPermissionState.Deny)
                {
                    return false;
                }
            }

            return role.PermissionPolicy == SecurityPermissionPolicy.AllowAllByDefault ||
                   role.PermissionPolicy == SecurityPermissionPolicy.ReadOnlyAllByDefault;
        }

        #region Cấp quyền theo chức năng

        private void CapQuyenQuanLyBenhNhan(PermissionPolicyRole role)
        {
            ChoPhepCRUD<Benhnhan>(role);
            ChoPhepDocThamChieu<Diaphuong>(role);
        }

        private void CapQuyenQuanLyCaKham(PermissionPolicyRole role)
        {
            ChoPhepCRUD<Cakham>(role);
            ChoPhepCRUD<CakhamSoDoChiTiet>(role);
            ChoPhepCRUD<CakhamChanDoanTuDong>(role);
            ChoPhepCRUD<CakhamNguTaTuDong>(role);
            ChoPhepCRUD<CakhamBenhChungTuDong>(role);
            ChoPhepCRUD<CakhamPhapTriTuDong>(role);

            ChoPhepDocThamChieu<Benhnhan>(role);
            ChoPhepDocThamChieu<Nhanvien>(role);
            ChoPhepDocThamChieu<Benhdanh>(role);
            ChoPhepDocThamChieu<Dieukien>(role);
            ChoPhepDocThamChieu<PhacDoDieuTri>(role);
            ChoPhepDocThamChieu<DanhMucHuyet>(role);
        }

        private void CapQuyenDanhMucThuoc(PermissionPolicyRole role)
        {
            ChoPhepCRUD<DanhMucThuoc>(role);
            ChoPhepCRUD<ThanhPhanThuoc>(role);
            ChoPhepDocThamChieu<DuocLieu>(role);
            ChoPhepDocThamChieu<ListDvt>(role);
        }

        private void CapQuyenDonThuoc(PermissionPolicyRole role)
        {
            ChoPhepCRUD<DonThuoc>(role);
            ChoPhepCRUD<ChiTietDonThuoc>(role);
            ChoPhepDocThamChieu<Benhnhan>(role);
            ChoPhepDocThamChieu<Cakham>(role);
            ChoPhepDocThamChieu<DanhMucThuoc>(role);
        }

        private void CapQuyenDuocLieu(PermissionPolicyRole role)
        {
            ChoPhepCRUD<DuocLieu>(role);
            ChoPhepDocThamChieu<ListDvt>(role);
        }

        private void CapQuyenNhapKho(PermissionPolicyRole role)
        {
            ChoPhepCRUD<PhieuNhapKho>(role);
            ChoPhepCRUD<ChiTietPhieuNhap>(role);
            ChoPhepDocThamChieu<DuocLieu>(role);
            ChoPhepDocThamChieu<Nhanvien>(role);
        }

        private void CapQuyenXuatKho(PermissionPolicyRole role)
        {
            ChoPhepCRUD<PhieuXuatKho>(role);
            ChoPhepCRUD<ChiTietPhieuXuat>(role);
            ChoPhepDocThamChieu<DuocLieu>(role);
            ChoPhepDocThamChieu<DonThuoc>(role);
            ChoPhepDocThamChieu<Benhnhan>(role);
            ChoPhepDocThamChieu<Nhanvien>(role);
        }

        private void CapQuyenHoaDon(PermissionPolicyRole role)
        {
            ChoPhepCRUD<HoaDonBenhNhan>(role);
            ChoPhepCRUD<ChiTietHoaDonBenhNhan>(role);
            ChoPhepDocThamChieu<PhieuXuatKho>(role);
            ChoPhepDocThamChieu<Benhnhan>(role);
        }

        private void CapQuyenBaoCaoDoanhThu(PermissionPolicyRole role)
        {
            ChoPhepDoc<BaoCaoDoanhThu>(role);
            ChoPhepDoc<ChiTietBaoCaoHoaDon>(role);
            ChoPhepDoc<ChiTietBaoCaoPhieuNhapKho>(role);
        }

        private void CapQuyenBaoCaoLoaiKham(PermissionPolicyRole role)
        {
            ChoPhepDoc<BaoCaoLoaiKham>(role);
            ChoPhepDoc<ChiTietBaoCaoLoaiKham>(role);
        }

        private void CapQuyenBaoCaoTonKho(PermissionPolicyRole role)
        {
            ChoPhepDoc<BaoCaoTonKho>(role);
            ChoPhepDoc<ChiTietBaoCaoTonKho>(role);
        }

        private void CapQuyenQuanLyTaiKhoan(PermissionPolicyRole role)
        {
            ChoPhepCRUD<ApplicationUser>(role);
            ChoPhepCRUD<ApplicationUserLoginInfo>(role);
        }

        private void CapQuyenQuanLyRole(PermissionPolicyRole role)
        {
            ChoPhepCRUD<PermissionPolicyRole>(role);
            ChoPhepCRUD<PermissionPolicyTypePermissionObject>(role);
            ChoPhepCRUD<PermissionPolicyObjectPermissionsObject>(role);
            ChoPhepCRUD<PermissionPolicyMemberPermissionsObject>(role);
            ChoPhepCRUD<PermissionPolicyNavigationPermissionObject>(role);
            ChoPhepCRUD<PermissionPolicyActionPermissionObject>(role);
        }

        private void DamBaoQuyenThamChieuChoChucNangDaChon(
            PermissionPolicyRole role,
            PhanQuyenNhanhForm form)
        {
            if (form.QuanLyBenhNhan)
            {
                ChoPhepDocThamChieu<Diaphuong>(role);
            }

            if (form.QuanLyCaKham)
            {
                ChoPhepDocThamChieu<Benhnhan>(role);
                ChoPhepDocThamChieu<Nhanvien>(role);
                ChoPhepDocThamChieu<Benhdanh>(role);
                ChoPhepDocThamChieu<Dieukien>(role);
                ChoPhepDocThamChieu<PhacDoDieuTri>(role);
                ChoPhepDocThamChieu<DanhMucHuyet>(role);
            }

            if (form.DanhMucThuoc)
            {
                ChoPhepDocThamChieu<DuocLieu>(role);
                ChoPhepDocThamChieu<ListDvt>(role);
            }

            if (form.DonThuoc)
            {
                ChoPhepDocThamChieu<Benhnhan>(role);
                ChoPhepDocThamChieu<Cakham>(role);
                ChoPhepDocThamChieu<DanhMucThuoc>(role);
            }

            if (form.DuocLieu)
            {
                ChoPhepDocThamChieu<ListDvt>(role);
            }

            if (form.NhapKho)
            {
                ChoPhepDocThamChieu<DuocLieu>(role);
                ChoPhepDocThamChieu<Nhanvien>(role);
            }

            if (form.XuatKho)
            {
                ChoPhepDocThamChieu<DuocLieu>(role);
                ChoPhepDocThamChieu<DonThuoc>(role);
                ChoPhepDocThamChieu<Benhnhan>(role);
                ChoPhepDocThamChieu<Nhanvien>(role);
            }

            if (form.HoaDonThanhToan)
            {
                ChoPhepDocThamChieu<PhieuXuatKho>(role);
                ChoPhepDocThamChieu<Benhnhan>(role);
            }
        }

        #endregion

        #region Xóa quyền theo chức năng

        private void XoaQuyenQuanLyBenhNhan(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<Benhnhan>(role);
        }

        private void XoaQuyenQuanLyCaKham(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<Cakham>(role);
            XoaQuyenTheoLoai<CakhamSoDoChiTiet>(role);
            XoaQuyenTheoLoai<CakhamChanDoanTuDong>(role);
            XoaQuyenTheoLoai<CakhamNguTaTuDong>(role);
            XoaQuyenTheoLoai<CakhamBenhChungTuDong>(role);
            XoaQuyenTheoLoai<CakhamPhapTriTuDong>(role);
        }

        private void XoaQuyenDanhMucThuoc(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<DanhMucThuoc>(role);
            XoaQuyenTheoLoai<ThanhPhanThuoc>(role);
        }

        private void XoaQuyenDonThuoc(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<DonThuoc>(role);
            XoaQuyenTheoLoai<ChiTietDonThuoc>(role);
        }

        private void XoaQuyenNhapKho(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<PhieuNhapKho>(role);
            XoaQuyenTheoLoai<ChiTietPhieuNhap>(role);
        }

        private void XoaQuyenXuatKho(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<PhieuXuatKho>(role);
            XoaQuyenTheoLoai<ChiTietPhieuXuat>(role);
        }

        private void XoaQuyenHoaDon(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<HoaDonBenhNhan>(role);
            XoaQuyenTheoLoai<ChiTietHoaDonBenhNhan>(role);
        }

        private void XoaQuyenBaoCaoDoanhThu(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<BaoCaoDoanhThu>(role);
            XoaQuyenTheoLoai<ChiTietBaoCaoHoaDon>(role);
            XoaQuyenTheoLoai<ChiTietBaoCaoPhieuNhapKho>(role);
        }

        private void XoaQuyenBaoCaoLoaiKham(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<BaoCaoLoaiKham>(role);
            XoaQuyenTheoLoai<ChiTietBaoCaoLoaiKham>(role);
        }

        private void XoaQuyenBaoCaoTonKho(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<BaoCaoTonKho>(role);
            XoaQuyenTheoLoai<ChiTietBaoCaoTonKho>(role);
        }

        private void XoaQuyenQuanLyTaiKhoan(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<ApplicationUser>(role);
            XoaQuyenTheoLoai<ApplicationUserLoginInfo>(role);
        }

        private void XoaQuyenQuanLyRole(PermissionPolicyRole role)
        {
            XoaQuyenTheoLoai<PermissionPolicyRole>(role);
            XoaQuyenTheoLoai<PermissionPolicyTypePermissionObject>(role);
            XoaQuyenTheoLoai<PermissionPolicyObjectPermissionsObject>(role);
            XoaQuyenTheoLoai<PermissionPolicyMemberPermissionsObject>(role);
            XoaQuyenTheoLoai<PermissionPolicyNavigationPermissionObject>(role);
            XoaQuyenTheoLoai<PermissionPolicyActionPermissionObject>(role);
        }

        private void XoaTatCaQuyenChucNangDuocQuanLy(PermissionPolicyRole role)
        {
            XoaQuyenQuanLyBenhNhan(role);
            XoaQuyenQuanLyCaKham(role);

            XoaQuyenTheoLoai<Benhdanh>(role);
            XoaQuyenTheoLoai<Dieukien>(role);
            XoaQuyenTheoLoai<PhacDoDieuTri>(role);
            XoaQuyenTheoLoai<DanhMucHuyet>(role);

            XoaQuyenDanhMucThuoc(role);
            XoaQuyenDonThuoc(role);
            XoaQuyenTheoLoai<DuocLieu>(role);
            XoaQuyenNhapKho(role);
            XoaQuyenXuatKho(role);
            XoaQuyenHoaDon(role);

            XoaQuyenBaoCaoDoanhThu(role);
            XoaQuyenBaoCaoLoaiKham(role);
            XoaQuyenBaoCaoTonKho(role);

            XoaQuyenTheoLoai<Nhanvien>(role);
            XoaQuyenQuanLyTaiKhoan(role);
            XoaQuyenQuanLyRole(role);
        }

        #endregion

        #region Hàm hỗ trợ XAF PermissionPolicy

        private void ChoPhepDoc<T>(PermissionPolicyRole role)
            where T : class
        {
            PermissionPolicyTypePermissionObject permission =
                LayHoacTaoQuyenTheoLoai<T>(role);

            permission.ReadState = SecurityPermissionState.Allow;
            permission.NavigateState = SecurityPermissionState.Allow;
        }

        /// <summary>
        /// Chỉ cho phép đọc dữ liệu tham chiếu, không bật Navigate để tránh
        /// tự động hiện thêm chức năng ngoài checkbox đã chọn.
        /// </summary>
        private void ChoPhepDocThamChieu<T>(PermissionPolicyRole role)
            where T : class
        {
            PermissionPolicyTypePermissionObject permission =
                LayHoacTaoQuyenTheoLoai<T>(role);

            if (permission.ReadState != SecurityPermissionState.Allow)
            {
                permission.ReadState = SecurityPermissionState.Allow;
            }
        }

        private void ChoPhepCRUD<T>(PermissionPolicyRole role)
            where T : class
        {
            PermissionPolicyTypePermissionObject permission =
                LayHoacTaoQuyenTheoLoai<T>(role);

            permission.ReadState = SecurityPermissionState.Allow;
            permission.WriteState = SecurityPermissionState.Allow;
            permission.CreateState = SecurityPermissionState.Allow;
            permission.DeleteState = SecurityPermissionState.Allow;
            permission.NavigateState = SecurityPermissionState.Allow;
        }

        private PermissionPolicyTypePermissionObject
            LayHoacTaoQuyenTheoLoai<T>(PermissionPolicyRole role)
            where T : class
        {
            Type targetType = typeof(T);

            PermissionPolicyTypePermissionObject permission =
                role.TypePermissions.FirstOrDefault(
                    item => item.TargetType == targetType);

            if (permission != null)
            {
                return permission;
            }

            permission =
                ObjectSpace.CreateObject<PermissionPolicyTypePermissionObject>();

            permission.TargetType = targetType;
            role.TypePermissions.Add(permission);

            return permission;
        }

        private void XoaQuyenTheoLoai<T>(PermissionPolicyRole role)
            where T : class
        {
            if (role == null)
            {
                return;
            }

            Type targetType = typeof(T);

            var permissions =
                role.TypePermissions
                    .Where(item => item.TargetType == targetType)
                    .ToList();

            foreach (PermissionPolicyTypePermissionObject permission in permissions)
            {
                role.TypePermissions.Remove(permission);
                ObjectSpace.Delete(permission);
            }
        }

        #endregion
    }
}