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

            ToolbarItems.Add(new ToolbarItem("", "ic_more", HandleMoreActionAsync));
        }

        async void HandleMoreActionAsync()
        {
            string pageAction = "Resumo do dia";
            if (CurrentPage == Children[0])
                pageAction = "Relatório de dias anteriores";

            var action = await DisplayActionSheet(null, "Cancelar", null, "Logout", "Recarregar", pageAction);
            switch (action) {
                case "Logout":
                    App.Logout();
                    break;
                case "Recarregar":
                    vm.RefreshCommand.Execute(null);
                    if (Device.RuntimePlatform == Device.iOS)
                        ScrollResume.ScrollToAsync(0, -60, true);
                    break;
                case "Resumo do dia":
                    CurrentPage = Children[0];
                    break;
                case "Relatório de dias anteriores":
                    CurrentPage = Children[1];
                    break;
            }
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
