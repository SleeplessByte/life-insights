using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectLifeInsights.MVC;

namespace ProjectLifeInsights.Models
{
    public partial class Prototype : Model
    {
        public String Name { get; protected set; }

        public Type DataType { get; protected set; }
        public IModel Data { get; protected set; }
        public Int32 DataCount { get; protected set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Prototype(String name, Type dataType, IModel data, Int32 count = 1)
        {
            this.Name = name;
            this.Data = data;
            this.DataType = dataType;
            this.DataCount = count;
        }

        /// <summary>
        /// 
        /// </summary>
        public Prototype() { }

        /// <summary>
        /// 
        /// </summary>
        public new void Save()
        {
            throw new NotSupportedException();
        }
    }
}
