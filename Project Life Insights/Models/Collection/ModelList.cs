using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using System.Collections;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectLifeInsights.Models.Collection
{
    /// <summary>
    /// Class that represents a collection of models
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelList<T> : Model, IEnumerable<T>, IEnumerable
        where T : IModel
    {
        [BsonElement("Items")]
        protected IList<T> _items;

        /// <summary>
        /// Creates an empty collection
        /// </summary>
        protected ModelList()
        {
            _items = new List<T>();
        }

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item) 
        {
            _items.Add(item);
        }

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        /// <param name="item"></param>
        public virtual void Remove(T item)
        {
            _items.Remove(item);
        }

        /// <summary>
        /// Get the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Privately exposes enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String innerString = String.Join("\n", (_items ?? new List<T>()).Select(a => a.ToString()));
            return String.Format("[Models].ModelList<{1}>({0})", innerString, typeof(T).Name);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            foreach (var item in this)
            {
                item.Save();
            }
        }
    }
}
