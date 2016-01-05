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
			AppWidgetManager manager = AppWidgetManager.GetInstance (context);
			ComponentName thisWidget = new ComponentName (context, Java.Lang.Class.FromType (typeof (TrainWidget)).Name);

            var resultsTask = new TimetableGetter().GetValidServicesFor(TimetableGetter.GetDepartureStation(), TimetableGetter.GetDestinationStation());

			view.SetTextViewText(Resource.Id.departs_1, "");
			view.SetTextViewText(Resource.Id.platform_1, "");
			view.SetTextViewText(Resource.Id.arrives_1, "");
			view.SetTextViewText(Resource.Id.departs_2, "");
			view.SetTextViewText(Resource.Id.platform_2, "");
			view.SetTextViewText(Resource.Id.arrives_2, "");

			view.SetTextViewText (Resource.Id.last_updated, "updating now...");

			manager.UpdateAppWidget (thisWidget, view);
            
            resultsTask.ContinueWith (x=>PopulateViewWithResults(x, context, manager, view, thisWidget), TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		private void PopulateViewWithResults(Task<IEnumerable<Train>> trainsTask, Context context, AppWidgetManager manager, RemoteViews view, ComponentName thisWidget)
        {
			view.SetTextViewText (Resource.Id.last_updated, DateTime.Now.ToString ("HH:mm:ss"));

			var trains = trainsTask.Result;
			var train = trains.FirstOrDefault();

			if(train!=null)
			{
				view.SetTextViewText(Resource.Id.departs_1, Format(train.ProbableDepartureTime));
				view.SetTextViewText(Resource.Id.platform_1, train.Platform);
				view.SetTextViewText(Resource.Id.arrives_1, Format(train.AimedArrivalTime));
			}

			train = trains.Skip(1).FirstOrDefault();

			if(train!=null)
			{
				view.SetTextViewText(Resource.Id.departs_2, Format(train.ProbableDepartureTime));
				view.SetTextViewText(Resource.Id.platform_2, train.Platform);
				view.SetTextViewText(Resource.Id.arrives_2, Format(train.AimedArrivalTime));
			}

            manager.UpdateAppWidget (thisWidget, view);
        }

		private string Format(DateTime? time)
		{
			return time == null ? "unkonwn" : time.Value.ToString ("HH:mm");
		}
    }
}