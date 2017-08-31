using System;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace AgogaSim
{
    public partial class App : Application
    {
        static App app;

        RestService restService;
        AlertService alertService;
		ICredentialsService credentialsService;

        public static string AppName { get { return "AGOGA"; } }

        public App()
        {
            InitializeComponent();
            credentialsService = DependencyService.Get<ICredentialsService>();
            restService = new RestService();
            alertService = new AlertService(this);

            app = this;
            MainPage = new LoginPage(credentialsService, restService, alertService);
        }

		public static void GoToMainPage()
		{
            var mainPage = new AgogaSimPage(App.GetCredentialsService(), App.GetRestService());

			app.MainPage = new NavigationPage(mainPage)
			{
				BarBackgroundColor = Color.White
			};

			//if (Device.RuntimePlatform == Device.iOS)
                //NavigationPage.SetTitleIcon(mainPage, new FileImageSource { File = "logo.png" });
		}

		public static void Logout()
		{
            var credentialsService = App.GetCredentialsService();
            var credentials = credentialsService.LoadCredentials();
            credentials.AutomaticLogin = false;
            credentialsService.SaveCredentials(credentials);
            app.MainPage = new LoginPage(App.GetCredentialsService(), App.GetRestService(), App.GetAlertService());
		}

		public static RestService GetRestService()
		{
			return app.restService;
		}

        public static ICredentialsService GetCredentialsService()
		{
            return app.credentialsService;
		}

		public static AlertService GetAlertService()
		{
			return app.alertService;
		}

        protected override void OnStart()
        {
			MobileCenter.Start("ios=15ff53da-3e73-4672-aef0-bbd8755678b1;" +
				   "android=2432f771-0fed-406c-90da-1c4102974d36;",
				   typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
