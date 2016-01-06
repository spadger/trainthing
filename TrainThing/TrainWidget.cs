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

			view.SetTextViewText (Resource.Id.last_updated, "updating now...");
			view.SetViewVisibility(Resource.Id.train_1, Android.Views.ViewStates.Gone);
			view.SetViewVisibility(Resource.Id.train_2, Android.Views.ViewStates.Gone);
			view.SetViewVisibility(Resource.Id.train_3, Android.Views.ViewStates.Gone);

			manager.UpdateAppWidget (thisWidget, view);
            
            resultsTask.ContinueWith (x=>PopulateViewWithResults(x, context, manager, view, thisWidget), TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		private void PopulateViewWithResults(Task<IEnumerable<Train>> trainsTask, Context context, AppWidgetManager manager, RemoteViews view, ComponentName thisWidget)
        {
			view.SetTextViewText (Resource.Id.last_updated, DateTime.Now.ToString ("HH:mm:ss"));

			var trains = trainsTask.Result;

			var train = trains.Skip(0).FirstOrDefault();
			SetupView(train, view, Resource.Id.departs_1, Resource.Id.platform_1, Resource.Id.arrives_1, Resource.Id.duration_1, Resource.Id.train_1);

			train = trains.Skip(1).FirstOrDefault();
			SetupView(train, view, Resource.Id.departs_2, Resource.Id.platform_2, Resource.Id.arrives_2, Resource.Id.duration_2, Resource.Id.train_2);

			train = trains.Skip(2).FirstOrDefault();
			SetupView(train, view, Resource.Id.departs_3, Resource.Id.platform_3, Resource.Id.arrives_3, Resource.Id.duration_3, Resource.Id.train_3);

            manager.UpdateAppWidget (thisWidget, view);
        }

		private void SetupView(Train train, RemoteViews view, int departsId, int platformId, int arrivesId, int durationId, int containerId)
		{
			if (train == null) 
			{
				return;
			}

			view.SetTextViewText(departsId, Format(train.ProbableDepartureTime));
			view.SetTextViewText(platformId, train.Platform);
			view.SetTextViewText(arrivesId, Format(train.AimedArrivalTime));
			view.SetTextViewText(durationId, train.FormattedDurationMinutes);

			view.SetViewVisibility (containerId, Android.Views.ViewStates.Visible);
		}

		private string Format(DateTime? time)
		{
			return time == null ? "unkonwn" : time.Value.ToString ("HH:mm");
		}
    }
}