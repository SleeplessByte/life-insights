using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLifeInsights.Models
{
    /// <summary>
    /// The Delimiter Parser is able to determine how to parse delimited input data, 
    /// if there is a given set of data fields. 
    /// </summary>
    public class DelimiterParser
    {
        /// <summary>
        /// Tries to parse the dataFields using the delimiter and fields. If true,
        /// result will hold an IStringConverter that can convert all valid input
        /// in the form of dataFields, only extracting fields.
        /// </summary>
        /// <param name="dataFields">Input string</param>
        /// <param name="delimiter">Seperator</param>
        /// <param name="fields">Required (conversion) fields</param>
        /// <param name="result">Converter</param>
        /// <param name="options">Parseoptions</param>
        /// <returns>Parse is possible</returns>
        public static Boolean TryParse(String dataFields, Char delimiter, String[] fields, out IStringConverter result, 
            ParseOptions options = ParseOptions.CleanupQuotes) 
        {
            result = new DelimiterConverter(delimiter);

            var splittedDataFields = new Queue<String>(dataFields.Split(delimiter));
            var fieldsQueue = new Queue<String>(fields);

            while (fieldsQueue.Count > 0 && splittedDataFields.Count > 0)
            {
                var field = fieldsQueue.Peek();
                var data = splittedDataFields.Dequeue().Trim();

                if (options.HasFlag(ParseOptions.GlueStrings))
                    while (data.StartsWith("\"") && !data.EndsWith("\"") && splittedDataFields.Count > 0)
                        data = data + "," + splittedDataFields.Dequeue().Trim();

                if (options.HasFlag(ParseOptions.CleanupQuotes))
                    data = data.Replace("\"", "").Trim();

                (result as DelimiterConverter).PushMask(field == data);

                if (field == data)
                    fieldsQueue.Dequeue();
            }

            return fieldsQueue.Count == 0;
        }

        [Flags]
        public enum ParseOptions {
            None = 0,

            CleanupQuotes = (1 << 0),
            GlueStrings = (1 << 1),
        }
    }
}
