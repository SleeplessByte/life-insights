using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using Itenso.TimePeriod;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Globalization;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.Models
{
    /// <summary>
    /// Holds the transaction model
    /// </summary>
    public class Transaction : Model
    {
        /// <summary>
        /// Amount
        /// </summary>
        [BsonRequired]
        public Decimal Amount { get; protected set; }

        /// <summary>
        /// Currency
        /// </summary>
        [BsonRequired]
        public String Currency { get; protected set; }

        /// <summary>
        /// Bank transaction id
        /// </summary>
        public String TransactionId { get; set; }

        /// <summary>
        /// Name/Description of the transaction
        /// </summary>
        [BsonRequired]
        public String Description { get; protected set; }

        /// <summary>
        /// Optional comment values
        /// </summary>
        public String Note { get; protected set; }

        /// <summary>
        /// Starting point of the transaction
        /// </summary>
        [BsonRequired]
        public Contact.PaymentAccount From { get; protected set; }
        
        /// <summary>
        /// Ending point of the transaction
        /// </summary>
        [BsonRequired]
        public Contact.PaymentAccount To { get; protected set; }

        /// <summary>
        /// Date of the transaction
        /// </summary>
        [BsonIgnore]
        public Date Date { 
            get { return _date.Data; } 
            protected set { _date = new SerializableDate(value); } 
        }
        
        [BsonElement("Date")]
        private SerializableDate _date;

        /// <summary>
        /// Time of the transaction
        /// </summary>
        [BsonIgnore]
        public Time Time
        {
            get { return _time.Data; }
            protected set { _time = new SerializableTime(value); }
        }

        [BsonElement("Time")]
        private SerializableTime _time;

        /// <summary>
        /// Timezone of the transaction's origin
        /// </summary>
        [BsonIgnore]
        public TimeZoneInfo TimeZone
        {
            get { return _timeZoneInfo.Data; }
            protected set { _timeZoneInfo = new SerializableTimeZoneInfo(value); }
        }

        [BsonElement("TimeZone")]
        private SerializableTimeZoneInfo _timeZoneInfo;

        /// <summary>
        /// Mutation type
        /// </summary>
        [BsonRequired]
        public Transaction.Code Type { get; protected set; }

        /// <summary>
        /// Is a withdrawal
        /// </summary>
        [BsonIgnore]
        public Boolean IsWithdrawal { get { return this.Amount < 0; }}

        /// <summary>
        /// Is a deposit
        /// </summary>
        [BsonIgnore]
        public Boolean IsDeposit { get { return this.Amount > 0; }}

        /// <summary>
        /// Constructor for ING/Rabobank transactions
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="description"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="code"></param>
        /// <param name="amount"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static Transaction Generate(DateTime datetime, String description, String from, String to, Transaction.Code code,
            Decimal amount, String notes = null, String currency = "EUR")
        {
            return new Transaction(new Date(datetime), new Time(datetime), TimeZoneInfo.Local, description,
                Contact.PaymentAccount.Parse(from), Contact.PaymentAccount.Parse(to), code, amount, currency, String.Empty, notes);
        }

        /// <summary>
        /// Constructor for Paypal transactions
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="description"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="code"></param>
        /// <param name="amount"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static Transaction Generate(Date date, Time time, TimeZoneInfo zone, String description, Transaction.Code code, 
            String currency, Decimal amount, String from, String to, String transactionId, String notes = null)
        {
            return new Transaction(date, time, zone, description, Contact.PaymentAccount.FromEmail(from), Contact.PaymentAccount.FromEmail(to),
                code, amount, currency, transactionId, notes);
        }

        /// <summary>
        /// Constructor for any transaction
        /// </summary>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="code"></param>
        /// <param name="amount"></param>
        /// <param name="notes"></param>
        public Transaction(Date date, Time time, TimeZoneInfo zone, String description, Contact.PaymentAccount from, Contact.PaymentAccount to, 
            Transaction.Code code, Decimal amount, String currency, String transactionId, String notes) : base()
        {
            this.Date = date;
            this.Time = time;
            this.TimeZone = zone;
            this.Description = description;
            this.From = from;
            this.To = to;
            this.Type = code;
            this.Amount = amount;
            this.Currency = currency;
            this.TransactionId = transactionId ?? String.Empty;
            this.Note = notes ?? String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Transaction Parse(ParseInfo info, params String[] data)
        {
            var splitted = info.HasFlag(ParseInfo.CVS) ? data[0].Split(',') : data;
            switch (info & ~ParseInfo.CVS)
            {
                case ParseInfo.ING:

                    DateTime ingDate = DateTime.Now;
                        if (!DateTime.TryParse(splitted[0], out ingDate))
                            ingDate = DateTime.ParseExact(splitted[0], "yyyyMMdd", CultureInfo.CurrentCulture);

                    var ingTime = DateTime.Now;
                    var ingTimestamp = splitted[1].Contains(' ') ? splitted[1].Split(' ') : null;

                    var ingDesc = TryExtractingDateAndTime(ingTimestamp, ref ingDate, ref ingTime);
                    splitted[1] = ingDesc ?? splitted[1];

                    if (ingDesc == null)
                        if (!DateTime.TryParse(splitted[0], out ingDate))
                            ingDate = DateTime.ParseExact(splitted[0], "yyyyMMdd", CultureInfo.CurrentCulture);

                    return Transaction.Generate(ingDate, splitted[1].Trim(), splitted[2], splitted[3],
                        (Code)Enum.Parse(typeof(Code), splitted[4]), (splitted[5].Contains("Af") ? -1 : 1) * Decimal.Parse(splitted[6], NumberStyles.Currency), splitted[7]);

                case ParseInfo.Rabobank:

                    DateTime raboDate = DateTime.Now;
                        if (!DateTime.TryParse(splitted[2], out raboDate))
                            raboDate = DateTime.ParseExact(splitted[2], "yyyyMMdd", CultureInfo.CurrentCulture);

                    var raboTime = DateTime.Now;
                    var raboTimestamp = splitted[10].Contains(' ') ? splitted[10].Split(' ') : null;

                    var raboDesc = TryExtractingDateAndTime(raboTimestamp, ref raboDate, ref raboTime);
                    splitted[10] = raboDesc ?? splitted[10];

                    if (raboDesc == null)
                        if (!DateTime.TryParse(splitted[2], out raboDate))
                            raboDate = DateTime.ParseExact(splitted[2], "yyyyMMdd", CultureInfo.CurrentCulture);

                    var numberFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                    numberFormat.CurrencyDecimalSeparator = ".";

                    return Transaction.Generate(raboDate, splitted[6].Trim(), splitted[0], splitted[5],
                        (Code)Enum.Parse(typeof(Code), splitted[8].ToUpper()), (splitted[3].Contains("D") ? 1 : 1) * Decimal.Parse(splitted[4], NumberStyles.Currency, numberFormat), 
                        splitted[10], splitted[1]);
                    
                case ParseInfo.PayPal:
                    return Transaction.Generate(new Date(DateTime.Parse(splitted[0])), new Time(DateTime.Parse(splitted[1])), null, //TimeZoneInfo.FindSystemTimeZoneById("2")
                        splitted[3], Code.Paypal, splitted[5], Decimal.Parse(splitted[6]), splitted[7], splitted[8], splitted[9]); //(Code)Enum.Parse(typeof(Code), splitted[4])
            }

            throw new NotSupportedException("No parsing available for " + info.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static String TryExtractingDateAndTime(String[] timestamp, ref DateTime date, ref DateTime time)
        {
            if (timestamp == null)
                return null;

            if (DateTime.TryParse(timestamp[1], out time))
            {
                // Rabo format
                DateTime dummy;
                if (!DateTime.TryParse(timestamp[0], out dummy))
                {
                    date = date.AddTicks(time.Ticks);
                    return String.Join(" ", timestamp[0], timestamp.Skip(2));
                }
            }

            // ING format
            if (timestamp != null && (DateTime.TryParse(timestamp[0], out date) || 
                DateTime.TryParseExact(timestamp[0], "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out date)))
            {
                date = date.AddTicks(time.Ticks);
                return String.Join(" ", timestamp.Skip(2));
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<Transaction>("Transaction");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.From.GetHashCode() * 255) ^
                (this.To.GetHashCode() * 127) ^
                (this.Date.GetHashCode() * 63) ^
                (this.Time.GetHashCode() * 31) ^
                (this.Currency.GetHashCode() * 15) ^
                (this.Amount.GetHashCode() * 7) ^
                (this.Type.GetHashCode() * 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(object obj)
        {
            var other = obj as Transaction;

            if (other == null)
                return false;

            return other.From == this.From &&
                other.To == this.To &&
                other.Date.Equals(this.Date) &&
                other.Time.Equals(this.Time) &&
                other.Currency == this.Currency &&
                other.Amount == this.Amount &&
                other.Type == this.Type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return String.Format("[Model].Transaction(Id: {0}, Type: {4}, Date: {5}, Time {6}, Description: {1}, Amount: {2} {3})",
                this.Id, this.Description, this.Amount, this.Currency, this.Type, this.Date, this.Time);
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ParseInfo
        {
            None = 0,
            CVS = (1 << 0),
            ING = (1 << 1),
            PayPal = (1 << 2),
            Rabobank = (1 << 3),
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Code
        {
            None = 0,

            /// <summary>
            /// Acceptgiro
            /// </summary>
            AC,

            /// <summary>
            /// Incasso
            /// </summary>
            IC,

            /// <summary>
            /// Betaalautomaat
            /// </summary>
            BA,

            /// <summary>
            /// Overschrijving
            /// </summary>
            OV,

            /// <summary>
            /// Cheque
            /// </summary>
            CH,

            /// <summary>
            /// Opname Kantoor
            /// </summary>
            PK,

            /// <summary>
            /// Diversen
            /// </summary>
            DV,

            /// <summary>
            /// Periodieke overschrijving
            /// </summary>
            PO,

            /// <summary>
            /// Overschrijvingskaart
            /// </summary>
            GB,

            /// <summary>
            /// Rente
            /// </summary>
            R,

            /// <summary>
            /// Telefonisch Bankieren
            /// </summary>
            GF,

            /// <summary>
            /// Reservering
            /// </summary>
            RV,

            /// <summary>
            /// Geld Automaat
            /// </summary>
            GM,

            /// <summary>
            /// Storting
            /// </summary>
            ST,

            /// <summary>
            /// Internet bankieren
            /// </summary>
            GT,

            /// <summary>
            /// Verzamelbetaling
            /// </summary>
            VZ,

            /// <summary>
            /// Paypall electronic check
            /// </summary>
            PEC,

            /// <summary>
            /// Paypall mobile
            /// </summary>
            PM,

            /// <summary>
            /// Paypall web accept
            /// </summary>
            PWA,

            /// <summary>
            /// Paypall general
            /// </summary>
            Paypal,

            /// <summary>
            /// Bankgiro opdracht
            /// </summary>
            BG,

            /// <summary>
            /// Crediteuren betaling
            /// </summary>
            CB,

            /// <summary>
            /// Chipknip
            /// </summary>
            CK,

            /// <summary>
            /// Diverse boekingen
            /// </summary>
            DB,

            /// <summary>
            /// Bedrijven Euro-incasso
            /// </summary>
            EB,

            /// <summary>
            /// Euro-incasso
            /// </summary>
            EI,

            /// <summary>
            /// FiNBOX
            /// </summary>
            FB,

            /// <summary>
            /// Geld automaat Euro
            /// </summary>
            GA,

            /// <summary>
            /// iDEAL
            /// </summary>
            ID,

            /// <summary>
            /// Kashandeling
            /// </summary>
            KH,
            
            /// <summary>
            /// Machtiging
            /// </summary>
            MA,

            /// <summary>
            /// Salaris betaling
            /// </summary>
            SB,

            /// <summary>
            /// Eigen rekening
            /// </summary>
            TB,

            /// <summary>
            /// Telegiro
            /// </summary>
            TG,
        }
    }
}
