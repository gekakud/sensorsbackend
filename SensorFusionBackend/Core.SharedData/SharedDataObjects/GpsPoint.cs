using System;
using Core.Common.Interfaces;

namespace Core.Common.SharedDataObjects
{
    public class GpsPoint : IIdentifiable
    {
        public Guid Id { get; set; }

        public float lat;
        public float long_;
        public DateTime time;
    }
}