using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace AgogaSim
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel vm;

        public LoginPage(ICredentialsService credentialsService, RestService restService, 
                          AlertService alertService)
		{
			InitializeComponent();

			vm = new LoginViewModel(App.GetCredentialsService(), App.GetRestService(), 
                                    App.GetAlertService());
			vm.OnSuccess += LoginPageOnSuccess;

			BindingContext = vm;
            Analytics.TrackEvent("Login");
        }

        public void Init(bool didAppCrash)
        {
            vm.Init(didAppCrash);
        }

		void LoginPageOnSuccess()
		{
			App.GoToMainPage();
		}

    }
}
