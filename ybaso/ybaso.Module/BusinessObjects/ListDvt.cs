using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.DanhMucHeThong)]
    [ImageName("BO_List")]
    [XafDisplayName("Đơn vị tính")]
    [DefaultProperty(nameof(TenDvt))]
    public class ListDvt : XPObject
    {
        public ListDvt(Session session) : base(session) { }

        private string tenDvt;

        [Size(100)]
        [XafDisplayName("Tên đơn vị tính")]
        public string TenDvt
        {
            get => tenDvt;
            set => SetPropertyValue(nameof(TenDvt), ref tenDvt, value);
        }

        private bool macdinh;

        [XafDisplayName("Mặc định")]
        public bool Macdinh
        {
            get => macdinh;
            set => SetPropertyValue(nameof(Macdinh), ref macdinh, value);
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(TenDvt) ? base.ToString() : TenDvt;
        }
    }
}