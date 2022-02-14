using Grpc.Net.Client;
using microservice_map_info.Protos;

var channel = GrpcChannel.ForAddress(new Uri("https://localhost:5001"));
var client = new DistanceInfo.DistanceInfoClient(channel);
var response = await
client.GetDistanceAsync(new Cities
{ OriginCity = "Topeka,KS", DestinationCity = "Los Angeles,CA" });

Console.WriteLine(response.Miles);

Console.ReadKey();

