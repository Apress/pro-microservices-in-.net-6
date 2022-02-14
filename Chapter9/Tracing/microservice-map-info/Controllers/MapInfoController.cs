using System;
using System.Threading.Tasks;
using microservice_map_info.Models;
using Microsoft.AspNetCore.Mvc;

namespace microservice_map_info.Controllers
{

	[Route("[controller]")]
	[Route("[controller]/[action]")]
	[ApiController]
	public class MapInfoController : ControllerBase
	{
		private readonly DistanceInfoService _distanceInfoService;

		public MapInfoController(DistanceInfoService distanceInfoService)
		{
			_distanceInfoService = distanceInfoService;
		}

		[HttpGet]
		public async Task<GoogleDistanceData> GetDistance(string originCity, string destinationCity)
		{
			return await _distanceInfoService.GetMapDistanceAsync(originCity, destinationCity);
		}
	}
}
