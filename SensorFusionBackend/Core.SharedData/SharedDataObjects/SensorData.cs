using System;
using Core.Common.Interfaces;

namespace Core.Common.SharedDataObjects
{
    public enum Role
    {
        TempSensor, HumiditySensor
    }

    public class SensorData : IIdentifiable
    {
        public Guid Id { get; set; }
        
        public Role Role { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}