using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using microservice_map_info.Protos;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace microservice_map_tester
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
			var configuration = new ConfigurationBuilder()
			  .SetBasePath(Directory.GetCurrentDirectory())
			  .AddJsonFile("appsettings.json")
			  .AddJsonFile($"appsettings.{envName}.json", optional: true)
			  .AddEnvironmentVariables()
			  .AddUserSecrets(typeof(Program).Assembly, optional: true)
			  .AddCommandLine(args)
			  .Build();

			string jaegerHost = configuration.GetValue<string>("openTelemetry:jaegerHost");

			using var tracerProvider = Sdk.CreateTracerProviderBuilder()
				.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(typeof(Program).Assembly.GetName().Name))
				.AddHttpClientInstrumentation()
				.AddJaegerExporter(options =>
				{
					options.AgentHost = jaegerHost;
				})
				.AddConsoleExporter()
				.Build();

			HttpClient httpClient = new HttpClient();
			string mapInfoUrl = configuration.GetValue<string>("mapInfoUrl");
			httpClient.BaseAddress = new Uri(mapInfoUrl);

			while (true)
			{

				Thread.Sleep(5000);

				try
				{
					string originCity = "Topeka,KS";
					string destinationCity = "Los Angeles,CA";

					var res = await httpClient.GetAsync($"/MapInfo/GetDistance?originCity={originCity}&destinationCity={destinationCity}");
					string data = await res.Content.ReadAsStringAsync();

					Console.WriteLine($"Response: {data}");

				}
				catch (Exception ex)
				{
					Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
				}
			}

		}
	}
}
