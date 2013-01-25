using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using ProjectLifeInsights.Services;
using MongoDB.Driver;

namespace ProjectLifeInsights.Models
{
    public partial class Phone : Goods
    {
        public class Message : Model
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override MongoCollection GetCollection()
            {
                return MongoService.Instance.Database.GetCollection<Phone.Message>("Phone.Message");
            }
        }
    }
}
