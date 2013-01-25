using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectLifeInsights.Models
{
    public class SerializableTimeZoneInfo
    {
        [BsonRequired]
        public String _data;

        /// <summary>
        /// 
        /// </summary>
        public TimeZoneInfo Data { get { return TimeZoneInfo.FromSerializedString(_data); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public SerializableTimeZoneInfo(TimeZoneInfo from)
        {
            _data = from.ToSerializedString();
        }
    }
}
