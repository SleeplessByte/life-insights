using System;
namespace ProjectLifeInsights.Models
{
    public interface IStringConverter
    {
        /// <summary>
        /// Converts the input to multiple outputs
        /// </summary>
        /// <param name="source">string input</param>
        /// <returns></returns>
        String[] Convert(String source);
    }
}
