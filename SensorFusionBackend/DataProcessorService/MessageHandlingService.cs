using System.Linq;
using Core.Common.Interfaces;
using Core.Common.RabbitMessageBus;
using Core.Common.SharedDataObjects;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace DataProcessorService
{
    public class MessageHandlingService
    {
        public readonly IMessageBus _bus;
        public readonly IDataRepository<SensorData> _sensorDataRepo;
        public readonly IDataRepository<GpsPoint> _gpsDataRepo;
        public readonly IDistributedCache _cache;

        public MessageHandlingService(IDistributedCache cache, IMessageBus bus,
            IDataRepository<SensorData> sensorDataRep, IDataRepository<GpsPoint> gpsDataRepo)
        {
            _cache = cache;
            _bus = bus;
            _sensorDataRepo = sensorDataRep;
            _gpsDataRepo = gpsDataRepo;

            BindMessageHandlers();
        }

        private void BindMessageHandlers()
        {
            _bus.Subscribe("SimpleMessagesPipe", m =>
            {
                Log.Warning("!!!got simple message!!! test:" + m.MessageText);
            });

            _bus.Response(request =>
            {
                Log.Warning("!!!got request message!!!");
                if (request.RequestCommand == RequestCommand.GetAllGpsData)
                {
                    var results = _gpsDataRepo.GetAllAsync().Result;
                    return new ResponseMessage(results.ToList());
                }
                if (request.RequestCommand == RequestCommand.GetAllSensorData)
                {
                    var results = _sensorDataRepo.GetAllAsync().Result;
                    return new ResponseMessage(results.ToList());
                }
                return new ResponseMessage("server sent no bits");
            });
           
            _bus.SubscribeAsync("DataProcessPipe", async m =>
            {
                Log.Warning("!!!got!!!");
                if (m.RequestCommand == RequestCommand.AddNewGpsPoint)
                {
                    var gpsData = (GpsPoint) m.RequestPayload;
                    await _gpsDataRepo.AddAsync(new GpsPoint
                    {
                        lat = gpsData.lat,
                        long_ = gpsData.long_,
                        time = gpsData.time
                    });
                }

                if (m.RequestCommand == RequestCommand.AddSensorData)
                {
                    var sensorData = (SensorData)m.RequestPayload;
                    await _sensorDataRepo.AddAsync(new SensorData
                    {
                        TimeStamp = sensorData.TimeStamp,
                        Role = sensorData.Role,
                        Data = sensorData.Data
                    });
                }
            });
        }
    }
}
