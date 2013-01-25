using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace ProjectLifeInsights.Models
{
    public static class XmlStreamAxisHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> SimpleStreamAxis(XmlReader reader, String[] elementNames)
        {
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (elementNames.Contains(reader.Name))
                    {
                        XElement el = XNode.ReadFrom(reader) as XElement;
                        if (el != null)
                        {
                            yield return el;
                        }
                    }
                }
            }
        }

    }
}
