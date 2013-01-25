using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLifeInsights.MVC
{
    public interface IView
    {
        void Update(IModel model);
    }
}
