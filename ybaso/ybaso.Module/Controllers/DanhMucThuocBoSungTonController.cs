using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class DanhMucThuocBoSungTonController
        : ObjectViewController<DetailView, DanhMucThuoc>
    {
        private readonly SimpleAction boSungTonThuocAction;

        public DanhMucThuocBoSungTonController()
        {
            boSungTonThuocAction = new SimpleAction(
                this,
                "BoSungTonThuoc",
                PredefinedCategory.Edit
            );

            boSungTonThuocAction.Caption = "Bổ sung tồn thuốc";
            boSungTonThuocAction.ImageName = "Action_Grant";
            boSungTonThuocAction.ConfirmationMessage =
                "Bạn có chắc muốn bổ sung số lượng cho thang thuốc này không? " +
                "Hệ thống sẽ trừ dược liệu tương ứng trong kho.";

            boSungTonThuocAction.Execute += BoSungTonThuocAction_Execute;
        }

        private void BoSungTonThuocAction_Execute(
            object sender,
            SimpleActionExecuteEventArgs e)
        {
            if (View?.CurrentObject is not DanhMucThuoc thuoc)
                return;

            decimal soLuongBoSung = thuoc.SoLuongBoSung;

            if (soLuongBoSung <= 0)
            {
                HienThongBao(
                    "Vui lòng nhập số lượng bổ sung lớn hơn 0.",
                    InformationType.Error
                );
                return;
            }

            var thanhPhans = thuoc.ThanhPhanThuocs
                .Where(x => !x.IsDeleted && x.DuocLieu != null)
                .ToList();

            if (thanhPhans.Count == 0)
            {
                HienThongBao(
                    "Thang thuốc này chưa có thành phần dược liệu. Vui lòng khai báo thành phần trước khi bổ sung tồn.",
                    InformationType.Error
                );
                return;
            }

            foreach (ThanhPhanThuoc thanhPhan in thanhPhans)
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * soLuongBoSung;

                if (thanhPhan.DuocLieu.SoLuongTon < soLuongCanDung)
                {
                    HienThongBao(
                        $"Dược liệu '{thanhPhan.DuocLieu.TenDuocLieu}' không đủ tồn kho. " +
                        $"Cần {soLuongCanDung:n0}, hiện còn {thanhPhan.DuocLieu.SoLuongTon:n0}. " +
                        $"Vui lòng nhập thêm dược liệu hoặc giảm số lượng bổ sung.",
                        InformationType.Error
                    );
                    return;
                }
            }

            foreach (ThanhPhanThuoc thanhPhan in thanhPhans)
            {
                decimal soLuongCanDung = thanhPhan.SoLuong * soLuongBoSung;
                thanhPhan.DuocLieu.SoLuongTon -= soLuongCanDung;
            }

            thuoc.SoLuongTon += soLuongBoSung;
            thuoc.SoLuongBoSung = 0;

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            HienThongBao(
                $"Đã bổ sung {soLuongBoSung:n0} {thuoc.DonViTinh} cho thuốc '{thuoc.TenThuoc}'.",
                InformationType.Success
            );
        }

        private void HienThongBao(string noiDung, InformationType loaiThongBao)
        {
            Application.ShowViewStrategy.ShowMessage(
                noiDung,
                loaiThongBao,
                8000,
                InformationPosition.Top
            );
        }
    }
}