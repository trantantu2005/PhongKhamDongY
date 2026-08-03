using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace ybaso.Module.BusinessObjects
{
    [NavigationItem(false)]
    [XafDisplayName("Ngũ tả")]
    public class Nguta : XPObject
    {
        public Nguta(Session session) : base(session) { }

        private Cakham ca;
        [Association("Cakham-Nguta")]
        [XafDisplayName("Ca khám")]
        public Cakham Ca
        {
            get => ca;
            set => SetPropertyValue(nameof(Ca), ref ca, value);
        }

        private int soTT;
        [XafDisplayName("Số thứ tự")]
        public int SoTT
        {
            get => soTT;
            set => SetPropertyValue(nameof(SoTT), ref soTT, value);
        }

        private DanhMucHuyet huyet;
        [XafDisplayName("Huyệt")]
        public DanhMucHuyet Huyet
        {
            get => huyet;
            set => SetPropertyValue(nameof(Huyet), ref huyet, value);
        }
    }
}