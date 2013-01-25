using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.Models;
using System.Xml;
using System.IO;

namespace ProjectLifeInsights.Models.Collection
{
    public class PhoneCallList : ModelList<Phone.Call>
    {
        /// <summary>
        /// 
        /// </summary>
        public PhoneCallList() : base()
        {

        }

        internal static Boolean TryParse(String data, out PhoneCallList list)
        {
            list = new PhoneCallList();

            using (StringReader wrapper = new StringReader(data))
            {
                String[] mandatoryFields = new[] { "number", "duration", "readable_date", "type" };
                Dictionary<String, String> callData = new Dictionary<string, string>();
                Boolean isList = true;
                Boolean hasCalls = false;

                using (XmlReader reader = XmlReader.Create(wrapper))
                {
                    try
                    {
                        foreach (var call in XmlStreamAxisHelper.SimpleStreamAxis(reader, new[] { "call", "calls" }))
                        {
                            switch (call.Name.LocalName)
                            {
                                case "call":
                                    var attribute = call.FirstAttribute;
                                    while (attribute != null)
                                    {
                                        callData.Add(attribute.Name.LocalName, attribute.Value);
                                        attribute = attribute.NextAttribute;
                                    }

                                    if (mandatoryFields.All(mfield => callData.Keys.Contains(mfield)))
                                    {
                                        var display = callData.ContainsKey("contact_name") ? callData["contact_name"] : callData["number"];
                                        hasCalls |= true;

                                        try
                                        {
                                            list.Add(Phone.Call.Parse(callData["number"], callData["readable_date"], callData["duration"], callData["type"], display));
                                        }
                                        catch (NotSupportedException e)
                                        {
                                            Console.WriteLine("Phone call was not parsed because {0}", e.Message);
                                        }
                                    }

                                    break;
                                default:
                                    isList = false;
                                    break;
                            }

                            callData.Clear();
                        }
                    }
                    catch (XmlException)
                    {
                        // Probably not xml
                        return false;
                    }
                }

                if (!isList || !hasCalls)
                    return false;
            }
            

            // Parse succes
            return true;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String innerString = String.Join("\n", (_items ?? new List<Phone.Call>()).Select(a => a.ToString()));
            return String.Format("[Models].PhoneCallList({0})", innerString);
        }
    }
}
