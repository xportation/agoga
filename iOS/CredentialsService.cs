using System.Linq;
using AgogaSim.iOS;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(CredentialsService))]
namespace AgogaSim.iOS
{
	public class CredentialsService : ICredentialsService
	{
		
        public void SaveCredentials(Credentials credentials)
		{
            Account account = new Account { Username = App.AppName };
			account.Properties.Add("Company", credentials.Company);
			account.Properties.Add("UserID", credentials.UserID);
			account.Properties.Add("Password", credentials.Password);
			account.Properties.Add("AutomaticLogin", System.Convert.ToString(credentials.AutomaticLogin));
			AccountStore.Create().Save(account, App.AppName);

		}

		public bool UpdatePassword(string password)
		{
            var account = AccountStore.Create().FindAccountsForService(App.AppName).SingleOrDefault(a => a.Username == App.AppName);
			if (account != null)
			{
				account.Properties["Password"] = password;
				return true;
			}

			return false;
		}

        public Credentials LoadCredentials()
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).SingleOrDefault(a => a.Username == App.AppName);
            if (account != null)
            {
                var credentials = new Credentials();
            	credentials.Company = account.Properties["Company"];
            	credentials.UserID = account.Properties["UserID"];
                credentials.Password = account.Properties["Password"];
            	credentials.AutomaticLogin = System.Convert.ToBoolean(account.Properties["AutomaticLogin"]);
                return credentials;
            }
            return null;
        }

		public void DeleteCredentials()
		{
			var account = AccountStore.Create().FindAccountsForService(App.AppName).SingleOrDefault(a => a.Username == App.AppName);
			if (account != null)
			{
				AccountStore.Create().Delete(account, App.AppName);
			}
		}

	}
}