using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class DanhMucThuocSaveMessageController
        : ObjectViewController<DetailView, DanhMucThuoc>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            if (View?.CurrentObject is not DanhMucThuoc thuoc)
                return;

            // Chỉ kiểm tra khi tạo mới thang thuốc.
            // Nếu sửa tên, sửa mô tả, sửa giá bán của thuốc cũ thì không trừ dược liệu lại.
            if (!ObjectSpace.IsNewObject(thuoc))
                return;

            string thongBaoLoi = LayThongBaoLoiDuocLieu(thuoc);

            if (string.IsNullOrWhiteSpace(thongBaoLoi))
                return;

            // Hủy lưu, không cho app văng lỗi.
            e.Cancel = true;

            // Hiển thị thông báo trực tiếp trên màn hình người dùng.
            Application.ShowViewStrategy.ShowMessage(
                thongBaoLoi,
                InformationType.Error,
                8000,
                InformationPosition.Top
            );
        }

        private static string LayThongBaoLoiDuocLieu(DanhMucThuoc thuoc)
        {
            if (thuoc.SoLuongTon <= 0)
                return "Vui lòng nhập số lượng thang thuốc cần tạo.";

            var thanhPhans = thuoc.ThanhPhanThuocs
                .Where(x => !x.IsDeleted && x.DuocLieu != null)
                .ToList();

            if (thanhPhans.Count == 0)
            {
                return "Vui lòng chọn ít nhất một dược liệu trong thành phần thuốc.";
            }

            foreach (ThanhPhanThuoc thanhPhan in thanhPhans)
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * thuoc.SoLuongTon;

                if (thanhPhan.DuocLieu.SoLuongTon < soLuongCanDung)
                {
                    return
                        $"Dược liệu '{thanhPhan.DuocLieu.TenDuocLieu}' không đủ tồn kho. " +
                        $"Cần {soLuongCanDung:n0}, hiện còn {thanhPhan.DuocLieu.SoLuongTon:n0}. " +
                        $"Vui lòng nhập thêm dược liệu hoặc giảm số lượng thang thuốc cần tạo.";
                }
            }

            return string.Empty;
        }
    }
}