using System;
namespace AgogaSim
{
    public class Credentials
    {
		public string Company { get; set; }
		public string UserID { get; set; }
		public string Password { get; set; }
		public bool AutomaticLogin { get; set; }
    }

	public interface ICredentialsService
	{
        Credentials LoadCredentials();

        void SaveCredentials(Credentials credentials);

		bool UpdatePassword(string password);

		void DeleteCredentials();
    }
}
