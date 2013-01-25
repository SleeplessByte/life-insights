using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectLifeInsights.Models
{
    public class SerializableMailAddress
    {
        [BsonRequired]
        public String _data1;

        [BsonRequired]
        public String _data2;

        public MailAddress Data { get { return new MailAddress(_data1, _data2); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public SerializableMailAddress(MailAddress from)
        {
            _data1 = from.Address;
            _data2 = from.DisplayName;
            
        }
    }
}
