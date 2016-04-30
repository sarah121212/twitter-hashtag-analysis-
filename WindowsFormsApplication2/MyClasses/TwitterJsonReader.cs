using AzizaMapReducer.MyClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsApplication2.MyClasses
{
    /// <summary>
    /// this class converts any twitter json string to json object
    /// </summary>
  public  class TwitterJsonReader
    {
        public IEnumerable<string> MaxUser { get; set; }
        public Dictionary<string, double> uCountry { get; set; }
        public IEnumerable<KeyValuePair<string, double>> uSources { get; set; }
        public List<KeyValuePair<long, int>> LDates { get; set; }
        public Dictionary<string, int> Others { get; set; }
        Dictionary<string, int> Dates;
        public TwitterJsonReader(string fileName)
        {
            string file = DataSetPreperator.StripHTML(fileName);

            List<string> Finish = JsonConvert.DeserializeObject<List<string>>(file);

            MaxUser = JsonConvert.DeserializeObject<IEnumerable<string>>(Finish[0]);
            uCountry = JsonConvert.DeserializeObject<Dictionary<string, double>>(Finish[1]);
            uSources = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, double>>>(Finish[2]);
            LDates = JsonConvert.DeserializeObject<List<KeyValuePair<long, int>>>(Finish[3]);
            int RetweetSum = JsonConvert.DeserializeObject<int>(Finish[4]);
            int FavouriteSum = JsonConvert.DeserializeObject<int>(Finish[5]);
            int tweetsCount = JsonConvert.DeserializeObject<int>(Finish[6]);
            Dates = new Dictionary<string, int>();
            foreach (var item in LDates.Take(30))
            {
                DateTime d = new DateTime(item.Key);
                Dates.Add(d.ToString(), item.Value);
            }
            Others = new Dictionary<string, int>();
         


            int sum = RetweetSum + FavouriteSum + tweetsCount;

            double a1 = Math.Round((double)RetweetSum / (double)sum / 1.5, 2) * 100;
            double a3 = Math.Round((double)tweetsCount * 2 / (double)sum, 2) * 100;
            double a2 = 100 - (a1 + a3);

            Others.Add("Retweets:" + (a1).ToString() + "%", (int)a1);
            Others.Add("Favourites:" + (a2).ToString() + "%", (int)a2);
            Others.Add("tweets:" + (a3).ToString() + "%", (int)a3);



        }
        /// <summary>
        /// countries after processing
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> CountriesReader()
        {
            return uCountry;

        }
        /// <summary>
        /// Sources  after processing
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, double>> SourceReader()
        {
            return uSources;

        }

        /// <summary>
        /// Timeline  after processing
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> TimelineReader()
        {


            return Dates;

        }

        /// <summary>
        /// Tweets, Retweets and Favourites statistics
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> OtherReader()
        {

           
            return Others;

        }

        /// <summary>
        /// top users in the file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> UsersReader()
        {
             return MaxUser;
        }
    }

    
}
