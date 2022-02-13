using System;
using System.Net.Http;
using System.Threading.Tasks;
using microservice_map_info.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace microservice_map_info.Controllers
{

	[Route("[controller]")]
	[Route("[controller]/[action]")]
	[ApiController]
	public class MapInfoController : ControllerBase
	{
		private readonly DistanceInfoService _distanceInfoService;
		private readonly ILogger<MapInfoController> _logger;

		public MapInfoController(DistanceInfoService distanceInfoService, ILogger<MapInfoController> logger)
		{
			_distanceInfoService = distanceInfoService;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		public async Task<ActionResult<GoogleDistanceData>> GetDistance(string originCity, string destinationCity)
		{
			try
			{
				return await _distanceInfoService.GetMapDistanceAsync(originCity, destinationCity);
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, $"Error getting map distance: ${originCity} to ${destinationCity}, status code: {ex.StatusCode}");
				return StatusCode(500);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting map distance: ${originCity} to ${destinationCity}");
				return StatusCode(500);
			}
		}
	}
}
