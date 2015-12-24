using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrainThing
{
	public interface ITimetableGetter
	{
		Task<IEnumerable<Train>> GetValidServicesFor (string from, string to);
	}

	public class TimetableGetter : ITimetableGetter
	{
		private readonly IRailClient client;

		public TimetableGetter() : this(new RailClient())
		{}

		public TimetableGetter (IRailClient client)
		{
			this.client = client;
		}

		public async Task<IEnumerable<Train>> GetValidServicesFor(string from, string to)
		{
			var departures = await client.GetDeparturesFrom(from, to);

			return departures.OrderBy (x => x.AimedArrivalTime)
				.Take(8);
		}
	}
}