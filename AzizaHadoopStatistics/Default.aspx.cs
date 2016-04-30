using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System.Drawing;

namespace AzizaHadoopStatistics
{
    public partial class _Default : Page
    {
        public string DD { get; set; }
        public string DDate { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                var a = new List<string>();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("aziza");
                Session["container"] = container;
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        var blobs = container.ListBlobs();
                        foreach (var blobItem in blobs)
                        {
                            string n = System.IO.Path.GetFileNameWithoutExtension(blobItem.Uri.ToString());
                            if (a.Contains(n) == false && blobItem.Uri.ToString().EndsWith("json"))
                                a.Add(n);
                        }
                    }
                }
                ddlFile.DataSource = a;
                ddlFile.DataBind();

            }
        }

        protected void Bind(CloudBlobContainer container, string fname)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("userout/" + fname.Trim() + ".txt");
        

            string aa = blockBlob.DownloadText();



            List<string> Finish = JsonConvert.DeserializeObject<List<string>>(aa);

            IEnumerable<string> MaxUser = JsonConvert.DeserializeObject<IEnumerable<string>>(Finish[0]);
            Dictionary<string, double> uCountry = JsonConvert.DeserializeObject<Dictionary<string, double>>(Finish[1]);
            IEnumerable<KeyValuePair<string, double>> uSources = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, double>>>(Finish[2]);
            List<KeyValuePair<long, int>> LDates = JsonConvert.DeserializeObject<List<KeyValuePair<long, int>>>(Finish[3]);
            int RetweetSum = JsonConvert.DeserializeObject<int>(Finish[4]);
            int FavouriteSum = JsonConvert.DeserializeObject<int>(Finish[5]);
            int tweetsCount = JsonConvert.DeserializeObject<int>(Finish[6]);

            Dictionary<string, int> Others = new Dictionary<string, int>();
            Dictionary<string, int> Dates = new Dictionary<string, int>();
            Others.Add("Retweets", RetweetSum);
            Others.Add("Favourites", FavouriteSum);
            Others.Add("tweets", tweetsCount);

            foreach (var item in LDates.Take(30))
            {
                DateTime d = new DateTime(item.Key);
                Dates.Add(d.ToString(), item.Value);
            }

            BulletedList1.DataSource = MaxUser;
            BulletedList1.DataBind();


            this.chDates.DataSource = Dates;
            chDates.Series["Series1"].IsXValueIndexed = true;
            chDates.ChartAreas["ChartArea1"].AxisX.Title = "Dates";
            chDates.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            chDates.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 12, FontStyle.Bold);
            chDates.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 12, FontStyle.Bold);
            foreach (KeyValuePair<string, int> employee in Dates)
                chDates.Series[0].Points.AddXY(employee.Key, employee.Value);
            this.chDates.DataBind();


            this.chCountries.DataSource = uCountry;
           chCountries.Series["Series1"].IsXValueIndexed = true;
           chCountries.ChartAreas["ChartArea1"].AxisX.Title = "Country Name";
           chCountries.ChartAreas["ChartArea1"].AxisY.Title = "Count";
           chCountries.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            chCountries.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            foreach (KeyValuePair<string, double> employee in uCountry)
                chCountries.Series[0].Points.AddXY(employee.Key, employee.Value * 100);
            this.chCountries.DataBind();


            this.chSources.DataSource = uSources;
            chSources.Series["Series1"].IsXValueIndexed = true;
            chSources.ChartAreas["ChartArea1"].AxisX.Title = "Source Name";
            chSources.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            chSources.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            chSources.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            foreach (KeyValuePair<string, double> employee in uSources)
                chSources.Series[0].Points.AddXY(employee.Key + (employee.Value * 100).ToString(), employee.Value * 100); // 
            this.chSources.DataBind();



            this.ChOthers.DataSource = Others;
            ChOthers.Series["Series1"].IsXValueIndexed = true;
            ChOthers.ChartAreas["ChartArea1"].AxisX.Title = "Name";
            ChOthers.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            ChOthers.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            ChOthers.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            foreach (KeyValuePair<string, int> employee in Others)
                ChOthers.Series[0].Points.AddXY(employee.Key, employee.Value);
            this.ChOthers.DataBind();

        }

        protected void ddlFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFile.SelectedIndex > 0)
            {
                pnlAll.Visible = true;
                CloudBlobContainer container = (CloudBlobContainer)Session["container"];
                Bind(container, ddlFile.SelectedValue);
            }
        }

        protected void ddlFile_DataBound(object sender, EventArgs e)
        {
            ddlFile.Items.Insert(0, new ListItem("Select ...", "0"));
        }
    }
}