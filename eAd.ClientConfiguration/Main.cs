using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eAd.Utilities;

namespace eAd.ClientConfiguration
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
          var conf =   Constants.CurrentClientConfiguration;
         DefaultDuration.Text=    conf.DefaultDuration.ToString();
         ApplicationPath.Text = conf.AppPath;
         MessageWaitTime.Text = conf.MessageWaitTime.ToString();
         StationID.Text = conf.MyStationID.ToString();
         PlayListFile.Text = conf.PlayListFile;
         ServerUrl.Text = conf.ServerUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
            
                Constants.CurrentClientConfiguration.DefaultDuration = Convert.ToDouble(DefaultDuration.Text);
                Constants.CurrentClientConfiguration.AppPath = ApplicationPath.Text;
                Constants.CurrentClientConfiguration.MessageWaitTime = Convert.ToInt32(MessageWaitTime.Text);
                Constants.CurrentClientConfiguration.MyStationID = Convert.ToInt32(StationID.Text);
                Constants.CurrentClientConfiguration.PlayListFile = PlayListFile.Text;
                Constants.CurrentClientConfiguration.ServerUrl = ServerUrl.Text;
                Constants.SaveDefaults();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please Check Your Inputs and Try Again","Invalid Input",MessageBoxButtons.OK);
            }
        }
    }
}
