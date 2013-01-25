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
    public class KeyLog : Model
    {

        [BsonRequired]
        public String Keys { get; protected set; }

        [BsonRequired]
        public String Alphanum { get; protected set; }

        [BsonIgnore]
        public String Mouse { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public KeyLog()
            : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static String FilterAlphaNumeric(String buffer)
        {
            var letters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSstTUuVvWwXxYyZz0123456789 ";
            var selection = buffer.Split(' ').Select(c => c.ToLower() == "space" ? " " : c).Where(c => letters.Contains(c));
            return String.Join("", selection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static KeyLog Flush(String buffer, Boolean autoPush = true)
        {
            var log = new KeyLog() { Keys = buffer, Alphanum = FilterAlphaNumeric(buffer) };

            if (autoPush)
                log.Save();

            return log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<KeyLog>("Key.Log");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            var other = obj as KeyLog;
            if (obj == null)
                return false;

            return other.Keys == this.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Keys.GetHashCode() * 127);
        }
    }
}
