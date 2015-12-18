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

		private TimeSpan? Duration
		{
			get { return AimedArrivalTime - ProbableDepartureTime; }
		}

		public override string ToString ()
		{
			var template = @"Platform {0} to {1}
{2:HH:mm} - {3:HH:mm} ({4} mins)";
				
			return string.Format (template, Platform, Destination, ProbableDepartureTime, AimedArrivalTime, Duration == null ? null : (double?)Duration.Value.TotalMinutes);
		}
	}


}

