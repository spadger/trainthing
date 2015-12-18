using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace TrainThing
{
	[Activity (Label = "TrainThing", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		ArrayAdapter<string> resultsAdapter;
		IEnumerable<Train> results;
		Button button;
		Spinner spnDirection;

		protected async override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Main);

			button = FindViewById<Button> (Resource.Id.myButton);

			spnDirection = FindViewById<Spinner>(Resource.Id.spnDirection);
			var directionAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
			directionAdapter.AddAll (new List<string>{ "Benfleet -> London", "London -> Benfleet" });
			spnDirection.Adapter = directionAdapter;

			var now = DateTime.Now.Hour;
			if (now < 3 || now > 13) {
				spnDirection.SetSelection(1);
			}

			resultsAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
			var resultsView = FindViewById<ListView> (Resource.Id.lstResults);
			resultsView.Adapter = resultsAdapter;

			button.Click += async delegate {
				await Refresh();
			};

			await Refresh();
		}

		private async Task Refresh()
		{
			try
			{
				button.Enabled = false;
				spnDirection.Enabled = false;
				button.Text = "Wait...";

				resultsAdapter.Clear();

				var fst = "fst";
				var bef = "bef";

				using(var client = new HttpClient(new NativeMessageHandler()))
				{
					var railClient = new RailClient(client);
					var timetableGetter = new TimetableGetter(railClient);

					var fromBEF = spnDirection.SelectedItemPosition==0;
					results = await timetableGetter.GetValidServicesFor(fromBEF?bef:fst, fromBEF?fst:bef);

					foreach(var result in results)
					{
						resultsAdapter.Add(result.ToString());
					}
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