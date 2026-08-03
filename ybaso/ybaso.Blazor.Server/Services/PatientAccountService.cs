using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Components.Authorization;
using ybaso.Module.BusinessObjects;

namespace ybaso.Blazor.Server.Services
{
    public sealed class PatientAccountService
    {
        private const string PatientRoleName =
            "BenhNhan";

        private readonly
            INonSecuredObjectSpaceFactory
            objectSpaceFactory;

        private readonly
            AuthenticationStateProvider
            authenticationStateProvider;

        public PatientAccountService(
            INonSecuredObjectSpaceFactory
                objectSpaceFactory,
            AuthenticationStateProvider
                authenticationStateProvider)
        {
            this.objectSpaceFactory =
                objectSpaceFactory;

            this.authenticationStateProvider =
                authenticationStateProvider;
        }

        /// <summary>
        /// Tạo tên đăng nhập mặc định:
        /// BN000001, BN000002...
        /// </summary>
        public static string CreateDefaultUserName(
            int patientOid)
        {
            return patientOid > 0
                ? $"BN{patientOid:000000}"
                : string.Empty;
        }

        /// <summary>
        /// Lấy trạng thái tài khoản của bệnh nhân.
        /// </summary>
        public async Task<PatientAccountInfo>
            GetInfoAsync(
                int patientOid)
        {
            await EnsureStaffOrAdminAsync();

            if (patientOid <= 0)
            {
                return PatientAccountInfo.NotFound(
                    "Bệnh nhân chưa được lưu."
                );
            }

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<
                        ApplicationUser
                    >();

            Benhnhan patient =
                objectSpace.GetObjectByKey<
                    Benhnhan
                >(patientOid);

            if (patient == null)
            {
                return PatientAccountInfo.NotFound(
                    "Không tìm thấy hồ sơ bệnh nhân."
                );
            }

            ApplicationUser account =
                patient.TaiKhoanDangNhap;

            if (account == null)
            {
                return new PatientAccountInfo(
                    PatientFound: true,
                    HasAccount: false,
                    UserName:
                        CreateDefaultUserName(
                            patientOid
                        ),
                    IsActive: false,
                    Message:
                        "Bệnh nhân chưa có tài khoản."
                );
            }

            return new PatientAccountInfo(
                PatientFound: true,
                HasAccount: true,
                UserName:
                    account.UserName
                    ?? string.Empty,
                IsActive:
                    account.IsActive,
                Message:
                    account.IsActive
                        ? "Tài khoản đang hoạt động."
                        : "Tài khoản đang bị khóa."
            );
        }

        /// <summary>
        /// Tạo tài khoản mới và liên kết
        /// với hồ sơ bệnh nhân.
        /// </summary>
        public async Task<PatientAccountResult>
            CreateAsync(
                int patientOid,
                string userName,
                string password,
                string confirmPassword)
        {
            await EnsureStaffOrAdminAsync();

            userName =
                (userName ?? string.Empty)
                    .Trim();

            string validationError =
                ValidateAccount(
                    patientOid,
                    userName,
                    password,
                    confirmPassword
                );

            if (!string.IsNullOrWhiteSpace(
                    validationError))
            {
                return PatientAccountResult.Fail(
                    validationError
                );
            }

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<
                        ApplicationUser
                    >();

            ApplicationUser createdUser =
                null;

            bool userCommitted =
                false;

            try
            {
                Benhnhan patient =
                    objectSpace.GetObjectByKey<
                        Benhnhan
                    >(patientOid);

                if (patient == null)
                {
                    return PatientAccountResult.Fail(
                        "Không tìm thấy hồ sơ bệnh nhân."
                    );
                }

                if (patient.TaiKhoanDangNhap != null)
                {
                    return PatientAccountResult.Fail(
                        "Bệnh nhân đã có tài khoản " +
                        $"{patient.TaiKhoanDangNhap.UserName}."
                    );
                }

                ApplicationUser duplicateUser =
                    objectSpace
                        .FirstOrDefault<
                            ApplicationUser
                        >(
                            item =>
                                item.UserName ==
                                userName
                        );

                if (duplicateUser != null)
                {
                    return PatientAccountResult.Fail(
                        "Tên đăng nhập đã tồn tại. " +
                        "Vui lòng chọn tên khác."
                    );
                }

                PermissionPolicyRole patientRole =
                    objectSpace
                        .FirstOrDefault<
                            PermissionPolicyRole
                        >(
                            item =>
                                item.Name ==
                                PatientRoleName
                        );

                if (patientRole == null)
                {
                    return PatientAccountResult.Fail(
                        "Không tìm thấy vai trò " +
                        "BenhNhan trong hệ thống."
                    );
                }

                createdUser =
                    objectSpace.CreateObject<
                        ApplicationUser
                    >();

                createdUser.UserName =
                    userName;

                createdUser.IsActive =
                    true;

                createdUser
                    .ChangePasswordOnFirstLogon =
                    false;

                createdUser.SetPassword(
                    password
                );

                /*
                 * Phải lưu trước để tài khoản
                 * có khóa chính.
                 */
                objectSpace.CommitChanges();

                userCommitted =
                    true;

                bool hasPasswordLogin =
                    createdUser.UserLogins.Any(
                        login =>
                            login.LoginProviderName ==
                            SecurityDefaults
                                .PasswordAuthentication
                    );

                if (!hasPasswordLogin)
                {
                    ((ISecurityUserWithLoginInfo)
                        createdUser)
                        .CreateUserLoginInfo(
                            SecurityDefaults
                                .PasswordAuthentication,
                            objectSpace
                                .GetKeyValueAsString(
                                    createdUser
                                )
                        );
                }

                if (!createdUser.Roles.Contains(
                        patientRole))
                {
                    createdUser.Roles.Add(
                        patientRole
                    );
                }

                /*
                 * Liên kết tài khoản với đúng
                 * hồ sơ bệnh nhân.
                 */
                patient.TaiKhoanDangNhap =
                    createdUser;

                objectSpace.CommitChanges();

                return PatientAccountResult.Success(
                    $"Đã tạo tài khoản {userName}.",
                    userName
                );
            }
            catch (Exception exception)
            {
                /*
                 * Nếu tài khoản đã được lưu nhưng
                 * liên kết bệnh nhân thất bại,
                 * cố gắng xóa tài khoản tạo dở.
                 */
                if (userCommitted &&
                    createdUser != null)
                {
                    TryDeleteCreatedUser(
                        objectSpace,
                        createdUser
                    );
                }

                return PatientAccountResult.Fail(
                    "Không thể tạo tài khoản: " +
                    GetLastError(exception)
                );
            }
        }

        /// <summary>
        /// Đặt lại mật khẩu và mở khóa tài khoản.
        /// </summary>
        public async Task<PatientAccountResult>
            ResetPasswordAsync(
                int patientOid,
                string password,
                string confirmPassword)
        {
            await EnsureStaffOrAdminAsync();

            string validationError =
                ValidatePassword(
                    password,
                    confirmPassword
                );

            if (!string.IsNullOrWhiteSpace(
                    validationError))
            {
                return PatientAccountResult.Fail(
                    validationError
                );
            }

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<
                        ApplicationUser
                    >();

            try
            {
                Benhnhan patient =
                    objectSpace.GetObjectByKey<
                        Benhnhan
                    >(patientOid);

                ApplicationUser account =
                    patient?.TaiKhoanDangNhap;

                if (account == null)
                {
                    return PatientAccountResult.Fail(
                        "Bệnh nhân chưa có tài khoản."
                    );
                }

                account.IsActive =
                    true;

                account
                    .ChangePasswordOnFirstLogon =
                    false;

                account.SetPassword(
                    password
                );

                objectSpace.SetModified(
                    account
                );

                objectSpace.CommitChanges();

                return PatientAccountResult.Success(
                    "Đã đặt lại mật khẩu cho " +
                    $"{account.UserName}.",
                    account.UserName
                    ?? string.Empty
                );
            }
            catch (Exception exception)
            {
                return PatientAccountResult.Fail(
                    "Không thể đặt lại mật khẩu: " +
                    GetLastError(exception)
                );
            }
        }

        /// <summary>
        /// Chỉ nhân viên hoặc quản trị viên
        /// mới được quản lý tài khoản bệnh nhân.
        /// </summary>
        private async Task
            EnsureStaffOrAdminAsync()
        {
            AuthenticationState state =
                await authenticationStateProvider
                    .GetAuthenticationStateAsync();

            ClaimsPrincipal user =
                state.User;

            bool allowed =
                user.Identity?.IsAuthenticated ==
                true
                &&
                (
                    user.IsInRole("NhanVien")
                    ||
                    user.IsInRole("Admin")
                );

            if (!allowed)
            {
                throw new
                    UnauthorizedAccessException(
                        "Bạn không có quyền quản lý " +
                        "tài khoản bệnh nhân."
                    );
            }
        }

        private static string ValidateAccount(
            int patientOid,
            string userName,
            string password,
            string confirmPassword)
        {
            if (patientOid <= 0)
            {
                return
                    "Vui lòng lưu bệnh nhân trước.";
            }

            if (string.IsNullOrWhiteSpace(
                    userName))
            {
                return
                    "Vui lòng nhập tên đăng nhập.";
            }

            if (userName.Length < 4)
            {
                return
                    "Tên đăng nhập phải có ít nhất " +
                    "4 ký tự.";
            }

            if (userName.Any(
                    char.IsWhiteSpace))
            {
                return
                    "Tên đăng nhập không được chứa " +
                    "khoảng trắng.";
            }

            return ValidatePassword(
                password,
                confirmPassword
            );
        }

        private static string ValidatePassword(
            string password,
            string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(
                    password))
            {
                return
                    "Vui lòng nhập mật khẩu.";
            }

            if (password.Length < 6)
            {
                return
                    "Mật khẩu phải có ít nhất " +
                    "6 ký tự.";
            }

            if (!string.Equals(
                    password,
                    confirmPassword,
                    StringComparison.Ordinal))
            {
                return
                    "Mật khẩu nhập lại không khớp.";
            }

            return string.Empty;
        }

        private static void TryDeleteCreatedUser(
            IObjectSpace objectSpace,
            ApplicationUser user)
        {
            try
            {
                objectSpace.Rollback();

                objectSpace.Delete(user);

                objectSpace.CommitChanges();
            }
            catch (Exception cleanupException)
            {
                Console.Error.WriteLine(
                    "Không thể xóa tài khoản " +
                    "tạo dở: " +
                    cleanupException
                );
            }
        }

        private static string GetLastError(
            Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception =
                    exception.InnerException;
            }

            return exception.Message;
        }
    }

    public sealed record PatientAccountInfo(
        bool PatientFound,
        bool HasAccount,
        string UserName,
        bool IsActive,
        string Message)
    {
        public static PatientAccountInfo
            NotFound(
                string message)
        {
            return new PatientAccountInfo(
                PatientFound: false,
                HasAccount: false,
                UserName: string.Empty,
                IsActive: false,
                Message: message
            );
        }
    }

    public sealed record PatientAccountResult(
        bool Succeeded,
        string Message,
        string UserName)
    {
        public static PatientAccountResult
            Success(
                string message,
                string userName)
        {
            return new PatientAccountResult(
                Succeeded: true,
                Message: message,
                UserName: userName
            );
        }

        public static PatientAccountResult
            Fail(
                string message)
        {
            return new PatientAccountResult(
                Succeeded: false,
                Message: message,
                UserName: string.Empty
            );
        }
    }
}