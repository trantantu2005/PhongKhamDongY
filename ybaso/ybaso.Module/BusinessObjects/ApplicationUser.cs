using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XpoAggregated = DevExpress.Xpo.AggregatedAttribute;

namespace ybaso.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(NavigationGroups.QuanTri)]
    [DefaultProperty(nameof(UserName))]
    [ImageName("BO_User")]
    [XafDisplayName("Application User")]
    public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo
    {
        public ApplicationUser(Session session) : base(session) { }

        [Association("ApplicationUser-UserLogins")]
        [XpoAggregated]
        [Browsable(false)]
        public XPCollection<ApplicationUserLoginInfo> UserLogins
        {
            get => GetCollection<ApplicationUserLoginInfo>(nameof(UserLogins));
        }

        IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins
        {
            get => UserLogins.OfType<ISecurityUserLoginInfo>();
        }

        ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(
            string loginProviderName,
            string providerUserKey)
        {
            ApplicationUserLoginInfo result = new ApplicationUserLoginInfo(Session)
            {
                LoginProviderName = loginProviderName,
                ProviderUserKey = providerUserKey,
                User = this
            };

            return result;
        }
    }
}