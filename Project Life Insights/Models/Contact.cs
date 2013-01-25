using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using MongoDB.Driver;
using ProjectLifeInsights.Services;
using MongoDB.Bson.Serialization.Attributes;
using System.Net.Mail;
using Thought.vCards;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace ProjectLifeInsights.Models
{
    public partial class Contact : Model
    {
        /// <summary>
        /// Empty instance
        /// </summary>
        protected static readonly Contact Empty = new Contact(String.Empty);

        /// <summary>
        /// Display Name of the Contact
        /// </summary>
        [BsonIgnore]
        public String DisplayName { get; protected set; }

        /// <summary>
        /// vCardData
        /// </summary>
        public vCard Data { get; protected set; }

        /// <summary>
        /// Numbers for this contact
        /// </summary>
        public List<Phone.Number> Numbers { get; protected set; }

        /// <summary>
        /// Mailaddresses for this contact
        /// </summary>
        public List<SerializableMailAddress> MailAddresses { get; protected set; }

        /// <summary>
        /// Linked contacts
        /// </summary>
        public HashSet<MongoDBRef> LinkedContacts { get; protected set; }

        /// <summary>
        /// Creates new contact
        /// </summary>
        /// <param name="name">Display Name</param>
        public Contact(String name)
        {
            this.Numbers = new List<Phone.Number>();
            this.MailAddresses = new List<SerializableMailAddress>();
            this.LinkedContacts = new HashSet<MongoDBRef>();

            this.Data = new vCard();

            this.DisplayName = name;
            this.Data.DisplayName = name;
        }

        /// <summary>
        /// Adds a mail address
        /// </summary>
        /// <param name="address"></param>
        public void AddMailAddress(MailAddress address) 
        {
            this.MailAddresses.Add(new SerializableMailAddress(address));
            this.Data.EmailAddresses.Add(new vCardEmailAddress(address.Address, vCardEmailAddressType.Internet));
            Save();
        }

        /// <summary>
        /// Removes a mail address
        /// </summary>
        /// <param name="address"></param>
        public void RemoveMailAddress(MailAddress address)
        {
            this.MailAddresses.Remove(new SerializableMailAddress(address));
            this.Data.EmailAddresses.Remove(this.Data.EmailAddresses.First(m => m.Address == address.Address));
            Save();
        }

        /// <summary>
        /// Adds a phone number
        /// </summary>
        /// <param name="number"></param>
        public void AddPhoneNumber(Phone.Number number)
        {
            this.Numbers.Add(number);
            this.Data.Phones.Add(new vCardPhone(number.Value)); //, vCardPhoneTypes.))
            Save();
        }

        /// <summary>
        /// Removes a phone number
        /// </summary>
        /// <param name="number"></param>
        public void RemovePhoneNumber(Phone.Number number)
        {
            this.Numbers.Remove(number);
            this.Data.Phones.Remove(this.Data.Phones.First(p => p.FullNumber == number.Value));
            Save();
        }

        /// <summary>
        /// Links a contact
        /// </summary>
        /// <param name="link"></param>
        private void LinkContact(MongoDBRef link)
        {
            if (this.LinkedContacts.All(reference => !MongoDBRef.ReferenceEquals(link, reference)))
            {
                this.LinkedContacts.Add(link);
                Save();
            }
        }

        /// <summary>
        /// Unlinks a contact
        /// </summary>
        /// <param name="link"></param>
        private void UnlinkContact(MongoDBRef link)
        {
            this.LinkedContacts.Remove(link);
            Save();
        }

        /// <summary>
        /// Links a contact
        /// </summary>
        /// <param name="link"></param>
        public void LinkContact(Contact link)
        {
            var refLink = new MongoDBRef(link.GetCollection().Name, link.Id);
            var refThis = new MongoDBRef(this.GetCollection().Name, this.Id);

            // Add link to all this references
            foreach (var reference in this.LinkedContacts)
            {
                if (reference.Id == link.Id)
                    continue;

                Contact.LinkContact(reference.Id, refLink);
                link.LinkContact(reference);
            }

            // Add this to all link references
            foreach (var reference in link.LinkedContacts)
            {
                Contact.LinkContact(reference.Id, refThis);
            }

            //
            // Add link to this
            LinkContact(refLink);

            // Add this to link
            link.LinkContact(refThis);
        }

        /// <summary>
        /// Unlinks a contact
        /// </summary>
        /// <param name="link"></param>
        public void UnlinkContact(Contact link)
        {
            var refLink = new MongoDBRef(link.GetCollection().Name, link.Id);
            var refThis = new MongoDBRef(this.GetCollection().Name, this.Id);

            foreach (var reference in this.LinkedContacts)
                Contact.UnlinkContact(reference.Id, refLink);

            foreach (var reference in link.LinkedContacts)
                Contact.UnlinkContact(reference.Id, refThis);

            //if (this.LinkedContacts.All(reference => !MongoDBRef.ReferenceEquals(refLink, reference))
            UnlinkContact(refLink);
            link.UnlinkContact(refThis);
        }

        /// <summary>
        /// Links a contact to a contact from id
        /// </summary>
        /// <param name="source"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static void LinkContact(BsonValue source, MongoDBRef link)
        {
            Contact.Get(source).ContinueWith(t => t.Result.LinkContact(link));
        }

        /// <summary>
        /// Unlinks a contact from a link
        /// </summary>
        /// <param name="source"></param>
        /// <param name="link"></param>
        public static void UnlinkContact(BsonValue source, MongoDBRef link)
        {
            Contact.Get(source).ContinueWith(t => t.Result.UnlinkContact(link));
        }

        /// <summary>
        /// Gets a contact from id
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Task<Contact> Get(BsonValue source)
        {
            return Task<Contact>.Factory.StartNew(() =>
            {
                return Contact.Empty.GetCollection().FindOneByIdAs<Contact>(source);
            });
        }

        /// <summary>
        /// Gets the collection
        /// </summary>
        /// <returns></returns>
        public override MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<Contact>("Contact");
        }
    }
}
