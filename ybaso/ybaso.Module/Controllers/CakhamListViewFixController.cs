using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class CakhamListViewFixController : ViewController<ListView>
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            Type objectType = View?.ObjectTypeInfo?.Type;

            if (objectType == null)
                return;

            // Danh sách ca khám
            if (objectType == typeof(Cakham))
            {
                ApplyColumns(new[]
                {
                    "TenCaKham",
                    "TenBenhNhanGon",
                    "NgayCT",
                    "Tuoi",
                    "Bacsi",
                    "LoaiCakham",
                    "TrangThai"
                });

                SetCaption("TenCaKham", "Tên ca khám");
                SetCaption("TenBenhNhanGon", "Bệnh nhân");
                SetCaption("NgayCT", "Ngày khám");
                SetCaption("Tuoi", "Tuổi");
                SetCaption("Bacsi", "Bác sĩ");
                SetCaption("LoaiCakham", "Loại ca khám");
                SetCaption("TrangThai", "Trạng thái");
            }

            // Tab Số đo trong Ca khám
            else if (objectType == typeof(CakhamSoDoChiTiet))
            {
                ApplyColumns(new[]
                {
                    "TT",
                    "TinhHuyet",
                    "TraiDoC",
                    "PhaiDoC"
                });

                SetCaption("TT", "TT");
                SetCaption("TinhHuyet", "Tinh huyệt");
                SetCaption("TraiDoC", "Trái");
                SetCaption("PhaiDoC", "Phải");
            }

            // Tab Chẩn đoán trong Ca khám
            else if (objectType == typeof(CakhamChanDoanTuDong))
            {
                ApplyColumns(new[]
                {
                    "Nhom",
                    "KetQua"
                });

                SetCaption("Nhom", "Nhóm");
                SetCaption("KetQua", "Kết quả");
            }

            // Tab Ngũ tà trong Ca khám
            else if (objectType == typeof(CakhamNguTaTuDong))
            {
                ApplyColumns(new[]
                {
                    "CotTrai",
                    "CotGiua",
                    "CotPhai"
                });

                SetCaption("CotTrai", "");
                SetCaption("CotGiua", "");
                SetCaption("CotPhai", "");
            }

            // Tab Bệnh chứng trong Ca khám
            else if (objectType == typeof(CakhamBenhChungTuDong))
            {
                ApplyColumns(new[]
                {
                    "TenBenhChung",
                    "TrieuChung"
                });

                SetCaption("TenBenhChung", "Bệnh chứng");
                SetCaption("TrieuChung", "Triệu chứng");
            }

            // Tab Pháp trị trong Ca khám
            else if (objectType == typeof(CakhamPhapTriTuDong))
            {
                ApplyColumns(new[]
                {
                    "Huyet",
                    "PhapDieuTri",
                    "PhuongDuoc",
                    "BenhChung"
                });

                SetCaption("Huyet", "Huyệt");
                SetCaption("PhapDieuTri", "Pháp điều trị");
                SetCaption("PhuongDuoc", "Phương dược");
                SetCaption("BenhChung", "Bệnh chứng");
            }

            // Tab Số đo trong Phiếu khám xem trước
            else if (objectType == typeof(PhieuKhamSoDoPreview))
            {
                ApplyColumns(new[]
                {
                    "TT",
                    "NhomChi",
                    "TinhHuyet",
                    "Trai",
                    "TraiDanhGia",
                    "Phai",
                    "PhaiDanhGia",
                    "LechTrungBinh",
                    "ChenhTraiPhai",
                    "Nguong"
                });

                SetCaption("TT", "TT");
                SetCaption("NhomChi", "Nhóm chi");
                SetCaption("TinhHuyet", "Tinh huyệt");
                SetCaption("Trai", "Trái");
                SetCaption("TraiDanhGia", "Trái +/-");
                SetCaption("Phai", "Phải");
                SetCaption("PhaiDanhGia", "Phải +/-");
                SetCaption("LechTrungBinh", "Lệch TB");
                SetCaption("ChenhTraiPhai", "Chênh trái/phải");
                SetCaption("Nguong", "Ngưỡng");
            }

            // Tab Chẩn đoán trong Phiếu khám xem trước
            else if (objectType == typeof(PhieuKhamChanDoanPreview))
            {
                ApplyColumns(new[]
                {
                    "Nhom",
                    "KetQua"
                });

                SetCaption("Nhom", "Nhóm");
                SetCaption("KetQua", "Kết quả");
            }

            // Tab Ngũ tà trong Phiếu khám xem trước
            else if (objectType == typeof(PhieuKhamNguTaPreview))
            {
                ApplyColumns(new[]
                {
                    "CotTrai",
                    "CotGiua",
                    "CotPhai"
                });

                SetCaption("CotTrai", "");
                SetCaption("CotGiua", "");
                SetCaption("CotPhai", "");
            }

            // Tab Bệnh chứng trong Phiếu khám xem trước
            else if (objectType == typeof(PhieuKhamBenhChungPreview))
            {
                ApplyColumns(new[]
                {
                    "TenBenhChung",
                    "TrieuChung"
                });

                SetCaption("TenBenhChung", "Bệnh chứng");
                SetCaption("TrieuChung", "Triệu chứng");
            }

            // Tab Pháp trị trong Phiếu khám xem trước
            else if (objectType == typeof(PhieuKhamPhapTriPreview))
            {
                ApplyColumns(new[]
                {
                    "Huyet",
                    "PhapDieuTri",
                    "PhuongDuoc",
                    "BenhChung"
                });

                SetCaption("Huyet", "Huyệt");
                SetCaption("PhapDieuTri", "Pháp điều trị");
                SetCaption("PhuongDuoc", "Phương dược");
                SetCaption("BenhChung", "Bệnh chứng");
            }
        }

        private void ApplyColumns(string[] visibleNames)
        {
            if (View?.Model?.Columns == null)
                return;

            var visible = new HashSet<string>(visibleNames, StringComparer.OrdinalIgnoreCase);
            int index = 0;

            foreach (object column in View.Model.Columns)
            {
                string id = GetString(column, "Id");
                string propertyName = GetString(column, "PropertyName");
                string name = !string.IsNullOrWhiteSpace(propertyName) ? propertyName : id;

                if (visible.Contains(name))
                {
                    SetValue(column, "Index", index++);
                    SetValue(column, "Visible", true);
                }
                else
                {
                    SetValue(column, "Index", -1);
                    SetValue(column, "Visible", false);
                }
            }
        }

        private void SetCaption(string propertyName, string caption)
        {
            if (View?.Model?.Columns == null)
                return;

            foreach (object column in View.Model.Columns)
            {
                string id = GetString(column, "Id");
                string prop = GetString(column, "PropertyName");

                if (string.Equals(id, propertyName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(prop, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    SetValue(column, "Caption", caption);
                }
            }
        }

        private static string GetString(object obj, string propertyName)
        {
            try
            {
                object value = obj.GetType().GetProperty(propertyName)?.GetValue(obj);
                return value?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void SetValue(object obj, string propertyName, object value)
        {
            try
            {
                var prop = obj.GetType().GetProperty(propertyName);

                if (prop == null || !prop.CanWrite)
                    return;

                prop.SetValue(obj, value);
            }
            catch
            {
                // Nếu phiên bản XAF không hỗ trợ property này thì bỏ qua.
            }
        }
    }
}