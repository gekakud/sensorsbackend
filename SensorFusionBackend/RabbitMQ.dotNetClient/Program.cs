using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using MongoDB.Bson;
using MongoDB.Driver;
using Core.Common.RabbitMessageBus;
using Core.Common.SharedDataObjects;
using EasyNetQ.FluentConfiguration;

namespace RabbitMQ.dotNetClient
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine(" Press [enter] to start.");
            Console.ReadLine();
            var bus = RabbitHutch.CreateBus("host=localhost;timeout=10");

            bus.Advanced.Container.Resolve<IConventions>().QueueNamingConvention = (type, id) => type.Name + "_Queue_" + id;
            bus.Advanced.Container.Resolve<IConventions>().ExchangeNamingConvention = type => type.Name + "_Exchange";

            bus.Subscribe<Message>(string.Empty, m => { Console.WriteLine("Id:" + m.MessageId + " message:" + m.MessageText); });

            int i = 0;
            while (true)
            {
                i++;
                Thread.Sleep(500);
                

                bus.Publish(new Message{MessageText = "message number " + i}, "SimpleMessagesPipe");
                bus.Publish(new RequestMessage(RequestCommand.AddNewGpsPoint
                    , new GpsPoint {lat = 33, long_ = 12, time = DateTime.Now }), "DataProcessPipe");

                PublishSensorData(bus);

                var req = new RequestMessage(RequestCommand.GetAllGpsData, "GetAllGpsData request");

                //send request and proceed
                Task.Run(() =>
                {
                    var resp = bus.RequestAsync<RequestMessage, ResponseMessage>(req).Result;
                    Console.WriteLine("server responsed with " + ((List<GpsPoint>)resp.ResponsePayload).Count + " items");
                });

                Console.WriteLine("OK");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void PublishSensorData(IBus bus)
        {
            bus.Publish(new RequestMessage(RequestCommand.AddSensorData
                , new SensorData
                {
                    Data = "some sensor data here",
                    Role = Role.HumiditySensor,
                    TimeStamp = DateTime.Now
                }), "DataProcessPipe");
        }
    }

    class DbTest
    {
        public DbTest()
        {
            var client = new MongoClient("mongodb://localhost:27017");


            IMongoDatabase database = client.GetDatabase("foo");
            var collection = database.GetCollection<Person>("bar");

            collection.InsertOneAsync(new Person { Name = "Jack" });

            var list = collection.Find(x => x.Name == "Jack")
                .ToListAsync();

            foreach (var person in list.Result)
            {
                Console.WriteLine(person.Name);
            }
        }
    }

    public class Person
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
