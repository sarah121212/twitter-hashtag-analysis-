using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzizaMapReducer.MyClasses
{
    /// <summary>
    /// General Function
    /// </summary>
  public class DataSetPreperator
    {
        /// <summary>
        /// is the string is valid json
        /// </summary>
        /// <param name="json">json text</param>
        /// <returns></returns>
        public static bool IsValidJSON(String json)
        {
            try
            {
                JToken token = JObject.Parse(json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// removes html tags
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        /// <summary>
        /// removes all lines
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripLines(string input)
        {
            return Regex.Replace(input, @"\t|\n|\r", "");
        }

    }
}
