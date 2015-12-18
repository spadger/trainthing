using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrainThing
{

	public class TimetableGetter
	{
		private readonly RailClient client;

		public TimetableGetter (RailClient client)
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