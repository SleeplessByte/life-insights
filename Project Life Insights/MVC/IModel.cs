using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLifeInsights.Models;
using ProjectLifeInsights.Models.Collection;

namespace ProjectLifeInsights.MVC
{
    public interface IModel
    {
        Boolean AddObserver(IView view);
        Boolean RemoveObserver(IView view);
        void NotifyObservers();

        void Save();
    }
}
