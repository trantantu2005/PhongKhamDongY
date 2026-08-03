using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System.ComponentModel;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    [DomainComponent]
    [XafDisplayName("Đặt mật khẩu")]
    public class DatMatKhauNguoiDungForm
    {
        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng nhập mật khẩu mới.")]
        [XafDisplayName("Mật khẩu mới")]
        [PasswordPropertyText(true)]
        public string MatKhauMoi { get; set; }

        [RuleRequiredField(DefaultContexts.Save, CustomMessageTemplate = "Vui lòng nhập lại mật khẩu.")]
        [XafDisplayName("Nhập lại mật khẩu")]
        [PasswordPropertyText(true)]
        public string NhapLaiMatKhau { get; set; }
    }

    public class DatMatKhauApplicationUserController : ObjectViewController<DetailView, ApplicationUser>
    {
        private readonly PopupWindowShowAction datMatKhauAction;

        public DatMatKhauApplicationUserController()
        {
            datMatKhauAction = new PopupWindowShowAction(
                this,
                "DatMatKhauNguoiDung",
                PredefinedCategory.Edit
            )
            {
                Caption = "Đặt mật khẩu",
                ImageName = "Action_ResetPassword",
                PaintStyle = ActionItemPaintStyle.CaptionAndImage
            };

            datMatKhauAction.CustomizePopupWindowParams += DatMatKhauAction_CustomizePopupWindowParams;
            datMatKhauAction.Execute += DatMatKhauAction_Execute;
        }

        private void DatMatKhauAction_CustomizePopupWindowParams(
            object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(DatMatKhauNguoiDungForm));

            DatMatKhauNguoiDungForm form = objectSpace.CreateObject<DatMatKhauNguoiDungForm>();

            DetailView detailView = Application.CreateDetailView(objectSpace, form);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

            e.View = detailView;
        }

        private void DatMatKhauAction_Execute(
            object sender,
            PopupWindowShowActionExecuteEventArgs e)
        {
            ApplicationUser user = ViewCurrentObject;

            if (user == null)
                throw new UserFriendlyException("Không tìm thấy tài khoản cần đặt mật khẩu.");

            DatMatKhauNguoiDungForm form = e.PopupWindowViewCurrentObject as DatMatKhauNguoiDungForm;

            if (form == null)
                throw new UserFriendlyException("Không lấy được thông tin mật khẩu.");

            if (string.IsNullOrWhiteSpace(form.MatKhauMoi))
                throw new UserFriendlyException("Vui lòng nhập mật khẩu mới.");

            if (form.MatKhauMoi != form.NhapLaiMatKhau)
                throw new UserFriendlyException("Mật khẩu nhập lại không khớp.");

            user.IsActive = true;
            user.SetPassword(form.MatKhauMoi);

            // Tắt bắt buộc đổi mật khẩu để bệnh nhân không bị kẹt.
            user.ChangePasswordOnFirstLogon = false;

            ObjectSpace.SetModified(user);
            ObjectSpace.CommitChanges();

            Application.ShowViewStrategy.ShowMessage(
                $"Đã đặt mật khẩu cho tài khoản {user.UserName}.",
                InformationType.Success,
                3000,
                InformationPosition.Top
            );
        }
    }
}