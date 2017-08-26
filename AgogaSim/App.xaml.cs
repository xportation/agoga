using Xamarin.Forms;

namespace AgogaSim
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var page = new AgogaSimPage();
            MainPage = new NavigationPage(page)
            {
                BarBackgroundColor = Color.White
            };

			if (Device.RuntimePlatform == Device.iOS)
                NavigationPage.SetTitleIcon(page, new FileImageSource { File = "logo.png" });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
