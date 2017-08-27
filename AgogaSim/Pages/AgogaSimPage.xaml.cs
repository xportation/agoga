using System;
using System.Diagnostics;
using System.Net.Http;
using Xamarin.Forms;

namespace AgogaSim
{
    public partial class AgogaSimPage : CarouselPage
    {
        AgogaSimViewModel vm;

        public AgogaSimPage()
        {
            InitializeComponent();
			vm = new AgogaSimViewModel(new RestService());
			BindingContext = vm;

            ToolbarItems.Add(new ToolbarItem("", "ic_logout", () => vm.LoadCommand.Execute(null)));
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
