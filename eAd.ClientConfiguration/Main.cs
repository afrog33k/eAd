using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        LoadConfig();
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        var conf = Constants.CurrentClientConfiguration;
        DefaultDuration.Text = conf.DefaultDuration.ToString();
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

    private void button1_Click(object sender, EventArgs e)
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

            var app = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DesktopClient.exe";
            if(File.Exists(app))
            {
                Process.Start(app);
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Desktop Client App Not Installed", "Please ReInstall", MessageBoxButtons.OK);

            }




        }
        catch (Exception ex)
        {
            MessageBox.Show("Please Check Your Inputs and Try Again", "Invalid Input", MessageBoxButtons.OK);
        }
    }
}
}
