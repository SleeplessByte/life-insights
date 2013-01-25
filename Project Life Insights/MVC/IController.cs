using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLifeInsights.MVC
{
    public interface IController
    {
        void Set(IModel model);
        void Set(IView view);
    }
}
