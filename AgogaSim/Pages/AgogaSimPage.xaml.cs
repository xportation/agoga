using System;
using System.Diagnostics;
using System.Net.Http;
using Xamarin.Forms;

namespace AgogaSim
{
    public partial class AgogaSimPage : CarouselPage
    {
        AgogaSimViewModel vm;

        public AgogaSimPage(ICredentialsService credentialsService, RestService restService)
        {
            InitializeComponent();
			vm = new AgogaSimViewModel(credentialsService, restService);
			BindingContext = vm;

            ToolbarItems.Add(new ToolbarItem("", "ic_logout", () => App.Logout()));
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var dayReport = e.Item as DayReportViewModel;
			if (dayReport == null)
				return;

			dayReport.IsExpanded = !dayReport.IsExpanded;
			int itemIndex = vm.Days.IndexOf(dayReport);
			vm.Days[itemIndex] = dayReport;
        }
    }
}
