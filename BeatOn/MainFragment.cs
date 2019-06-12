using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V17.Leanback.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BeatOn
{
    public class MainFragment : BrowseFragment
    {
        private static string TAG = nameof(MainFragment);


        private void LoadUI()
        {
            Title = "Beat ON!";
            HeadersState = HeadersEnabled;
            HeadersTransitionOnBackEnabled = true;
            // set fastLane (or headers) background color

            BrandColor = Resource.Color.colorPrimaryDark;
            SearchAffordanceColor = Resource.Color.colorPrimary;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            LoadUI();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}