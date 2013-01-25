using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using Itenso.TimePeriod;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLifeInsights.Services;
using MongoDB.Driver;

namespace ProjectLifeInsights.Models
{
    public partial class Phone : Goods
    {
        public class Call : Model
        {
            [BsonElement("Action")]
            protected Action _result;

            /// <summary>
            /// 
            /// </summary>
            [BsonIgnore]
            public String DisplayName { get { return this.Number.DisplayName;  } }

            /// <summary>
            /// 
            /// </summary>
            [BsonRequired]
            public Phone.Number Number { get; protected set; }

            /// <summary>
            /// 
            /// </summary>
            [BsonRequired]
            public ITimePeriod Period { get; protected set; }

            
            /// <summary>
            /// 
            /// </summary>
            public Boolean IsIncoming {
                get { return _result == Action.In; }
            }

            /// <summary>
            /// 
            /// </summary>
            public Boolean IsOutgoing {
                get { return _result == Action.Out; }
            }

            /// <summary>
            /// 
            /// </summary>
            public Boolean IsMissed {
                get { return _result == Action.Missed; }
            }

            /// <summary>
            /// 
            /// </summary>
            public Boolean IsVoicemail {
                get { return _result == Action.Voicemail; }
            }

            /// <summary>
            /// Generates a call from number, duration, calltime and action
            /// </summary>
            /// <param name="number"></param>
            /// <param name="duration"></param>
            /// <param name="callTime"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public static Call Generate(Phone.Number number, DateTime callTime, Int32 duration, Action action)
            {
                return new Call(number, new TimeRange(callTime, TimeSpan.FromSeconds(duration), true), action);
            }

            /// <summary>
            /// Creates a new call
            /// </summary>
            /// <param name="number"></param>
            /// <param name="period"></param>
            /// <param name="action"></param>
            public Call(Phone.Number number, ITimePeriod period, Action action)
            {
                this.Number = number;
                this.Period = period;

                _result = action;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="number"></param>
            /// <param name="duration"></param>
            /// <param name="date"></param>
            /// <param name="type"></param>
            /// <param name="display"></param>
            /// <returns></returns>
            internal static Call Parse(String number, String date, String duration, String type, String display)
            {
                Phone.Number parsedNumber;
                if (Phone.Number.TryParse(number, out parsedNumber)) {
                    parsedNumber.SetDisplayName(display);
                    return Call.Generate(parsedNumber, DateTime.Parse(date), Int32.Parse(duration), (Call.Action)Enum.Parse(typeof(Call.Action), type));
                }

                throw new NotSupportedException("That format phone number is not supported");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override MongoCollection GetCollection()
            {
                return MongoService.Instance.Database.GetCollection<Phone.Call>("Phone.Call");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override String ToString()
            {
                return String.Format("[Model].Phone.Call(Id: {0}, Action: {1}, Period: {2}, From: {3})",
                    this.Id, _result.ToString(), this.Period, this.Number);
            }

            public enum Action : byte
            {
                None = 0,
                In = 1,
                Out = 2,
                Missed = 3,
                Voicemail = 4,
            }
        }
    }
}
