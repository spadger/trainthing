using Android.App;
using Android.Content;
using Android.Appwidget;
using System;
using Android.Widget;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TrainThing
{
	[BroadcastReceiver(Label = "Train Thing")]
	[MetaData("android.appwidget.provider", Resource = "@xml/widget")]
	[IntentFilter (new string [] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	public class TrainWidget : AppWidgetProvider
	{
		public override void OnUpdate (Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{

			var intent = new Intent(context, GetType());
			intent.SetAction("refresh_now");
			var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);

			var view = new RemoteViews (context.PackageName, Resource.Layout.widget);
			view.SetOnClickPendingIntent(Resource.Id.refresh_now, pendingIntent);

			Refresh(context, view);
		}

		public override void OnReceive (Context context, Intent intent)
		{
			base.OnReceive (context, intent);
			if (intent.Action == "refresh_now") {
				Refresh (context, new RemoteViews (context.PackageName, Resource.Layout.widget));
			}
		}

		private void Refresh(Context context, RemoteViews view)
		{
            var resultsTask = new TimetableGetter().GetValidServicesFor(TimetableGetter.GetDepartureStation(), TimetableGetter.GetDestinationStation());

            view.SetTextViewText(Resource.Id.next_train_time, "");
            view.SetTextViewText(Resource.Id.platform, "");
            view.SetTextViewText (Resource.Id.last_updated, DateTime.Now.ToString ("HH:mm:ss"));

            resultsTask.ContinueWith (x=>PopulateViewWithResults(x, context, view), TaskContinuationOptions.OnlyOnRanToCompletion);
		}

        private void PopulateViewWithResults(Task<IEnumerable<Train>> trainsTask, Context context, RemoteViews view)
        {
            AppWidgetManager manager = AppWidgetManager.GetInstance (context);
            ComponentName thisWidget = new ComponentName (context, Java.Lang.Class.FromType (typeof (TrainWidget)).Name);

            var train = trainsTask.Result.FirstOrDefault();

            if(train==null)
            {
                return;
            }

            view.SetTextViewText(Resource.Id.next_train_time, train.ProbableDepartureTime.ToString("hh:mm"));
            view.SetTextViewText(Resource.Id.platform, train.Platform);

            manager.UpdateAppWidget (thisWidget, view);
        }
    }
}