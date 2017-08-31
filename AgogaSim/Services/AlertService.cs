using System;
using Xamarin.Forms;

namespace AgogaSim
{
    public class AlertService
    {
        readonly Application app;

        public AlertService(Application app)
        {
            this.app = app;
        }

		public async System.Threading.Tasks.Task ShowAsync(string message, string title = "Atenção")
		{
			await app.MainPage.DisplayAlert(title, message, "Ok");
		}
    }
}
