using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.DanhMucHeThong)]
    [ImageName("BO_Localization")]
    [XafDisplayName("Địa phương")]
    public class Diaphuong : XPObject
    {
        public Diaphuong(Session session) : base(session) { }

        private string tenDP;
        [Size(255)]
        [XafDisplayName("Tên địa phương")]
        public string TenDP
        {
            get => tenDP;
            set => SetPropertyValue(nameof(TenDP), ref tenDP, value);
        }

        private string capDiaPhuong;
        [Size(50)]
        [XafDisplayName("Cấp địa phương")]
        public string CapDiaPhuong
        {
            get => capDiaPhuong;
            set => SetPropertyValue(nameof(CapDiaPhuong), ref capDiaPhuong, value);
        }

        private Diaphuong parent;
        [Association("Diaphuong-Children")]
        [XafDisplayName("Địa phương cha")]
        public Diaphuong Parent
        {
            get => parent;
            set => SetPropertyValue(nameof(Parent), ref parent, value);
        }

        [Association("Diaphuong-Children")]
        [XafDisplayName("Địa phương con")]
        public XPCollection<Diaphuong> Children
        {
            get => GetCollection<Diaphuong>(nameof(Children));
        }

        public override string ToString()
        {
            return TenDP;
        }
    }
}