using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using System.IO;

namespace ProjectLifeInsights.Models.Collection
{
    /// <summary>
    /// Collection of Transactions 
    /// </summary>
    public class TransactionList : ModelList<Transaction>
    {
        /// <summary>
        /// Creates an empty collection
        /// </summary>
        public TransactionList() : base()
        {
            
        }

        /// <summary>
        /// Tries to parse a string delimited by new lines
        /// </summary>
        /// <param name="data">input</param>
        /// <param name="list">parsed items</param>
        /// <returns>Parse was (partially) valid</returns>
        public static Boolean TryParse(String data, out TransactionList list)
        {
            list = new TransactionList();
            IStringConverter converter = null;
            var parseInfo = Transaction.ParseInfo.None;

            using (StringReader reader = new StringReader(data))
            {
                // Read the first line
                var line = reader.ReadLine();

                // If it is cvs
                if (line.Contains(','))
                {
                    // ING and Paypal header fields
                    var ing = new String[] { "Datum","Naam / Omschrijving", "Rekening", "Tegenrekening", "Code", "Af Bij", "Bedrag (EUR)", "Mededelingen" };
                    var paypal = new String[] { "Date", "Time", "Time Zone", "Name", "Type", "Currency", "Net", "From Email Address", "To Email Address", "Transaction ID"};

                    converter = new DelimiterConverter(',');
                    parseInfo |= Transaction.ParseInfo.ING;

                    // ING
                    if (!DelimiterParser.TryParse(line, ',', ing, out converter))
                    {
                        parseInfo |= Transaction.ParseInfo.PayPal;
                        parseInfo = parseInfo &~Transaction.ParseInfo.ING;

                        // Paypal
                        if (!DelimiterParser.TryParse(line, ',', paypal, out converter))
                        {
                            // Rabo (Doesn't have header fields)
                            if (line.Split(',').Length >= 15)
                            {
                                parseInfo |= Transaction.ParseInfo.Rabobank;
                                parseInfo = parseInfo & ~Transaction.ParseInfo.PayPal;
                                converter = new DelimiterConverter(',', new Boolean[15].Select(a => true).ToArray());

                                // Try to parse the first line
                                try
                                {
                                    list.Add(Transaction.Parse(parseInfo, converter.Convert(line)));
                                }
                                catch (Exception)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                // If there is no converter, we can't parse the data
                if (converter == null)
                    return false;

                // Read all the items
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        list.Add(Transaction.Parse(parseInfo, converter.Convert(line)));
                    }
                    // Skip faulty entries
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("While parsing Transaction data \"{0}\" was thrown.", ex.Message);
                    }
                }
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
            String innerString = String.Join("\n", (_items ?? new List<Transaction>()).Select(a => a.ToString()));
            return String.Format("[Models].TransactionList({0})", innerString); 
        }
    }
}
