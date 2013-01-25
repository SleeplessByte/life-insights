using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.Models
{
    public class ServiceLog : Model
    {
        [BsonRequired]
        public Type Reason { get; protected set; }

        [BsonRequired]
        public String Message { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ServiceLog()
            : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            (new ServiceLog() { Reason = Type.State, Message = "Service starting" }).Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static ServiceLog Information(String message, Boolean autoPush = true)
        {
            var log = new ServiceLog() { Reason = Type.Information, Message = message };

            if (autoPush)
                log.Save();

            return log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        internal static ServiceLog Error(String message, Boolean autoPush = true)
        {
            var log = new ServiceLog() { Reason = Type.Error, Message = message };
            
            if (autoPush)
                log.Save();

            return log;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Stop()
        {
            (new ServiceLog() { Reason = Type.State, Message = "Service stopping" }).Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<ServiceLog>("Service.Log");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            var other = obj as ServiceLog;
            if (obj == null)
                return false;

            return other.Message == this.Message && other.Reason == this.Reason;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Message.GetHashCode() * 127) ^ (this.Reason.GetHashCode() * 255);
        }

        public enum Type : byte
        {
            None = 0,
            State = 1,
            Information = 2,
            Warning = 3,
            Error = 4,
        }
    }
}
