using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class DanhMucThuocLuuKhoController
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

            // Chỉ xử lý khi tạo mới thuốc/thang thuốc.
            // Thuốc cũ muốn tăng tồn thì dùng nút "Bổ sung tồn thuốc".
            if (!ObjectSpace.IsNewObject(thuoc))
                return;

            if (thuoc.DaTruDuocLieuKhiTao)
                return;

            // Nếu tạo danh mục thuốc nhưng chưa nhập tồn ban đầu thì cho lưu bình thường.
            if (thuoc.SoLuongTon <= 0)
                return;

            var thanhPhans = thuoc.ThanhPhanThuocs
                .Where(x => !x.IsDeleted)
                .ToList();

            if (thanhPhans.Count == 0)
            {
                HuyLuuVaThongBao(
                    e,
                    "Vui lòng khai báo thành phần dược liệu trước khi tạo tồn thang thuốc."
                );
                return;
            }

            foreach (ThanhPhanThuoc thanhPhan in thanhPhans)
            {
                if (thanhPhan.DuocLieu == null)
                {
                    HuyLuuVaThongBao(
                        e,
                        "Vui lòng chọn dược liệu cho tất cả dòng thành phần."
                    );
                    return;
                }

                if (thanhPhan.SoLuong <= 0)
                {
                    HuyLuuVaThongBao(
                        e,
                        "Số lượng dược liệu trong thành phần phải lớn hơn 0."
                    );
                    return;
                }
            }

            foreach (ThanhPhanThuoc thanhPhan in thanhPhans)
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * thuoc.SoLuongTon;

                if (thanhPhan.DuocLieu.SoLuongTon < soLuongCanDung)
                {
                    HuyLuuVaThongBao(
                        e,
                        $"Dược liệu '{thanhPhan.DuocLieu.TenDuocLieu}' không đủ tồn kho. " +
                        $"Cần {soLuongCanDung:n0}, hiện còn {thanhPhan.DuocLieu.SoLuongTon:n0}. " +
                        "Vui lòng nhập thêm dược liệu hoặc giảm số lượng thang thuốc cần tạo."
                    );
                    return;
                }
            }

            // Nếu đủ dược liệu thì trừ kho dược liệu.
            foreach (ThanhPhanThuoc thanhPhan in thanhPhans)
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * thuoc.SoLuongTon;
                thanhPhan.DuocLieu.SoLuongTon -= soLuongCanDung;
            }

            // Đánh dấu đã trừ để không trừ lại lần sau.
            thuoc.DaTruDuocLieuKhiTao = true;
        }

        private void HuyLuuVaThongBao(CancelEventArgs e, string noiDung)
        {
            e.Cancel = true;

            Application.ShowViewStrategy.ShowMessage(
                noiDung,
                InformationType.Error,
                8000,
                InformationPosition.Top
            );
        }
    }
}