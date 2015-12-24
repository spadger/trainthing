using System;
using Android.App;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using System.Net.Http;
using ModernHttpClient;
using Android.Views;

namespace TrainThing
{
    public class ResultsFragment : Fragment
    {
        ArrayAdapter<string> resultsAdapter;
        IEnumerable<Train> results;
        Button button;
        Spinner spnDirection;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.Results, container, false);
		}

		public override async void OnResume()
		{
			base.OnResume();
		
            button = View.FindViewById<Button> (Resource.Id.myButton);

            spnDirection = View.FindViewById<Spinner>(Resource.Id.spnDirection);
			var directionAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1);
            directionAdapter.AddAll (new List<string>{ "Benfleet -> London", "London -> Benfleet" });
            spnDirection.Adapter = directionAdapter;

            var now = DateTime.Now.Hour;
            if (now < 3 || now > 13) {
                spnDirection.SetSelection(1);
            }

            resultsAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1);
            var resultsView = View.FindViewById<ListView> (Resource.Id.lstResults);
            resultsView.Adapter = resultsAdapter;

            button.Click += async delegate {
                await Refresh();
            };

            await Refresh();
        }

        async Task Refresh()
        {
            try
            {
                button.Enabled = false;
                spnDirection.Enabled = false;
                button.Text = "Wait...";

                resultsAdapter.Clear();

                var fst = "fst";
                var bef = "bef";

                var timetableGetter = new TimetableGetter();

                var fromBEF = spnDirection.SelectedItemPosition==0;
                results = await timetableGetter.GetValidServicesFor(fromBEF?bef:fst, fromBEF?fst:bef);

                foreach(var result in results)
                {
                    resultsAdapter.Add(result.ToString());
                }
            }
            finally
            {
                button.Text = "Refresh";
                button.Enabled = true;
                spnDirection.Enabled = true;
            }
        }

    }
}