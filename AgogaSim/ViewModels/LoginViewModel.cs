using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AgogaSim
{
    public class LoginViewModel : BaseViewModel
    {
        string company, user, password;

        private ICredentialsService credentialsService;
        private RestService restService;
		private AlertService alertService;

        public delegate void SuccessHandler();
        public event SuccessHandler OnSuccess;

        public LoginViewModel(ICredentialsService credentialsService, 
                              RestService restService, AlertService alertService)
        {
            this.alertService = alertService;
            this.credentialsService = credentialsService;
            this.restService = restService;
            this.ShowExplainMessage = false;
        }

		public void Init(bool didAppCrash)
		{
			var credentials = credentialsService.LoadCredentials();
			if (credentials != null)
			{
                Company = credentials.Company;
				User = credentials.UserID;
				Password = credentials.Password;
                if (credentials.AutomaticLogin && !didAppCrash)
                {
                    LoginCommand.Execute(null);
                }
            } else {
                ShowExplainMessage = true;
                Notify("ShowExplainMessage");
            }
		}

        void saveCredentials()
        {
            var credentials = new Credentials { Company = Company, UserID = User, Password = Password, AutomaticLogin = true };
            credentialsService.SaveCredentials(credentials);
        }

		bool isLoginEnabled()
		{
            return !string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Company);
		}

        public bool ShowExplainMessage { get; set; }

        public string Company
        {
            get { return company; }
            set
            {
                company = value;
                Notify("Company");
            }
        }

		public string User
		{
			get { return user; }
			set
			{
				user = value;
				Notify("User");
			}
		}

		public string Password
		{
            get { return password; }
			set
			{
                password = value;
				Notify("Password");
			}
		}

		ICommand loginCommand;
        public ICommand LoginCommand
		{
			get
			{
				return loginCommand ?? (loginCommand = new Command(async () => await ExecuteLoginCommand()));
			}
		}

		protected async Task ExecuteLoginCommand()
		{
			if (IsProcessing)
				return;

			if (!isLoginEnabled())
			{
				await alertService.ShowAsync("Por favor digite todos os campos para efetuar o login.");
				return;
			}


			IsProcessing = true;
			saveCredentials();
            bool logged = await restService.Login(Company, User, Password);
            if (logged)
            {
                if (ShowExplainMessage)
                    await alertService.ShowAsync("Duas dicas para usar o app:\n\n" +
                                                 "1. Deslize a tela para direita para ver o ponto de dias anteriores.\n" +
                                                 "2. Bastar puxar (deslizar para baixo) em qualquer tela para atualizar as informações.", 
                                                 "Bem Vinda(o)!!");
                
                OnSuccess?.Invoke();
            }
            else
            {
                await alertService.ShowAsync("Problema ao efetuar o login.");
            }

			IsProcessing = false;
		}
    }
}
