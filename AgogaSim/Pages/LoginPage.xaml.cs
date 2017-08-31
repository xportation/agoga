using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Xamarin.Forms;

namespace AgogaSim
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(ICredentialsService credentialsService, 
                         RestService restService, AlertService alertService)
		{
			InitializeComponent();

			var vm = new LoginViewModel(App.GetCredentialsService(), 
                                        App.GetRestService(), App.GetAlertService());
			vm.OnSuccess += LoginPageOnSuccess;

			BindingContext = vm;
            Analytics.TrackEvent("Login");
        }

		void LoginPageOnSuccess()
		{
			App.GoToMainPage();
		}

    }
}
