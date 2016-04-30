using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication2.MyClasses;

namespace WindowsFormsApplication2
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                lblTitle.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

                FillData(openFileDialog1.FileName);
            }
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("aziza");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("userout/ساعه_استجابه.txt");
            string aa = blockBlob.DownloadText();
            lblTitle.Text = "ساعه_استجابه";
            FillData(aa);

        }


        /// <summary>
        /// Converts json object into charts
        /// </summary>
        /// <param name="fileName"></param>
        protected void FillData(string fileName)
        {
            TwitterJsonReader tj = new TwitterJsonReader(fileName);


            //Countries
            var uCountry = tj.CountriesReader();
            this.chCountries.DataSource = uCountry;
            chCountries.Series["Countries"].IsXValueIndexed = true;
            chCountries.ChartAreas["ChartArea1"].AxisX.Title = "Country Name";
            chCountries.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            chCountries.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            chCountries.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            foreach (KeyValuePair<string, double> employee in uCountry)
                chCountries.Series[0].Points.AddXY(employee.Key, employee.Value * 100);
            this.chCountries.DataBind();

            Random random = new Random();
            foreach (var item in chCountries.Series[0].Points)
            {
                Color c = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                item.Color = c;
            }


            //Users
            var MaxUser = tj.UsersReader();
      

            List<string> ff = MaxUser.ToList();
            for (int i = 0; i <= ff.Count-1; i++)
            {
                listBox1.Items.Add((i + 1).ToString() + "- " + ff[i]);
            }


            //Sources
            var uSources = tj.SourceReader();
            this.chSources.DataSource = uSources;
            chSources.Series["Sources"].IsXValueIndexed = true;
            chSources.ChartAreas["ChartArea1"].AxisX.Title = "Source Name";
            chSources.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            chSources.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            chSources.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            foreach (KeyValuePair<string, double> employee in uSources)
                chSources.Series[0].Points.AddXY(employee.Key + (employee.Value * 100).ToString(), employee.Value * 100); // 
            this.chSources.DataBind();



            //Others
            var Others = tj.OtherReader();
            this.ChOthers.DataSource = Others;
            ChOthers.Series["Series1"].IsXValueIndexed = true;
            ChOthers.ChartAreas["ChartArea1"].AxisX.Title = "Name";
            ChOthers.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            ChOthers.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            ChOthers.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            foreach (KeyValuePair<string, int> employee in Others)
                ChOthers.Series[0].Points.AddXY(employee.Key, employee.Value);
            this.ChOthers.DataBind();

            //Timeline
            var Dates = tj.TimelineReader();
            this.chDates.DataSource = Dates;
            chDates.Series["Timeline"].IsXValueIndexed = true;
            chDates.ChartAreas["ChartArea1"].AxisX.Title = "Dates";
            chDates.ChartAreas["ChartArea1"].AxisY.Title = "Count";
            chDates.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Sans Serif", 12, FontStyle.Bold);
            chDates.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Sans Serif", 12, FontStyle.Bold);
            foreach (KeyValuePair<string, int> employee in Dates)
                chDates.Series[0].Points.AddXY(employee.Key, employee.Value);
            this.chDates.DataBind();
        }
    }
}
