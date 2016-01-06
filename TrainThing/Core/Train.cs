using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrainThing
{
	public class Train
	{  
		public string Platform { get; set; }
		public DateTime? AimedDepartureTime { get; set; }
		public DateTime? ExpectedDepartureTime { get; set; }
		public string Destination{ get; set; }
		public DateTime? AimedArrivalTime{ get; set; }

		public DateTime ProbableDepartureTime
		{
			get
			{
				return (ExpectedDepartureTime ?? AimedDepartureTime).Value;
			}
		}

		public TimeSpan? Duration
		{
			get 
			{
				var result = AimedArrivalTime - ProbableDepartureTime; 

				if (result != null && result.Value.TotalMinutes < 0) {
					result = result.Value.Add (TimeSpan.FromDays (1));
				}

				return result;
			}
		}

		public string FormattedDurationMinutes
		{
			get
			{
				var duration = Duration;
				return duration.HasValue ? duration.Value.TotalMinutes.ToString() : "unknown";
			}
		}

		public override string ToString ()
		{
			var template = @"Platform {0} to {1}
{2:HH:mm} - {3:HH:mm} ({4} mins)";
				
			return string.Format (template, Platform, Destination, ProbableDepartureTime, AimedArrivalTime, FormattedDurationMinutes);
		}
	}


}

