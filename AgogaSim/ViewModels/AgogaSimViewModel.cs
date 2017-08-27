using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public AgogaSimViewModel(RestService rest)
        {
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

                Notify(new List<string> { "Report", "LeavingTime", "Detail1", "Detail2", "Detail3", "Detail4", "Detail5" });
            }
        }

        public ObservableCollection<DayReportViewModel> Days { get; set; }

        public Detail Detail1 { get; set; }
        public Detail Detail2 { get; set; }
        public Detail Detail3 { get; set; }
        public Detail Detail4 { get; set; }
        public Detail Detail5 { get; set; }

		public string LeavingTime { get; set; }

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
            setDetailsToNull();
            var reportData = await rest.LoadData("a718864", "10000241", "l10000241s", DateTime.Today);
            if (reportData != null && reportData.ShouldReadNextMonth())
            {
                var nextReport = await rest.LoadData("a718864", "10000241", "l10000241s", DateTime.Today.AddMonths(1));
                if (nextReport != null)
                {
                    nextReport.AddDays(reportData.Days);
                    reportData = nextReport;
                }
            }
            this.Report = reportData;
            IsProcessing = false;
            IsStartingLoading= false;
        }

        private void setDetailsToNull()
        {
			Detail1 = Detail2 = Detail3 = Detail4 = Detail5 = null;
            Notify(new List<string> { "Detail1", "Detail2", "Detail3", "Detail4", "Detail5" });
        }
    }
}
