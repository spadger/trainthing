using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using ModernHttpClient;

namespace TrainThing
{
	public interface IRailClient
	{
		Task<List<Train>> GetDeparturesFrom (string from, string to);
	}

	public class RailClient : IRailClient
	{
		Func<HttpClient> getClient;

		public RailClient():this(()=>new HttpClient(new NativeMessageHandler()))
		{}

		public RailClient(Func<HttpClient> getClient)
		{
			this.getClient = getClient;
		}

		public async Task<List<Train>> GetDeparturesFrom(string from, string to)
		{
            const string Template = "http://transportapi.com/v3/uk/train/station/{0}/live.json?calling_at={1}&train_status=passenger&type=departure&station_detail=calling_at&app_id={2}&app_key={3}";
            var stationResourceUri = string.Format(Template, from, to, ApiKeys.AppId, ApiKeys.AppKey);

			var result = new List<Train> ();
			using (var client = getClient ())
			{
				var response = await client.GetAsync (stationResourceUri);
				if (!response.IsSuccessStatusCode) {
					return result;
				}

				var serverResult = JObject.Parse (await response.Content.ReadAsStringAsync ());
				var allDepartures = (JArray)serverResult ["departures"] ["all"];

				foreach (JObject departure in allDepartures) {
					var train = new Train {
						Platform = departure ["platform"].ToObject<string> (),
						AimedDepartureTime = departure ["aimed_departure_time"].ToObject<DateTime?> (),
						ExpectedDepartureTime = departure ["expected_departure_time"].ToObject<DateTime?> (),
						Destination = departure ["destination_name"].ToObject<string> (),
					};

					train.AimedArrivalTime = departure ["station_detail"] ["calling_at"] [0] ["aimed_arrival_time"].ToObject<DateTime?> ();

					result.Add (train);
				}
			}
			return result;
		}
	}
}