using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace ybaso.Module.BusinessObjects
{
    public class ApplicationUserLoginInfo : XPObject, ISecurityUserLoginInfo
    {
        public ApplicationUserLoginInfo(Session session) : base(session) { }

        private string loginProviderName;

        [Size(255)]
        [Appearance(
            "PasswordProvider",
            Enabled = false,
            Criteria = "!(IsNewObject(this)) and LoginProviderName == '" + SecurityDefaults.PasswordAuthentication + "'",
            Context = "DetailView"
        )]
        public string LoginProviderName
        {
            get => loginProviderName;
            set => SetPropertyValue(nameof(LoginProviderName), ref loginProviderName, value);
        }

        private string providerUserKey;

        [Size(255)]
        [Appearance(
            "PasswordProviderUserKey",
            Enabled = false,
            Criteria = "!(IsNewObject(this)) and LoginProviderName == '" + SecurityDefaults.PasswordAuthentication + "'",
            Context = "DetailView"
        )]
        public string ProviderUserKey
        {
            get => providerUserKey;
            set => SetPropertyValue(nameof(ProviderUserKey), ref providerUserKey, value);
        }

        private ApplicationUser user;

        [Association("ApplicationUser-UserLogins")]
        [RuleRequiredField(DefaultContexts.Save)]
        public ApplicationUser User
        {
            get => user;
            set => SetPropertyValue(nameof(User), ref user, value);
        }

        object ISecurityUserLoginInfo.User => User;
    }
}