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
        }

        public void Init()
        {
            vm.Init(false);
        }

		void LoginPageOnSuccess()
		{
			App.GoToMainPage();
		}

    }
}
