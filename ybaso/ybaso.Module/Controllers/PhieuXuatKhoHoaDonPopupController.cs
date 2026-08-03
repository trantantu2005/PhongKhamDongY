using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class PhieuXuatKhoHoaDonPopupController
        : ObjectViewController<DetailView, PhieuXuatKho>
    {
        private readonly SimpleAction xacNhanVaXemHoaDonAction;

        public PhieuXuatKhoHoaDonPopupController()
        {
            xacNhanVaXemHoaDonAction = new SimpleAction(
                this,
                "PhieuXuatKho_XacNhanVaXemHoaDon",
                PredefinedCategory.RecordEdit)
            {
                Caption = "Xác nhận xuất thuốc và xem hóa đơn",
                ImageName = "Action_Grant"
            };

            xacNhanVaXemHoaDonAction.Execute += XacNhanVaXemHoaDonAction_Execute;
        }

        private void XacNhanVaXemHoaDonAction_Execute(
            object sender,
            SimpleActionExecuteEventArgs e)
        {
            if (View?.CurrentObject is not PhieuXuatKho phieu)
                return;

            try
            {
                phieu.XacNhanXuatKho();

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                if (phieu.HoaDon == null)
                {
                    HienThongBao(
                        "Đã xác nhận xuất kho nhưng chưa tạo được hóa đơn. Vui lòng kiểm tra lại file PhieuXuatKho.cs.",
                        InformationType.Error
                    );
                    return;
                }

                IObjectSpace popupObjectSpace =
                    Application.CreateObjectSpace(typeof(HoaDonBenhNhan));

                HoaDonBenhNhan hoaDonPopup =
                    popupObjectSpace.GetObject(phieu.HoaDon);

                DetailView detailView =
                    Application.CreateDetailView(popupObjectSpace, hoaDonPopup);

                detailView.ViewEditMode = ViewEditMode.View;

                e.ShowViewParameters.CreatedView = detailView;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            }
            catch (UserFriendlyException ex)
            {
                HienThongBao(ex.Message, InformationType.Error);
            }
            catch (ValidationException ex)
            {
                ObjectSpace.Rollback();

                HienThongBao(
                    "Không thể xác nhận xuất thuốc vì dữ liệu chưa hợp lệ. " +
                    "Vui lòng kiểm tra lại các trường bắt buộc như nhân viên, bệnh nhân, đơn thuốc, số lượng, giá xuất. " +
                    ex.Message,
                    InformationType.Error
                );
            }
            catch (Exception ex)
            {
                ObjectSpace.Rollback();

                HienThongBao(
                    "Có lỗi khi xác nhận xuất thuốc và tạo hóa đơn: " + ex.Message,
                    InformationType.Error
                );
            }
        }

        private void HienThongBao(string noiDung, InformationType loai)
        {
            Application.ShowViewStrategy.ShowMessage(
                noiDung,
                loai,
                10000,
                InformationPosition.Top
            );
        }
    }
}