using Microsoft.Hadoop.MapReduce;
using Microsoft.Hadoop;
using System;
using System.Collections;
using System.Globalization;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Hadoop.WebClient.WebHCatClient;
namespace AzizaMapReducer
{
    class Program
    {
        const string clusterUserName = "admin";
        const string clusterPassword = "MonaMona!2";
        const string hadoopUserName = "hadoop";
        const string azureStorageAccount = "aziza";
        const string azureStorageKey = "jdPiqMg3vtVzQeOKTMI0W/ZzMK5Q4GS1pzfBH5W+Pj7YAgBQe3P/PNTwmG0eL11bPpQKQD84ECkyTPWW10N5rw==";
        const string azureStorageContainer = "aziza";
        const bool createContinerIfNotExist = true;
        static void Main(string[] args)
        {
            var azureCluster = new Uri("https://aziza.azurehdinsight.net");
           // var hadoop = Hadoop.Connect(azureCluster, clusterUserName, hadoopUserName, clusterPassword, azureStorageAccount, azureStorageKey, azureStorageContainer, createContinerIfNotExist);
            var hadoop = Hadoop.Connect();
            Console.WriteLine("Starting: {0} ", DateTime.Now);
           
                var result = hadoop.MapReduceJob.ExecuteJob<TwitExtractionJob>();
                var info = result.Info;
         

            Console.WriteLine("Done: {0} ", DateTime.Now);
            Console.Read();
        }
    }

    #region MapReduce

    public class TwitMap : MapperBase
    {
        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public override void Map(string inputLine, MapperContext context)
        {
            int i = 0;
            SingleTweet Tweets = JsonConvert.DeserializeObject<SingleTweet>(StripHTML(inputLine)); ;


            var MaxSource = from k in Tweets.statuses group k by k.source into r select r.Max(n => n.source);

            var SourceNotNullCount = (from k in Tweets.statuses where k.source != null select k.source).Count();
            var CountryNotNullCount = (from k in Tweets.statuses where k.place != null select k.place).Count();

            //"Fri Apr 22 16:26:21 +0000 2016"
            var CreatedAt = (from k in Tweets.statuses group k by k.created_at into r select r.Key);
            Dictionary<long, int> LDates = new Dictionary<long, int>();
            foreach (var item in CreatedAt)
            {
                var ds = item.Split(' ');
                var dsT = ds[3].Split(':');
                DateTime d = new DateTime(int.Parse(ds[5]), 4, int.Parse(ds[2]), int.Parse(dsT[0]), int.Parse(dsT[1]), int.Parse(dsT[2]));
                var cd = (from k in Tweets.statuses where k.created_at == item select k.created_at).Count();
                try
                {
                    LDates.Add(d.Ticks, cd);
                }
                catch (Exception)
                {

                }
            }


            var MaxUser = (from k in Tweets.statuses group k by k.user.name into r select r.Max(n => n.user.name)).Take(10);
            var MaxCountry = (from k in Tweets.statuses where k.place != null group k by k.place into r select r.Max(n => n.place));



            var RetweetSum = (from k in Tweets.statuses select k.retweet_count).Sum();
            var FavouriteSum = (from k in Tweets.statuses select k.favorite_count).Sum();
            var tweetsCount = (from k in Tweets.statuses select k).Count();

            dynamic a = MaxCountry.ToList();
            dynamic ac = MaxSource.ToList();


            List<string> Countries = new List<string>();
            List<string> Sources = new List<string>();

            foreach (var item in a)
            {
                string[] Sp = Regex.Split(item.ToString(), "\"country\":");
                string r = Sp[1].Replace("\"", "");
                r = r.Remove(Sp[1].IndexOf(",")).ToString().Trim();
                Countries.Add(r.Trim(',', ' ').Trim());
            }

            foreach (var item in ac)
            {
                Sources.Add(item.Trim());
            }
            Dictionary<string, double> CountryCounter = new Dictionary<string, double>();
            Dictionary<string, double> SourceCounter = new Dictionary<string, double>();
            foreach (var c in Countries)
            {
                var CountCountry = (from k in Countries where k == c select k).Count();

                double y = ((double)CountCountry / (double)CountryNotNullCount) * 100;
                try
                {
                    CountryCounter.Add(c, Math.Round(y, 2));
                }
                catch (Exception)
                {

                }
            }

            foreach (var c in Sources)
            {
                var CountCountry = (from k in Sources where k == c select k).Count();
                double y = ((double)CountCountry / (double)SourceNotNullCount) * 100;
                try
                {
                    SourceCounter.Add(c, Math.Round(y, 2));
                }
                catch (Exception)
                {

                }
            }

            var uCountry = CountryCounter;
            var uSources = SourceCounter.Distinct().Take(10);

            List<string> Finish = new List<string>();
            Finish.Add(JsonConvert.SerializeObject(MaxUser));
            Finish.Add(JsonConvert.SerializeObject(uCountry));
            Finish.Add(JsonConvert.SerializeObject(uSources));
            Finish.Add(JsonConvert.SerializeObject(LDates.Take(100)));

            Finish.Add(JsonConvert.SerializeObject(RetweetSum));
            Finish.Add(JsonConvert.SerializeObject(FavouriteSum));
            Finish.Add(JsonConvert.SerializeObject(tweetsCount));

            var json = JsonConvert.SerializeObject(Finish);
            context.EmitKeyValue(json, i.ToString());
        }
    }
    public class TwitReducer : ReducerCombinerBase
    {
        public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
        {

            string all = "";
            foreach (string a in values)
            {
                all += a;
            }

            context.EmitLine(all);
        }
    }
    public class TwitExtractionJob : HadoopJob<TwitMap, TwitReducer>
    {
        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            var config = new HadoopJobConfiguration();
            // config.DeleteOutputFolder = true;
            config.InputPath = "/input/";
            config.OutputFolder = "/output/";
            return config;
        }
    }
    #endregion


    #region Classes

    public class Hashtag
    {
        public string text { get; set; }
        public List<int> indices { get; set; }
    }

    public class Medium2
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Large
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Small
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Thumb
    {
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
    }

    public class Sizes
    {
        public Medium2 medium { get; set; }
        public Large large { get; set; }
        public Small small { get; set; }
        public Thumb thumb { get; set; }
    }

    public class Medium
    {
        public object id { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public string type { get; set; }
        public Sizes sizes { get; set; }
    }

    public class Entities
    {
        public List<Hashtag> hashtags { get; set; }
        public List<object> symbols { get; set; }
        public List<object> user_mentions { get; set; }
        public List<object> urls { get; set; }
        public List<Medium> media { get; set; }
    }

    public class Metadata
    {
        public string iso_language_code { get; set; }
        public string result_type { get; set; }
    }

    public class Url2
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public List<int> indices { get; set; }
    }

    public class Url
    {
        public List<Url2> urls { get; set; }
    }

    public class Description
    {
        public List<object> urls { get; set; }
    }

    public class Entities2
    {
        public Url url { get; set; }
        public Description description { get; set; }
    }

    public class User
    {
        public long id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public Entities2 entities { get; set; }
        public bool @protected { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public int? utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public int statuses_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public bool is_translation_enabled { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_banner_url { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool has_extended_profile { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public bool following { get; set; }
        public bool follow_request_sent { get; set; }
        public bool notifications { get; set; }
    }

    public class Hashtag2
    {
        public string text { get; set; }
        public List<int> indices { get; set; }
    }

    public class UserMention
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
    }

    public class Url3
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public List<int> indices { get; set; }
    }

    public class Entities3
    {
        public List<Hashtag2> hashtags { get; set; }
        public List<object> symbols { get; set; }
        public List<UserMention> user_mentions { get; set; }
        public List<Url3> urls { get; set; }
    }

    public class Metadata2
    {
        public string iso_language_code { get; set; }
        public string result_type { get; set; }
    }

    public class Url5
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public List<int> indices { get; set; }
    }

    public class Url4
    {
        public List<Url5> urls { get; set; }
    }

    public class Description2
    {
        public List<object> urls { get; set; }
    }

    public class Entities4
    {
        public Url4 url { get; set; }
        public Description2 description { get; set; }
    }

    public class User2
    {
        public long id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public Entities4 entities { get; set; }
        public bool @protected { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public int? utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public int statuses_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public bool is_translation_enabled { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_banner_url { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool has_extended_profile { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public bool following { get; set; }
        public bool follow_request_sent { get; set; }
        public bool notifications { get; set; }
    }

    public class RetweetedStatus
    {
        public string created_at { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public Entities3 entities { get; set; }
        public bool truncated { get; set; }
        public Metadata2 metadata { get; set; }
        public string source { get; set; }
        public object in_reply_to_status_id { get; set; }
        public object in_reply_to_status_id_str { get; set; }
        public object in_reply_to_user_id { get; set; }
        public object in_reply_to_user_id_str { get; set; }
        public object in_reply_to_screen_name { get; set; }
        public User2 user { get; set; }
        public object geo { get; set; }
        public object coordinates { get; set; }
        public object place { get; set; }
        public object contributors { get; set; }
        public bool is_quote_status { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public bool possibly_sensitive { get; set; }
        public string lang { get; set; }
    }

    public class Status
    {
        public string created_at { get; set; }
        public object id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public Entities entities { get; set; }
        public bool truncated { get; set; }
        public Metadata metadata { get; set; }
        public string source { get; set; }
        public object in_reply_to_status_id { get; set; }
        public object in_reply_to_status_id_str { get; set; }
        public object in_reply_to_user_id { get; set; }
        public object in_reply_to_user_id_str { get; set; }
        public object in_reply_to_screen_name { get; set; }
        public User user { get; set; }
        public object geo { get; set; }
        public object coordinates { get; set; }
        public object place { get; set; }
        public object contributors { get; set; }
        public bool is_quote_status { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public bool possibly_sensitive { get; set; }
        public string lang { get; set; }
        public RetweetedStatus retweeted_status { get; set; }
    }

    public class SearchMetadata
    {
        public double completed_in { get; set; }
        public long max_id { get; set; }
        public string max_id_str { get; set; }
        public string next_results { get; set; }
        public string query { get; set; }
        public string refresh_url { get; set; }
        public int count { get; set; }
        public long since_id { get; set; }
        public string since_id_str { get; set; }
    }

    public class SingleTweet
    {
        public List<Status> statuses { get; set; }
        public SearchMetadata search_metadata { get; set; }
    }
    #endregion
}
