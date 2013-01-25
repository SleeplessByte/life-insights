using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLifeInsights.Models
{
    /// <summary>
    /// This converter is mainly used to filter and convert from a delimited input string 
    /// to an array of output strings.
    /// </summary>
    public class DelimiterConverter : IStringConverter
    {
        private Queue<Boolean> _parseList;
        private Char _delimiter;
        private ConversionOptions _options;

        /// <summary>
        /// Creates a new delimiterConverter
        /// </summary>
        /// <param name="delimiter"></param>
        public DelimiterConverter(Char delimiter)
        {
            _parseList = new Queue<Boolean>();

            _delimiter = delimiter;
            _options = ConversionOptions.CleanupQuotes | ConversionOptions.GlueStrings;
        }

        /// <summary>
        /// Creates a new delimiterConverter with a list of mask booleans
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="list">mask that will determine which items to output</param>
        public DelimiterConverter(Char delimiter, Boolean[] list) : this(delimiter)
        {
            foreach (var b in list)
            {
                _parseList.Enqueue(b);
            }
        }

        /// <summary>
        /// Pushes a new mask value
        /// </summary>
        /// <param name="value"></param>
        public void PushMask(Boolean value)
        {
            _parseList.Enqueue(value);
        }

        /// <summary>
        /// Rebuilds the source to output a string with only allowed items
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String Rebuild(String source)
        {
            return String.Join(_delimiter.ToString(), Convert(source));
        }
        
        /// <summary>
        /// Sets the conversionoptions
        /// </summary>
        /// <param name="options"></param>
        public void SetConversionOptions(ConversionOptions options) {
            _options = options;
        }

        /// <summary>
        /// Converts the source using the options
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public String[] Convert(String source, ConversionOptions options)
        {
            ConversionOptions temp = _options;
            SetConversionOptions(options);

            var result = Convert(source);

            SetConversionOptions(temp);
            return result;
        }

        /// <summary>
        /// Converts the input to multiple output
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String[] Convert(String source)
        {
            var i = 0;
            var result = new String[_parseList.Count(r => r == true)];
            var data = new Queue<String>(source.Split(_delimiter));
            var parseList = new Queue<Boolean>(_parseList);

            while (parseList.Count > 0 && data.Count > 0)
            {
                var current = data.Dequeue();

                // String should stay strings
                if (_options.HasFlag(ConversionOptions.GlueStrings))
                {
                    while (current.StartsWith("\"") && !current.EndsWith("\""))
                    {
                        if (data.Count == 0)
                            throw new ArgumentException("Malformed data or wrong ConvertOptions");

                        current += "," + data.Dequeue();
                    }
                }

                if (parseList.Dequeue())
                {
                    result[i++] = current;

                    // Remove quotes
                    if (_options.HasFlag(ConversionOptions.CleanupQuotes))
                        result[i - 1] = result[i - 1].Replace("\"", "");
                }
            }

            if (parseList.Count > 0)
                throw new ArgumentException("Source data is missing a field");

            return result;
        }

        [Flags]
        public enum ConversionOptions
        {
            None = 0,

            CleanupQuotes = (1 << 0),
            GlueStrings = (1 << 1)
        }
    }
}
