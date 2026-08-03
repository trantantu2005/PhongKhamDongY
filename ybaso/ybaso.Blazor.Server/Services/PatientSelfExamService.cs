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
    /// Service xử lý ca khám do chính bệnh nhân tạo.
    ///
    /// Service sử dụng ObjectSpace không bảo mật nhưng mọi thao tác
    /// đều bắt buộc kiểm tra lại:
    /// - Tài khoản đã đăng nhập.
    /// - Tài khoản có vai trò BenhNhan.
    /// - Hồ sơ bệnh nhân liên kết với tài khoản.
    /// - Ca khám phải thuộc đúng hồ sơ đó.
    ///
    /// Không trả Object XPO ra ngoài ObjectSpace.
    /// </summary>
    public sealed class PatientSelfExamService
    {
        private readonly INonSecuredObjectSpaceFactory
            objectSpaceFactory;

        private readonly AuthenticationStateProvider
            authenticationStateProvider;

        public PatientSelfExamService(
            INonSecuredObjectSpaceFactory objectSpaceFactory,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.objectSpaceFactory =
                objectSpaceFactory;

            this.authenticationStateProvider =
                authenticationStateProvider;
        }

        /// <summary>
        /// Tạo ca khám mới cho chính bệnh nhân đang đăng nhập.
        /// </summary>
        public async Task<int> CreateExamAsync(
            string lyDoKham,
            double chieuCao,
            double canNang,
            double nhietDoMoiTruong)
        {
            ValidateExamInformation(
                lyDoKham,
                chieuCao,
                canNang,
                nhietDoMoiTruong
            );

            string userName =
                await GetAuthorizedUserNameAsync();

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<Cakham>();

            Benhnhan patient =
                FindCurrentPatient(
                    objectSpace,
                    userName
                );

            bool hasPreviousExam =
                objectSpace
                    .GetObjects<Cakham>()
                    .Any(x =>
                        x.BenhNhan != null &&
                        x.BenhNhan.Oid == patient.Oid
                    );

            Cakham exam =
                objectSpace.CreateObject<Cakham>();

            /*
             * Các trường này do hệ thống tự thiết lập.
             * Bệnh nhân không được chọn bệnh nhân khác,
             * bác sĩ hoặc trạng thái.
             */
            exam.BenhNhan = patient;
            exam.Bacsi = null;
            exam.NgayCT = DateTime.Now;
            exam.Tuoi = CalculateAge(patient.Ngaysinh);

            exam.Chieucao = chieuCao;
            exam.Cannang = canNang;
            exam.NhietDoMoiTruong =
                nhietDoMoiTruong;

            exam.LydoKham =
                lyDoKham.Trim();

            exam.LoaiLuotKham =
                hasPreviousExam
                    ? LoaiLuotKham.BenhNhanCu
                    : LoaiLuotKham.BenhNhanMoi;

            exam.LoaiCakham =
                LoaiCaKham.TuXa;

            exam.TrangThai =
                TrangThaiCaKham.ChoKham;

            EnsureDefaultMeasurements(
                objectSpace,
                exam
            );

            objectSpace.CommitChanges();

            return exam.Oid;
        }

        /// <summary>
        /// Lưu 12 cặp số đo của ca khám thuộc bệnh nhân hiện tại.
        /// </summary>
        public async Task SaveMeasurementsAsync(
            int examOid,
            IReadOnlyCollection<
                PatientExamMeasurementInput
            > measurements)
        {
            ValidateMeasurements(measurements);

            string userName =
                await GetAuthorizedUserNameAsync();

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<Cakham>();

            Cakham exam =
                FindOwnedExam(
                    objectSpace,
                    userName,
                    examOid
                );

            EnsureExamCanBeEdited(exam);

            EnsureDefaultMeasurements(
                objectSpace,
                exam
            );

            foreach (
                PatientExamMeasurementInput input
                in measurements)
            {
                CakhamSoDoChiTiet row =
                    exam.SoDoChiTiets
                        .FirstOrDefault(x =>
                            x.TT == input.TT
                        );

                if (row == null)
                {
                    throw new UserFriendlyException(
                        $"Không tìm thấy dòng số đo thứ {input.TT}."
                    );
                }

                row.Trai = input.Trai;
                row.Phai = input.Phai;
            }

            exam.DanhDauCanPhanTich();

            objectSpace.CommitChanges();
        }

        /// <summary>
        /// Phân tích số đo và sinh kết quả chẩn đoán,
        /// ngũ tà, bệnh chứng và pháp trị.
        /// </summary>
        public async Task AnalyzeExamAsync(
            int examOid)
        {
            string userName =
                await GetAuthorizedUserNameAsync();

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<Cakham>();

            Cakham exam =
                FindOwnedExam(
                    objectSpace,
                    userName,
                    examOid
                );

            EnsureExamCanBeEdited(exam);
            EnsureAllMeasurementsEntered(exam);

            exam.CapNhatPhanTich();

            /*
             * Sau khi phân tích, ca vẫn ở trạng thái Chờ khám.
             * Bác sĩ sẽ kiểm tra và xác nhận chuyên môn.
             */
            exam.TrangThai =
                TrangThaiCaKham.ChoKham;

            objectSpace.CommitChanges();
        }

        /// <summary>
        /// Cập nhật thông tin cơ bản của ca khám do bệnh nhân tạo.
        /// Không cho thay đổi bệnh nhân, bác sĩ hoặc trạng thái.
        /// </summary>
        public async Task UpdateExamInformationAsync(
            int examOid,
            string lyDoKham,
            double chieuCao,
            double canNang,
            double nhietDoMoiTruong)
        {
            ValidateExamInformation(
                lyDoKham,
                chieuCao,
                canNang,
                nhietDoMoiTruong
            );

            string userName =
                await GetAuthorizedUserNameAsync();

            using IObjectSpace objectSpace =
                objectSpaceFactory
                    .CreateNonSecuredObjectSpace<Cakham>();

            Cakham exam =
                FindOwnedExam(
                    objectSpace,
                    userName,
                    examOid
                );

            EnsureExamCanBeEdited(exam);

            exam.LydoKham =
                lyDoKham.Trim();

            exam.Chieucao =
                chieuCao;

            exam.Cannang =
                canNang;

            exam.NhietDoMoiTruong =
                nhietDoMoiTruong;

            /*
             * Luôn giữ các giá trị hệ thống.
             */
            exam.Bacsi = null;
            exam.LoaiCakham =
                LoaiCaKham.TuXa;
            exam.TrangThai =
                TrangThaiCaKham.ChoKham;

            objectSpace.CommitChanges();
        }

        private async Task<string>
            GetAuthorizedUserNameAsync()
        {
            var authenticationState =
                await authenticationStateProvider
                    .GetAuthenticationStateAsync();

            var principal =
                authenticationState.User;

            if (principal?.Identity == null ||
                !principal.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException(
                    "Bạn chưa đăng nhập."
                );
            }

            if (!principal.IsInRole("BenhNhan"))
            {
                throw new UnauthorizedAccessException(
                    "Tài khoản hiện tại không có vai trò Bệnh nhân."
                );
            }

            string userName =
                principal.Identity.Name
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new UnauthorizedAccessException(
                    "Không xác định được tài khoản đăng nhập."
                );
            }

            return userName;
        }

        private static Benhnhan FindCurrentPatient(
            IObjectSpace objectSpace,
            string userName)
        {
            Benhnhan patient =
                objectSpace
                    .GetObjects<Benhnhan>()
                    .FirstOrDefault(x =>
                        x.TaiKhoanDangNhap != null &&
                        x.TaiKhoanDangNhap.UserName ==
                            userName
                    );

            if (patient == null)
            {
                throw new UserFriendlyException(
                    "Tài khoản của bạn chưa được liên kết " +
                    "với hồ sơ bệnh nhân."
                );
            }

            return patient;
        }

        private static Cakham FindOwnedExam(
            IObjectSpace objectSpace,
            string userName,
            int examOid)
        {
            Cakham exam =
                objectSpace
                    .GetObjects<Cakham>()
                    .FirstOrDefault(x =>
                        x.Oid == examOid &&
                        x.BenhNhan != null &&
                        x.BenhNhan.TaiKhoanDangNhap != null &&
                        x.BenhNhan.TaiKhoanDangNhap.UserName ==
                            userName
                    );

            if (exam == null)
            {
                throw new UnauthorizedAccessException(
                    "Ca khám không tồn tại hoặc không thuộc hồ sơ của bạn."
                );
            }

            return exam;
        }

        private static void EnsureExamCanBeEdited(
            Cakham exam)
        {
            if (exam == null)
            {
                throw new UserFriendlyException(
                    "Không tìm thấy ca khám."
                );
            }

            /*
             * Khi bác sĩ đã bắt đầu hoặc hoàn thành khám,
             * bệnh nhân không còn được thay đổi số đo.
             */
            if (exam.TrangThai ==
                    TrangThaiCaKham.DangKham ||
                exam.TrangThai ==
                    TrangThaiCaKham.DaKham ||
                exam.TrangThai ==
                    TrangThaiCaKham.DaHuy)
            {
                throw new UserFriendlyException(
                    "Ca khám đã được phòng khám tiếp nhận " +
                    "nên không thể chỉnh sửa."
                );
            }
        }

        private static void ValidateExamInformation(
            string lyDoKham,
            double chieuCao,
            double canNang,
            double nhietDoMoiTruong)
        {
            if (string.IsNullOrWhiteSpace(lyDoKham))
            {
                throw new UserFriendlyException(
                    "Vui lòng nhập lý do khám."
                );
            }

            if (lyDoKham.Trim().Length > 2000)
            {
                throw new UserFriendlyException(
                    "Lý do khám không được vượt quá 2.000 ký tự."
                );
            }

            if (chieuCao < 50 ||
                chieuCao > 250)
            {
                throw new UserFriendlyException(
                    "Chiều cao phải nằm trong khoảng 50 đến 250 cm."
                );
            }

            if (canNang < 5 ||
                canNang > 300)
            {
                throw new UserFriendlyException(
                    "Cân nặng phải nằm trong khoảng 5 đến 300 kg."
                );
            }

            if (nhietDoMoiTruong < 0 ||
                nhietDoMoiTruong > 50)
            {
                throw new UserFriendlyException(
                    "Nhiệt độ môi trường phải nằm trong khoảng 0 đến 50°C."
                );
            }
        }

        private static void ValidateMeasurements(
            IReadOnlyCollection<
                PatientExamMeasurementInput
            > measurements)
        {
            if (measurements == null ||
                measurements.Count != 12)
            {
                throw new UserFriendlyException(
                    "Vui lòng nhập đầy đủ 12 cặp số đo."
                );
            }

            int[] expectedNumbers =
                Enumerable
                    .Range(1, 12)
                    .ToArray();

            int[] actualNumbers =
                measurements
                    .Select(x => x.TT)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();

            if (!expectedNumbers.SequenceEqual(
                actualNumbers))
            {
                throw new UserFriendlyException(
                    "Danh sách số đo phải có đủ thứ tự từ 1 đến 12."
                );
            }

            foreach (
                PatientExamMeasurementInput input
                in measurements)
            {
                if (input.Trai <= 0 ||
                    input.Phai <= 0)
                {
                    throw new UserFriendlyException(
                        $"Số đo tại dòng {input.TT} phải lớn hơn 0."
                    );
                }

                if (input.Trai > 500 ||
                    input.Phai > 500)
                {
                    throw new UserFriendlyException(
                        $"Số đo tại dòng {input.TT} không hợp lệ."
                    );
                }
            }
        }

        private static void EnsureAllMeasurementsEntered(
            Cakham exam)
        {
            List<CakhamSoDoChiTiet> measurements =
                exam.SoDoChiTiets
                    .OrderBy(x => x.TT)
                    .ToList();

            if (measurements.Count != 12)
            {
                throw new UserFriendlyException(
                    "Ca khám chưa có đủ 12 dòng số đo."
                );
            }

            CakhamSoDoChiTiet invalidRow =
                measurements.FirstOrDefault(x =>
                    x.Trai <= 0 ||
                    x.Phai <= 0
                );

            if (invalidRow != null)
            {
                throw new UserFriendlyException(
                    $"Vui lòng nhập đầy đủ số đo tại " +
                    $"huyệt {invalidRow.TinhHuyet}."
                );
            }
        }

        private static void EnsureDefaultMeasurements(
            IObjectSpace objectSpace,
            Cakham exam)
        {
            var defaults = new[]
            {
                new
                {
                    TT = 1,
                    Name = "Tiểu",
                    Group = NhomChiDo.ChiTren
                },
                new
                {
                    TT = 2,
                    Name = "Tâm",
                    Group = NhomChiDo.ChiTren
                },
                new
                {
                    TT = 3,
                    Name = "Tam",
                    Group = NhomChiDo.ChiTren
                },
                new
                {
                    TT = 4,
                    Name = "Bào",
                    Group = NhomChiDo.ChiTren
                },
                new
                {
                    TT = 5,
                    Name = "Đại",
                    Group = NhomChiDo.ChiTren
                },
                new
                {
                    TT = 6,
                    Name = "Phế",
                    Group = NhomChiDo.ChiTren
                },
                new
                {
                    TT = 7,
                    Name = "Bàng",
                    Group = NhomChiDo.ChiDuoi
                },
                new
                {
                    TT = 8,
                    Name = "Thận",
                    Group = NhomChiDo.ChiDuoi
                },
                new
                {
                    TT = 9,
                    Name = "Đảm",
                    Group = NhomChiDo.ChiDuoi
                },
                new
                {
                    TT = 10,
                    Name = "Vị",
                    Group = NhomChiDo.ChiDuoi
                },
                new
                {
                    TT = 11,
                    Name = "Can",
                    Group = NhomChiDo.ChiDuoi
                },
                new
                {
                    TT = 12,
                    Name = "Tỳ",
                    Group = NhomChiDo.ChiDuoi
                }
            };

            foreach (var item in defaults)
            {
                CakhamSoDoChiTiet row =
                    exam.SoDoChiTiets
                        .FirstOrDefault(x =>
                            x.TT == item.TT
                        );

                if (row == null)
                {
                    row =
                        objectSpace
                            .CreateObject<
                                CakhamSoDoChiTiet
                            >();

                    row.Cakham = exam;
                }

                row.TT = item.TT;
                row.TinhHuyet = item.Name;
                row.NhomChi = item.Group;
            }
        }

        private static int CalculateAge(
            DateTime birthDate)
        {
            if (birthDate == DateTime.MinValue)
            {
                return 0;
            }

            DateTime today =
                DateTime.Today;

            int age =
                today.Year -
                birthDate.Year;

            if (birthDate.Date >
                today.AddYears(-age))
            {
                age--;
            }

            return Math.Max(age, 0);
        }
    }

    public sealed class PatientExamMeasurementInput
    {
        public int TT { get; set; }

        public double Trai { get; set; }

        public double Phai { get; set; }
    }
}