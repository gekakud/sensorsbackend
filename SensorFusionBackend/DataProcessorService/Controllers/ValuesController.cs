using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Core.Common.SharedDataObjects;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace DataProcessorService.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private readonly IDataRepository<SensorData> _sensorDataRepo;
        private readonly IDataRepository<GpsPoint> _gpsDataRepo;

        /// <summary>
        /// Controller initialization
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="bus"></param>
        /// <param name="sensorDataRep"></param>
        /// <param name="gpsDataRepo"></param>
        public ValuesController(IDistributedCache cache, IMessageBus bus, 
            IDataRepository<SensorData> sensorDataRep, IDataRepository<GpsPoint> gpsDataRepo)
        {   
//            _bus = bus;
//            var tt = bus.IsMessageBusAlive();
//            cache.SetString("hello", "me");
            _sensorDataRepo = sensorDataRep;
            _gpsDataRepo = gpsDataRepo;
        }

        // GET: api/values/GetRouteByDates
        //GetRouteByDates?FromDate=34&ToDate=44&FilterDistance=30
        [HttpGet]
        [Route("GetRouteByDates")]
        public async Task<IActionResult> GetRouteByDates([FromQuery]DateTime FromDate, [FromQuery]DateTime ToDate,[FromQuery]int FilterDistance = 20)
        {
            try
            {
                if (ToDate == DateTime.MinValue)
                {
                    ToDate = DateTime.Now;
                }
                if (FromDate >= ToDate)
                {
                    FromDate = ToDate.AddMonths(-1);
                }

                var fetchResult = await _gpsDataRepo.FindAsync(g => g.time < ToDate && g.time > FromDate);
                
                var route = new Route { points = fetchResult.ToList() };
                ApplyDistanceFilter(route, FilterDistance);

                if(route.points.Count >= 1)
                {
                    route.StartLat = route.points.OrderBy(p => p.time).First().lat;
                    route.StartLng = route.points.OrderBy(p => p.time).First().long_;

                    route.EndLat = route.points.OrderByDescending(p => p.time).First().lat;
                    route.EndLng = route.points.OrderByDescending(p => p.time).First().long_;
                }

                return StatusCode(StatusCodes.Status200OK, new JsonResult(route).Value);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpGet]
        [Route("GetAllGpsData")]
        public async Task<IActionResult> GetAllGpsData()
        {
            try
            {
                var fetchResult = await _gpsDataRepo.GetAllAsync();

                return new JsonResult(new Route { points = fetchResult.ToList() });
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        //PostGpsValue?lat=34&longitude=44&time=2
        [HttpPost]
        [Route("PostGpsValue")]
        public async Task<IActionResult> PostGpsValue([FromQuery]float lat, [FromQuery]float longitude, [FromQuery]DateTime time)
        {
            try
            {
                await _gpsDataRepo.AddAsync(new GpsPoint
                {
                    lat = lat,
                    long_ = longitude,
                    time = time
                });

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpPost]
        [Route("PostSensorData")]
        public async Task<IActionResult> PostSensorData(SensorData data)
        {
            try
            {
                await _sensorDataRepo.AddAsync(data);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpGet]
        [Route("GetAllSensorData")]
        public async Task<IActionResult> GetAllSensorData()
        {
            try
            {
                var fetchResult = await _sensorDataRepo.GetAllAsync();

                return new JsonResult(fetchResult.ToList());
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpGet]
        [Route("GetSensorDataByDates")]
        public async Task<IActionResult> GetSensorData([FromQuery]DateTime FromDate, [FromQuery]DateTime ToDate, [FromQuery]int SensorType = 0)
        {
            try
            {
                if (ToDate == DateTime.MinValue)
                {
                    ToDate = DateTime.Now;
                }
                if (FromDate >= ToDate)
                {
                    FromDate = ToDate.AddMonths(-1);
                }

                var fetchResult = await _sensorDataRepo.FindAsync(g => g.TimeStamp < ToDate && g.TimeStamp > FromDate && (int)g.Role == SensorType);

                return StatusCode(StatusCodes.Status200OK, new JsonResult(fetchResult).Value);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        private Route ApplyDistanceFilter(Route pMapRoute, double pError)
        {
            if (pError <= 0)
            {
                return pMapRoute;
            }

            int i, j;

            var points = pMapRoute.points.ToArray();

            for (i = 0, j = 1; j < points.Length; i++, j++)
            {
                var a = new GeoCoordinate(points[i].lat, points[i].long_);
                var b = new GeoCoordinate(points[j].lat, points[j].long_);
                if (a.GetDistanceTo(b) > 800 || a.GetDistanceTo(b) < pError)
                {
                    pMapRoute.points.Remove(points[i]);
                }
            }

            for (i = 0, j = 1; j < points.Length; i++, j++)
            {
                var a = new GeoCoordinate(points[i].lat, points[i].long_);
                var b = new GeoCoordinate(points[j].lat, points[j].long_);
                pMapRoute.RouteLength += (float)a.GetDistanceTo(b);
            }

            pMapRoute.RouteLength = pMapRoute.RouteLength / 1000;
            return pMapRoute;
        }
    }
}
