using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AgogaSim
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int StartDay { get; set; }
        public bool StartMonthBefore { get; set; }
    }

    public class Detail
    {
		public string Value { get; set; }
		public string Description { get; set; }
    }

    public class Punch
    {
        public TimeSpan Time { get; set; }
        public string Device { get; set; }
    }

    public class DayReport
    {
        public DateTime Day { get; set; }
        public IList<Punch> Punches { get; set; }
        public IList<Detail> Resume { get; set; }
        public string Justification { get; set; }
		public TimeSpan DailyHours { get; set; }
		public TimeSpan DailyInterval { get; set; }
    }

	public class Person
	{
		public string Id { get; set; }
		public string Name { get; set; }
        public string JobDescription { get; set; }
        public DateTime HireDate { get; set; }
        public IList<Detail> Resume { get; set; }
	}

    public class Report
    {
        public Company Company { get; set; }
        public Person Person { get; set; }
        public IList<Detail> Resume { get; set; }
        public IList<DayReport> Days { get; set; }
    }

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

    public class RestService
    {
		Uri baseUri;
		HttpClient client;

		public RestService()
		{
			baseUri = new Uri("https://www.ahgora.com.br/externo/");

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;
			client.DefaultRequestHeaders.Accept.Clear();
		}

        public async Task<Report> Login()
        {
            var uri = new Uri(baseUri, "getApuracao");

            try
            {
                var data = new Dictionary<string, string> {
                    { "company", "a718864" },
                    { "matricula", "82" },
                    { "senha", "3277" },
                    { "mes", "08" },
                    { "ano", "2017" }
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(data));
                jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                
                var response = await client.PostAsync(uri, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    return content;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
                return ex.Message;
            }

            return "";
        }
    }

    public class AgogaSimViewModel : BaseViewModel
    {
		private HttpService http;
  
        public AgogaSimViewModel(HttpService http)
        {
            this.http = http;
            LoadCommand.Execute(null);
        }

        private string textParsed;
        public string TextParsed
        {
            get { return textParsed; }
            set
            {
                textParsed = value;
                Notify("TextParsed");
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
            TextParsed = await http.Login2();
			IsProcessing = false;
		}
    }

	public partial class AgogaSimPage : ContentPage
    {
        public AgogaSimPage()
        {
            InitializeComponent();

            BindingContext = new AgogaSimViewModel(new HttpService());
        }

    }
}
