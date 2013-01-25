using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using Itenso.TimePeriod;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.Models
{
    public partial class Activity : Model
    {
        /// <summary>
        /// 
        /// </summary>
        public String Description { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ITimePeriod Period { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<Activity>("Activity");
        }

        /// <summary>
        /// 
        /// </summary>
        public class Category : Model
        {
            /// <summary>
            /// 
            /// </summary>
            public String Description { get; protected set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override MongoCollection GetCollection()
            {
                return MongoService.Instance.Database.GetCollection<Activity.Category>("Activity.Category");
            }
        }
    }

}
