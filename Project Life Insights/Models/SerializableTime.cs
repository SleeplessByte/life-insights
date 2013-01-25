using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Itenso.TimePeriod;

namespace ProjectLifeInsights.Models
{
    public class SerializableTime
    {
        [BsonRequired]
        public TimeSpan _data;

        /// <summary>
        /// 
        /// </summary>
        public Time Data { get { return new Time(new DateTime(_data.Ticks)); } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public SerializableTime(Time from)
        {
            _data = from.Duration;
        }
        
    }
}
