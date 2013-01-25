using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.Models
{
    public partial class Phone : Goods
    {
        /// <summary>
        /// 
        /// </summary>
        public class Number : Model
        {
            /// <summary>
            /// 
            /// </summary>
            [BsonIgnore]
            public static readonly Regex Validator = new Regex("^\\+?(?:[0-9] ?){3,14}[0-9]$", RegexOptions.Compiled); //6, 14

            /// <summary>
            /// 
            /// </summary>
            [BsonRequired]
            public String Value { get; protected set; }

            /// <summary>
            /// 
            /// </summary>
            public String DisplayName { get; protected set; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Number Parse(String number)
            {
                Number result;
                if (TryParse(number, out result))
                    return result;

                throw new ArgumentException("Number not in the right format");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Boolean TryParse(String number, out Number result)
            {
                result = null;

                var match = Validator.Match(number);
                if (!match.Success)
                    return false;

                result = new Number() { Value = match.Value };

                return true;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="display"></param>
            internal void SetDisplayName(string display)
            {
                this.DisplayName = display;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override MongoCollection GetCollection()
            {
                return MongoService.Instance.Database.GetCollection<Phone.Number>("Phone.Number");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Format("[Model].Phone.Number(Id: {0}, Display: {1}, Value: {2})",
                    this.Id, this.DisplayName, this.Value);
            }
        }
    }
}
