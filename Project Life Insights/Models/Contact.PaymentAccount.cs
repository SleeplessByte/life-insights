using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using System.Net.Mail;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.Models
{
    public partial class Contact : Model
    {
        /// <summary>
        /// 
        /// </summary>
        public class PaymentAccount : Model
        {
            /// <summary>
            /// DisplayName
            /// </summary>
            public String DisplayName { get; protected set; }

            /// <summary>
            /// Banknumber of this account
            /// </summary>
            [BsonIgnoreIfNull]
            public String BankNumber { get; protected set; }

            /// <summary>
            /// Gironumber of this account
            /// </summary>
            [BsonIgnoreIfNull]
            public String GiroNumber { get; protected set; }

            /// <summary>
            /// Email adress of this account
            /// </summary>
            [BsonIgnoreIfNull]
            public MailAddress Email { get; protected set; }

            /// <summary>
            /// Data available
            /// </summary>
            [BsonRequired]
            public Contents Type { get; protected set; }

            /// <summary>
            /// Creates a new account from a bank number
            /// </summary>
            /// <param name="contents"></param>
            /// <returns></returns>
            public static PaymentAccount FromBank(String contents)
            {
                PaymentAccount result = new PaymentAccount();
                result.AddBank(contents);

                return result;
            }

            /// <summary>
            /// Creates a new account from a giro bank number
            /// </summary>
            /// <param name="contents"></param>
            /// <returns></returns>
            public static PaymentAccount FromGiro(String contents)
            {
                PaymentAccount result = new PaymentAccount();
                result.AddGiro(contents);

                return result;
            }

            /// <summary>
            /// Creates a new account from email
            /// </summary>
            /// <param name="contents"></param>
            /// <returns></returns>
            public static PaymentAccount FromEmail(String contents)
            {
                PaymentAccount result = new PaymentAccount();
                result.AddEmail(contents);

                return result;
            }

            /// <summary>
            /// Creates a new account from email
            /// </summary>
            /// <param name="contents"></param>
            /// <returns></returns>
            public static PaymentAccount FromEmail(MailAddress contents)
            {
                PaymentAccount result = new PaymentAccount();
                result.AddEmail(contents);

                return result;
            }

            /// <summary>
            /// Merges another account with this account
            /// </summary>
            /// <param name="other"></param>
            public void Merge(PaymentAccount other)
            {
                if (!String.IsNullOrEmpty(other.BankNumber))
                    AddBank(other.BankNumber);
                if (!String.IsNullOrEmpty(other.GiroNumber))
                    AddGiro(other.GiroNumber);
                if (other.Email == null)
                    AddEmail(other.Email);
            }

            /// <summary>
            /// Adds a bank number
            /// </summary>
            /// <param name="content"></param>
            public void AddBank(String content)
            {
                this.Type |= Contents.Bank;
                this.BankNumber = content;

                NotifyObservers();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="content"></param>
            public void RemoveBank(String content)
            {
                this.Type &= ~Contents.Bank;
                this.BankNumber = null;

                NotifyObservers();
            }

            /// <summary>
            /// Adds a giro number
            /// </summary>
            /// <param name="content"></param>
            public void AddGiro(String content)
            {
                this.Type |= Contents.Giro;
                this.GiroNumber = content;

                NotifyObservers();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="content"></param>
            public void RemoveGiro(String content)
            {
                this.Type &= ~Contents.Giro;
                this.GiroNumber = null;

                NotifyObservers();
            }

            /// <summary>
            /// Adds an email address
            /// </summary>
            /// <param name="content"></param>
            public void AddEmail(String content)
            {
                AddEmail(new MailAddress(content));
            }

            /// <summary>
            /// Adds an email address
            /// </summary>
            /// <param name="content"></param>
            public void AddEmail(MailAddress content)
            {
                this.Type |= Contents.Email;
                this.Email = content;

                NotifyObservers();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="content"></param>
            public void RemoveEmail(MailAddress content)
            {
                this.Type &= ~Contents.Email;
                this.Email = null;

                NotifyObservers();
            }

            /// <summary>
            /// Parses a bank/giro number
            /// </summary>
            /// <param name="from"></param>
            /// <returns></returns>
            internal static PaymentAccount Parse(String from)
            {
                if (from.StartsWith("P") || from.Length < 9)
                    return FromGiro(from);
                return FromBank(from);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override MongoCollection GetCollection()
            {
                return MongoService.Instance.Database.GetCollection<Contact.PaymentAccount>("Contact.PaymentAccount");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return ((this.BankNumber ?? this.GiroNumber ?? String.Empty).GetHashCode() * 127) ^
                    (this.Email == null ? 0 : this.Email.GetHashCode() * 63);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override Boolean Equals(object obj)
            {
                var other = obj as PaymentAccount;

                if (other != null)
                    return false;

                return (other.Email == this.Email && (
                    other.BankNumber == this.BankNumber ||
                    other.GiroNumber == this.GiroNumber));
            }

            /// <summary>
            /// 
            /// </summary>
            [Flags]
            public enum Contents : byte
            {
                None = 0,
                Email = (1 << 0),
                Bank = (1 << 1),
                Giro = (1 << 2),

            }
        }
    }
}
