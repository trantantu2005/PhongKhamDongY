using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Updating;
using ybaso.Blazor.Server.Templates;

namespace ybaso.Blazor.Server
{
    public class YbasoBlazorApplication : BlazorApplication
    {
        public YbasoBlazorApplication()
        {
            ApplicationName = "ybaso";
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;

            DatabaseVersionMismatch +=
                OnDatabaseVersionMismatch;

            /*
             * Bắt buộc đăng ký custom template.
             * Nếu thiếu đoạn này, XAF luôn dùng LogonWindowTemplate mặc định,
             * nên các class .mf-login-* trong login.css không xuất hiện.
             */
            CreateCustomTemplate +=
                OnCreateCustomTemplate;
        }

        protected override void OnSetupStarted()
        {
            base.OnSetupStarted();

#if DEBUG
            DatabaseUpdateMode =
                DatabaseUpdateMode.UpdateDatabaseAlways;
#endif
        }

        private static void
            OnCreateCustomTemplate(
                object sender,
                CreateCustomTemplateEventArgs e)
        {
            if (e.Context == TemplateContext.LogonWindow)
            {
                e.Template =
                    new MediFlowLogonWindowTemplate();
            }
        }

        private void
            OnDatabaseVersionMismatch(
                object sender,
                DatabaseVersionMismatchEventArgs e)
        {
            e.Updater.Update();
            e.Handled = true;
        }
    }
}