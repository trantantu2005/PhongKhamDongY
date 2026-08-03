using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ybaso.Module.BusinessObjects;

namespace ybaso.Blazor.Server.Services
{
    /// <summary>
    /// Xác định hồ sơ bệnh nhân thuộc tài khoản đang đăng nhập
    /// và cung cấp các phương thức lấy dữ liệu có kiểm tra sở hữu.
    /// </summary>
    public sealed class CurrentPatientService
    {
        public const string PatientNotLinkedMessage =
            "Tài khoản của bạn chưa được liên kết với hồ sơ bệnh nhân. " +
            "Vui lòng liên hệ phòng khám để được hỗ trợ.";

        private readonly AuthenticationStateProvider
            authenticationStateProvider;

        public CurrentPatientService(
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider =
                authenticationStateProvider;
        }

        /// <summary>
        /// Lấy tên tài khoản đang đăng nhập.
        /// Trả về chuỗi rỗng nếu chưa đăng nhập.
        /// </summary>
        public async Task<string> GetCurrentUserNameAsync()
        {
            var authenticationState =
                await authenticationStateProvider
                    .GetAuthenticationStateAsync();

            if (authenticationState.User?.Identity == null)
                return string.Empty;

            if (!authenticationState.User.Identity.IsAuthenticated)
                return string.Empty;

            return authenticationState.User.Identity.Name
                   ?? string.Empty;
        }

        /// <summary>
        /// Lấy đúng hồ sơ bệnh nhân được liên kết với tài khoản hiện tại.
        /// Không bao giờ tự động lấy bệnh nhân đầu tiên trong hệ thống.
        /// </summary>
        public async Task<Benhnhan> GetCurrentPatientAsync(
            IObjectSpace objectSpace)
        {
            ArgumentNullException.ThrowIfNull(objectSpace);

            string userName =
                await GetCurrentUserNameAsync();

            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return objectSpace.FirstOrDefault<Benhnhan>(x =>
                x.TaiKhoanDangNhap != null &&
                x.TaiKhoanDangNhap.UserName == userName
            );
        }

        /// <summary>
        /// Lấy lịch sử khám của chính bệnh nhân đang đăng nhập.
        /// </summary>
        public async Task<IReadOnlyList<Cakham>>
            GetExamHistoryAsync(
                IObjectSpace objectSpace)
        {
            Benhnhan currentPatient =
                await GetCurrentPatientAsync(objectSpace);

            if (currentPatient == null)
                return Array.Empty<Cakham>();

            return objectSpace
                .GetObjects<Cakham>()
                .Where(x =>
                    x.BenhNhan != null &&
                    x.BenhNhan.Oid == currentPatient.Oid
                )
                .OrderByDescending(x => x.NgayCT)
                .ToList();
        }

        /// <summary>
        /// Lấy chi tiết một ca khám và kiểm tra ca khám
        /// có thuộc bệnh nhân hiện tại hay không.
        /// </summary>
        public async Task<Cakham> GetOwnedExamAsync(
        IObjectSpace objectSpace,
        int examOid)
            {
                Benhnhan currentPatient =
                    await GetCurrentPatientAsync(objectSpace);

                if (currentPatient == null)
                {
                    return null;
                }

                return objectSpace
                    .GetObjects<Cakham>()
                    .FirstOrDefault(x =>
                        x.Oid == examOid &&
                        x.BenhNhan != null &&
                        x.BenhNhan.Oid ==
                            currentPatient.Oid
                    );
            }

        /// <summary>
        /// Lấy danh sách đơn thuốc của bệnh nhân hiện tại.
        /// </summary>
        public async Task<IReadOnlyList<DonThuoc>>
            GetPrescriptionHistoryAsync(
                IObjectSpace objectSpace)
        {
            Benhnhan currentPatient =
                await GetCurrentPatientAsync(objectSpace);

            if (currentPatient == null)
                return Array.Empty<DonThuoc>();

            return objectSpace
                .GetObjects<DonThuoc>()
                .Where(x =>
                    x.BenhNhan != null &&
                    x.BenhNhan.Oid == currentPatient.Oid
                )
                .OrderByDescending(x => x.NgayKeDon)
                .ToList();
        }

        /// <summary>
        /// Lấy một đơn thuốc nếu đơn đó thuộc bệnh nhân hiện tại.
        /// </summary>
        public async Task<DonThuoc>
            GetOwnedPrescriptionAsync(
                IObjectSpace objectSpace,
                int prescriptionOid)
        {
            Benhnhan currentPatient =
                await GetCurrentPatientAsync(objectSpace);

            if (currentPatient == null)
                return null;

            return objectSpace
                .GetObjects<DonThuoc>()
                .FirstOrDefault(x =>
                    x.Oid == prescriptionOid &&
                    x.BenhNhan != null &&
                    x.BenhNhan.Oid == currentPatient.Oid
                );
        }

        /// <summary>
        /// Lấy các phiếu xuất thuốc của bệnh nhân hiện tại.
        /// </summary>
        public async Task<IReadOnlyList<PhieuXuatKho>>
            GetExportHistoryAsync(
                IObjectSpace objectSpace)
        {
            Benhnhan currentPatient =
                await GetCurrentPatientAsync(objectSpace);

            if (currentPatient == null)
                return Array.Empty<PhieuXuatKho>();

            return objectSpace
                .GetObjects<PhieuXuatKho>()
                .Where(x =>
                    x.BenhNhan != null &&
                    x.BenhNhan.Oid == currentPatient.Oid
                )
                .OrderByDescending(x => x.NgayXuat)
                .ToList();
        }

        /// <summary>
        /// Lấy danh sách hóa đơn của bệnh nhân hiện tại.
        /// </summary>
        public async Task<IReadOnlyList<HoaDonBenhNhan>>
            GetInvoiceHistoryAsync(
                IObjectSpace objectSpace)
        {
            Benhnhan currentPatient =
                await GetCurrentPatientAsync(objectSpace);

            if (currentPatient == null)
                return Array.Empty<HoaDonBenhNhan>();

            return objectSpace
                .GetObjects<HoaDonBenhNhan>()
                .Where(x =>
                    x.BenhNhan != null &&
                    x.BenhNhan.Oid == currentPatient.Oid
                )
                .OrderByDescending(x => x.NgayLap)
                .ToList();
        }

        /// <summary>
        /// Kiểm tra nhanh tài khoản hiện tại đã được liên kết
        /// với hồ sơ bệnh nhân hay chưa.
        /// </summary>
        public async Task<bool> HasLinkedPatientAsync(
            IObjectSpace objectSpace)
        {
            Benhnhan currentPatient =
                await GetCurrentPatientAsync(objectSpace);

            return currentPatient != null;
        }
    }
}