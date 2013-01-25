using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.Models
{
    public class Goods : Model
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<Goods>("Goods");
        }
    }
}
