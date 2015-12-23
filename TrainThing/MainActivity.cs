using Android.App;
using Android.OS;

namespace TrainThing
{
	[Activity (Label = "TrainThing", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var resultsFragment = new ResultsFragment ();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Android.Resource.Id.Content, resultsFragment);
            fragmentTransaction.Commit();
        }
	}
}