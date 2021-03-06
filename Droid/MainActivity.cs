﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Refractored.XamForms.PullToRefresh.Droid;

namespace AgogaSim.Droid
{
    [Activity(Label = "Agoga", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation,
              ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            PullToRefreshLayoutRenderer.Init();

            LoadApplication(new App());
        }
    }
}
