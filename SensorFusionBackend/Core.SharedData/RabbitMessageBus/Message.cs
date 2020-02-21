using System;

namespace Core.Common.RabbitMessageBus
{
    public class Message
    {
        public string MessageText { get; set; }
        public Guid MessageId { get; } = Guid.NewGuid();
        public bool NotNew { get; set; }
    }

    public enum RequestCommand
    {
        GetAllGpsData,
        GetAllSensorData,
        AddNewGpsPoint,
        AddSensorData
    }
    

    public class RequestMessage
    {
        public RequestCommand RequestCommand { get; set; }
        public object RequestPayload { get; set; }
       
        [Newtonsoft.Json.JsonConstructor]
        public RequestMessage(RequestCommand commandType, object payload)
        {
            RequestCommand = commandType;
            RequestPayload = payload;
        }
    }

    public class ResponseMessage
    {
        public object ResponsePayload { get; set; }

        [Newtonsoft.Json.JsonConstructor]
        public ResponseMessage(object payload)
        {
            ResponsePayload = payload;
        }
    }
}
