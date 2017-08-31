using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Azure.Mobile.Analytics;
using Xamarin.Forms;

namespace AgogaSim
{
    public class DayReportViewModel : BaseViewModel
    {
        public DayReport DayReport { get; set; }
        public bool IsExpanded { get; set; }
    }

    public class AgogaSimViewModel : BaseViewModel
    {
        private RestService rest;
        private ICredentialsService credentialsService;

        public AgogaSimViewModel(ICredentialsService credentialsService, RestService rest)
        {
            this.credentialsService = credentialsService;
            this.rest = rest;
            Days = new ObservableCollection<DayReportViewModel>();
            IsStartingLoading = true;
            LoadCommand.Execute(null);
        }

		bool isStartingLoading;
		public bool IsStartingLoading
		{
			get { return isStartingLoading; }
			set
			{
				isStartingLoading = value;
				Notify("IsStartingLoading");
			}
		}

        private Report report;
        public Report Report
        {
            get { return report; }
            set
            {
                report = value;
                Detail1 = (report.Resume.Count >= 1) ? report.Resume[0] : null;
                Detail2 = (report.Resume.Count >= 2) ? report.Resume[1] : null;
                Detail3 = (report.Resume.Count >= 3) ? report.Resume[2] : null;
                Detail4 = (report.Resume.Count >= 4) ? report.Resume[3] : null;
                Detail5 = (report.Resume.Count >= 5) ? report.Resume[4] : null;
                defineLeavingTime();

                Days.Clear();
                foreach (var day in report.Days)
                    Days.Add(new DayReportViewModel { DayReport = day, IsExpanded = false });

                Notify(new List<string> { "Report", "LeavingTime", "LeavingTimeObs", 
                    "Detail1", "Detail2", "Detail3", "Detail4", "Detail5" });
            }
        }

        public ObservableCollection<DayReportViewModel> Days { get; set; }

        public Detail Detail1 { get; set; }
        public Detail Detail2 { get; set; }
        public Detail Detail3 { get; set; }
        public Detail Detail4 { get; set; }
        public Detail Detail5 { get; set; }

		public string LeavingTime { get; set; }
		public string LeavingTimeObs { get; set; }

        void defineLeavingTime()
        {
            if (report == null)
                return;

            var today = report.Today;
            var expectedTime = today.GetExpectedLeavingTime();
            if (expectedTime == TimeSpan.Zero)
                LeavingTime = "--:--";
            else
                LeavingTime = String.Format("{0:hh\\:mm}", expectedTime);

            if (expectedTime != TimeSpan.Zero && today.IntervalDid == TimeSpan.Zero)
                LeavingTimeObs = String.Format("* utiliza intervalo de {0:hh\\:mm} (conforme cadastro)", today.DayHiredInterval);
            else
                LeavingTimeObs = null;
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
            if (IsProcessing || IsRefreshing)
                return;

            Analytics.TrackEvent("Load Command");
            IsProcessing = true;
            setDetailsToNull();
			await loadDataFromServer();
			IsProcessing = false;
            IsRefreshing = false;
            IsStartingLoading= false;
        }

        private void setDetailsToNull()
        {
			Detail1 = Detail2 = Detail3 = Detail4 = Detail5 = null;
            Notify(new List<string> { "Detail1", "Detail2", "Detail3", "Detail4", "Detail5" });
        }

		ICommand refreshCommand;
        public ICommand RefreshCommand
		{
			get
			{
				return refreshCommand ?? (refreshCommand = new Command(async () => await ExecuteRefreshCommand()));
			}
		}

        protected async Task ExecuteRefreshCommand()
        {
            if (IsProcessing)
            {
				IsRefreshing = true;
                return;
            }

            if (IsRefreshing)
                return;

            Analytics.TrackEvent("Refresh Command");
            IsRefreshing = true;
            await loadDataFromServer();
            IsRefreshing = false;
        }

        private async Task loadDataFromServer()
        {
            var credentials = credentialsService.LoadCredentials();
            if (credentials == null)
                return;
            
            var reportData = await rest.LoadData(credentials.Company, credentials.UserID, 
                                                 credentials.Password, DateTime.Today);
            if (reportData != null && reportData.ShouldReadNextMonth())
            {
                var nextReport = await rest.LoadData(credentials.Company, credentials.UserID, 
                                                     credentials.Password, DateTime.Today.AddMonths(1));
                if (nextReport != null)
                {
                    nextReport.AddDays(reportData.Days);
                    reportData = nextReport;
                }
            }
            this.Report = reportData;
        }
    }
}
