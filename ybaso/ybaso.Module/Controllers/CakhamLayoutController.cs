using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class CakhamLayoutController : ObjectViewController<DetailView, Cakham>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ApplyLayoutOptions();
        }

        private void ApplyLayoutOptions()
        {
            if (View?.Model?.Layout == null)
                return;

            foreach (IModelViewLayoutElement element in View.Model.Layout)
            {
                ApplyLayoutOptions(element);
            }
        }

        private void ApplyLayoutOptions(IModelViewLayoutElement element)
        {
            if (element is IModelLayoutGroup group)
            {
                string id = group.Id ?? string.Empty;
                string caption = group.Caption ?? string.Empty;
                string key = (id + " " + caption).ToLowerInvariant();

                bool laGroupConChiaCot =
                    key.Contains("_col") ||
                    key.Contains("col1") ||
                    key.Contains("col2") ||
                    key.Contains(" cột") ||
                    key.Contains("cot");

                bool laGroupThongTinChiTiet =
                    key.Contains("thông tin chi tiết bệnh nhân") ||
                    key.Contains("thong tin chi tiet benh nhan");

                bool laGroupThongTinCaKham =
                    key.Contains("thông tin ca khám") ||
                    key.Contains("thong tin ca kham");

                if (laGroupConChiaCot)
                {
                    SetGroupValue(group, "ShowCaption", false);
                    SetGroupValue(group, "Caption", string.Empty);
                    SetGroupValue(group, "IsCollapsibleGroup", false);
                }
                else if (laGroupThongTinChiTiet || laGroupThongTinCaKham)
                {
                    SetGroupValue(group, "ShowCaption", true);
                    SetGroupValue(group, "IsCollapsibleGroup", true);
                    SetGroupValue(group, "IsGroupCollapsed", true);
                    SetGroupValue(group, "Collapsed", true);
                    SetGroupValue(group, "IsCollapsed", true);
                }

                foreach (IModelViewLayoutElement child in group)
                {
                    ApplyLayoutOptions(child);
                }
            }
        }

        private static void SetGroupValue<T>(IModelLayoutGroup group, string propertyName, T value)
        {
            try
            {
                group.SetValue(propertyName, value);
            }
            catch
            {
                // Một số phiên bản XAF có tên property khác nhau nên bỏ qua.
            }
        }
    }
}