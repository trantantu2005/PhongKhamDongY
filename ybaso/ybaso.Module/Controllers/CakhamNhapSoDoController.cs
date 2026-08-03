using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class CakhamNhapSoDoController : ObjectViewController<DetailView, Cakham>
    {
        private readonly SimpleAction nhapSoDoAction;

        public CakhamNhapSoDoController()
        {
            nhapSoDoAction = new SimpleAction(this, "NhapSoDoNhanh", PredefinedCategory.Edit)
            {
                Caption = "Nhập số đo",
                ImageName = "Action_Edit",
                ToolTip = "Mở form nhập số đo 12 huyệt. Sau khi cập nhật, dữ liệu sẽ xuống tab Số đo và tự phân tích."
            };

            nhapSoDoAction.Execute += NhapSoDoAction_Execute;
        }

        private void NhapSoDoAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Cakham caKham = ViewCurrentObject;

            if (caKham == null)
            {
                HienThongBaoCanhBao("Không tìm thấy ca khám hiện tại.");
                return;
            }

            // Trường hợp người dùng bấm nhầm Nhập số đo khi chưa chọn bệnh nhân
            if (caKham.BenhNhan == null)
            {
                HienThongBaoCanhBao("Vui lòng chọn bệnh nhân trước khi nhập số đo.");
                return;
            }

            /*
             * Nếu ca khám mới chưa lưu:
             * Tự lưu trước để có khóa chính, sau đó mới mở popup nhập số đo.
             */
            if (ObjectSpace.IsNewObject(caKham))
            {
                ObjectSpace.CommitChanges();
                caKham = ViewCurrentObject;
            }
            else if (ObjectSpace.ModifiedObjects.Count > 0)
            {
                ObjectSpace.CommitChanges();
                caKham = ViewCurrentObject;
            }

            object caKhamKey = ObjectSpace.GetKeyValue(caKham);

            if (caKhamKey == null)
            {
                HienThongBaoCanhBao("Không xác định được khóa chính của ca khám. Vui lòng bấm Save rồi thử lại.");
                return;
            }

            IObjectSpace popupObjectSpace = Application.CreateObjectSpace(typeof(CakhamNhapSoDoForm));
            CakhamNhapSoDoForm form = popupObjectSpace.CreateObject<CakhamNhapSoDoForm>();

            form.NapTuCaKham(caKham);

            DetailView detailView = Application.CreateDetailView(popupObjectSpace, form);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

            e.ShowViewParameters.CreatedView = detailView;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;

            DialogController dialogController = Application.CreateController<DialogController>();
            dialogController.AcceptAction.Caption = "Cập nhật";
            dialogController.CancelAction.Caption = "Đóng";

            dialogController.Accepting += (s, args) =>
            {
                CakhamNhapSoDoForm currentForm = detailView.CurrentObject as CakhamNhapSoDoForm;

                if (currentForm == null)
                {
                    HienThongBaoCanhBao("Không lấy được dữ liệu từ form nhập số đo.");
                    return;
                }

                IObjectSpace updateObjectSpace = Application.CreateObjectSpace(typeof(Cakham));

                try
                {
                    Cakham caKhamCanCapNhat = updateObjectSpace.GetObjectByKey<Cakham>(caKhamKey);

                    if (caKhamCanCapNhat == null)
                    {
                        HienThongBaoCanhBao("Không tìm thấy ca khám cần cập nhật.");
                        return;
                    }

                    currentForm.GhiVaoCaKham(caKhamCanCapNhat);

                    updateObjectSpace.SetModified(caKhamCanCapNhat);
                    updateObjectSpace.CommitChanges();

                    ObjectSpace.Refresh();
                    View.Refresh();

                    Application.ShowViewStrategy.ShowMessage(
                        "Đã cập nhật số đo và phân tích thành công.",
                        InformationType.Success,
                        4000,
                        InformationPosition.Top
                    );
                }
                catch
                {
                    updateObjectSpace.Rollback();
                    throw;
                }
                finally
                {
                    updateObjectSpace.Dispose();
                }
            };

            e.ShowViewParameters.Controllers.Add(dialogController);
        }

        private void HienThongBaoCanhBao(string noiDung)
        {
            Application.ShowViewStrategy.ShowMessage(
                noiDung,
                InformationType.Warning,
                5000,
                InformationPosition.Top
            );
        }
    }
}