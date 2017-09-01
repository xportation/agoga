﻿using System;
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
		LoginPage loginPage = null;
		ICredentialsService credentialsService;

        public static string AppName { get { return "AGOGA"; } }

        public App()
        {
            InitializeComponent();
            credentialsService = DependencyService.Get<ICredentialsService>();
            restService = new RestService();
            alertService = new AlertService(this);

            app = this;
            loginPage = new LoginPage(credentialsService, restService, alertService);
            MainPage = loginPage;
        }

		public static void GoToMainPage()
		{
            var mainPage = new AgogaSimPage(App.GetCredentialsService(), App.GetRestService());

            app.loginPage = null;
			app.MainPage = new NavigationPage(mainPage)
			{
				BarBackgroundColor = Color.White
			};
		}

		public static void Logout()
		{
            var credentialsService = App.GetCredentialsService();
            var credentials = credentialsService.LoadCredentials();
            credentials.AutomaticLogin = false;
            credentialsService.SaveCredentials(credentials);
            var loginPage = new LoginPage(App.GetCredentialsService(), App.GetRestService(), App.GetAlertService());
            loginPage.Init(false);
            app.MainPage = loginPage;
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

        protected override async void OnStart()
        {
            if (loginPage != null)
            {
                bool didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
                loginPage.Init(didAppCrash);
            }

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
