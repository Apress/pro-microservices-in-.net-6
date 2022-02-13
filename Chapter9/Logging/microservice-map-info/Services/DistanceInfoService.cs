using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using microservice_map_info.Models;
using microservice_map_info.Protos;
using Microsoft.Extensions.Configuration;

namespace microservice_map_info
{
	public class DistanceInfoService : DistanceInfo.DistanceInfoBase
	{
		private readonly string _googleDistanceApiKey;
		private readonly IHttpClientFactory _clientFactory;

		public DistanceInfoService(IHttpClientFactory clientFactory,
			IConfiguration configuration)
		{
			_clientFactory = clientFactory;
			_googleDistanceApiKey = configuration["googleDistanceApi:apiKey"];
		}

		public async Task<GoogleDistanceData> GetMapDistanceAsync(string originCity, string destinationCity)
		{
			if (string.IsNullOrWhiteSpace(originCity) || 
			    string.IsNullOrWhiteSpace(destinationCity) ||
			    string.IsNullOrWhiteSpace(_googleDistanceApiKey))
			{
				return new GoogleDistanceData();
			}

			var googleUrl = $"?units=imperial&origins={originCity}" +
			                $"&destinations={destinationCity}" +
			                $"&key={_googleDistanceApiKey}";

			using var client = _clientFactory.CreateClient("googleApi");

			var request = new HttpRequestMessage(HttpMethod.Get, googleUrl);

			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();

			await using var data = await response.Content.ReadAsStreamAsync();
			var distanceInfo = await JsonSerializer.DeserializeAsync<GoogleDistanceData>(data);

			return distanceInfo;
		}

		public override async Task<DistanceData> GetDistance(Cities cities, ServerCallContext context)
		{
			var totalMiles = "0";

			var distanceData = await GetMapDistanceAsync(cities.OriginCity, cities.DestinationCity);
			foreach (var distanceDataRow in distanceData.rows)
			{
				foreach (var element in distanceDataRow.elements)
				{
					totalMiles = element.distance.text;
				}
			}
			return new DistanceData { Miles = totalMiles };
		}
	}
}
