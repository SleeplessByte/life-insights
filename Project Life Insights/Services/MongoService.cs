using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace ProjectLifeInsights.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MongoService
    {
        private static volatile MongoService _instance;
        private static Object _syncRoot = new Object();

        /// <summary>
        /// 
        /// </summary>
        public MongoServer Server { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MongoDatabase Database { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private MongoService() 
        {
            this.Server = MongoServer.Create();
            this.Database = this.Server.GetDatabase("life", SafeMode.False);
        }

        /// <summary>
        /// 
        /// </summary>
        public static MongoService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        _instance = _instance ?? new MongoService();
                    }
                }

                return _instance;
            }
        }
    }
}
