using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Refractored.XamForms.PullToRefresh.iOS;
using UIKit;

namespace AgogaSim.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            PullToRefreshLayoutRenderer.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
