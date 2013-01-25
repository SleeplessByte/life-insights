using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itenso.TimePeriod;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectLifeInsights.Models
{
    public class SerializableDate
    {
        [BsonRequired]
        public DateTime _data;

        /// <summary>
        /// 
        /// </summary>
        public Date Data { get { return new Date(_data); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public SerializableDate(Date from)
        {
            _data = from.GetDateTime();
        }
    }
}
