using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrainThing
{
	[Activity (Label = "TrainThing", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		ArrayAdapter<string> resultsAdapter;
		IEnumerable<Train> results;
		Button button;

		protected async override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			button = FindViewById<Button> (Resource.Id.myButton);
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
				button.Text = "Wait...";

				resultsAdapter.Clear();

				var source = "fst";
				var dest = "bef";

				using(var client = new HttpClient(new NativeMessageHandler()))
				{
					var railClient = new RailClient(client);
					var timetableGetter = new TimetableGetter(railClient);

					results = await timetableGetter.GetValidServicesFor(source, dest);

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
			}
		}
	}
}