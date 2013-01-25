using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLifeInsights.Models;
using ProjectLifeInsights.Models.Collection;
using MongoDB.Driver;
using ProjectLifeInsights.Services;

namespace ProjectLifeInsights.MVC
{
    [BsonDiscriminator(RootClass = true),
    BsonKnownTypes(
        typeof(Activity), typeof(Activity.Category),
        typeof(Contact), typeof(Contact.PaymentAccount),
        typeof(Goods),
        typeof(Phone), typeof(Phone.Call), typeof(Phone.Message), typeof(Phone.Number),
        typeof(Transaction),

        typeof(PhoneCallList), typeof(TransactionList))]
    public abstract class Model : IModel
    {
        private HashSet<IView> _observers;

        /// <summary>
        /// 
        /// </summary>
        [BsonId]
        public ObjectId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime
        {
            get { return this.Id.CreationTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Model()
        {
            _observers = new HashSet<IView>();
            this.Id = ObjectId.GenerateNewId();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public virtual Boolean AddObserver(IView view)
        {
            return _observers.Add(view);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public virtual Boolean RemoveObserver(IView view)
        {
            return _observers.Remove(view);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void NotifyObservers()
        {
            foreach (var observer in _observers)
                observer.Update(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Save()
        {
            GetCollection().Save(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual MongoCollection GetCollection()
        {
            return MongoService.Instance.Database.GetCollection<Model>("Model");
        }
    }
}
