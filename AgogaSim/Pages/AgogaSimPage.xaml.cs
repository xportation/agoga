using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AgogaSim
{

	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void Notify(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Notify(IList<string> propertyNames)
		{
			if (PropertyChanged != null)
				foreach (var propertyName in propertyNames)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}


		bool isProcessing;
		public bool IsProcessing
		{
			get { return isProcessing; }
			set
			{
				isProcessing = value;
				IsEnabled = !isProcessing;

				Notify("IsProcessing");
			}
		}

		bool isEnabled = true;
		public bool IsEnabled
		{
			get { return isEnabled; }
			set
			{
				isEnabled = value;
				Notify("IsEnabled");
			}
		}
	}

    public class HttpService
    {
        Uri baseUri;
        HttpClient client;

        public HttpService()
        {
            baseUri = new Uri("https://www.ahgora.com.br/externo/");

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Clear();
        }

		public async Task<bool> Login()
		{
			var uri = new Uri(baseUri, "login");

			try
			{
				var formContent = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("empresa", "a718864"),
					new KeyValuePair<string, string>("matricula", "10000241"),
					new KeyValuePair<string, string>("senha", "l10000241s")
				});

				var response = await client.PostAsync(uri, formContent);
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(@"ERROR {0}", ex.Message);
			}

			return false;
		}
    }

    public class AgogaSimViewModel : BaseViewModel
    {
        private RestService rest;
  
        public AgogaSimViewModel(RestService rest)
        {
            this.rest = rest;
            LoadCommand.Execute(null);
        }

		private Report report;
        public Report Report 
        { 
            get { return report; } 
            set 
            {
                report = value;
                Notify("Report");
            }
        }
		
        ICommand loadCommand;
        public ICommand LoadCommand
		{
			get
			{
				return loadCommand ?? (loadCommand = new Command(async () => await ExecuteLoadCommand()));
			}
		}

		protected async Task ExecuteLoadCommand()
		{
			if (IsProcessing)
				return;

			IsProcessing = true;
            this.Report = await rest.Login();
			IsProcessing = false;
		}
    }

	public partial class AgogaSimPage : ContentPage
    {
        public AgogaSimPage()
        {
            InitializeComponent();
            BindingContext = new AgogaSimViewModel(new RestService());
        }

    }
}
