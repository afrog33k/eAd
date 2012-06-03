namespace ClientApp.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
        }

        [DebuggerNonUserCode, DefaultSettingValue("blacklist.xml"), ApplicationScopedSetting]
        public string blackListLocation
        {
            get
            {
                return (string) this["blackListLocation"];
            }
        }

        [DefaultSettingValue("cacheManager.xml"), ApplicationScopedSetting, DebuggerNonUserCode]
        public string CacheManagerFile
        {
            get
            {
                return (string) this["CacheManagerFile"];
            }
        }

        [DebuggerNonUserCode, ApplicationScopedSetting, DefaultSettingValue("1.3.2")]
        public string ClientVersion
        {
            get
            {
                return (string) this["ClientVersion"];
            }
        }

        [DefaultSettingValue("900"), UserScopedSetting, DebuggerNonUserCode]
        public decimal collectInterval
        {
            get
            {
                return (decimal) this["collectInterval"];
            }
            set
            {
                this["collectInterval"] = value;
            }
        }

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue("COMPUTERNAME"), UserScopedSetting]
        public string displayName
        {
            get
            {
                return (string) this["displayName"];
            }
            set
            {
                this["displayName"] = value;
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool DoubleBuffering
        {
            get
            {
                return (bool) this["DoubleBuffering"];
            }
            set
            {
                this["DoubleBuffering"] = value;
            }
        }

        [DefaultSettingValue("10"), UserScopedSetting, DebuggerNonUserCode]
        public decimal emptyLayoutDuration
        {
            get
            {
                return (decimal) this["emptyLayoutDuration"];
            }
            set
            {
                this["emptyLayoutDuration"] = value;
            }
        }

        [DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
        public bool EnableMouse
        {
            get
            {
                return (bool) this["EnableMouse"];
            }
            set
            {
                this["EnableMouse"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool expireModifiedLayouts
        {
            get
            {
                return (bool) this["expireModifiedLayouts"];
            }
            set
            {
                this["expireModifiedLayouts"] = value;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting]
        public string hardwareKey
        {
            get
            {
                return (string) this["hardwareKey"];
            }
            set
            {
                this["hardwareKey"] = value;
            }
        }

        [DefaultSettingValue("DEFAULT"), DebuggerNonUserCode, UserScopedSetting]
        public string LibraryPath
        {
            get
            {
                return (string) this["LibraryPath"];
            }
            set
            {
                this["LibraryPath"] = value;
            }
        }

        [DefaultSettingValue("0"), DebuggerNonUserCode, UserScopedSetting]
        public int licensed
        {
            get
            {
                return (int) this["licensed"];
            }
            set
            {
                this["licensed"] = value;
            }
        }

        [ApplicationScopedSetting, DebuggerNonUserCode, DefaultSettingValue("log.xml")]
        public string logLocation
        {
            get
            {
                return (string) this["logLocation"];
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("0")]
        public decimal offsetX
        {
            get
            {
                return (decimal) this["offsetX"];
            }
            set
            {
                this["offsetX"] = value;
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("0")]
        public decimal offsetY
        {
            get
            {
                return (decimal) this["offsetY"];
            }
            set
            {
                this["offsetY"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("False"), DebuggerNonUserCode]
        public bool powerpointEnabled
        {
            get
            {
                return (bool) this["powerpointEnabled"];
            }
            set
            {
                this["powerpointEnabled"] = value;
            }
        }

        [DefaultSettingValue(""), DebuggerNonUserCode, UserScopedSetting]
        public string ProxyDomain
        {
            get
            {
                return (string) this["ProxyDomain"];
            }
            set
            {
                this["ProxyDomain"] = value;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting]
        public string ProxyPassword
        {
            get
            {
                return (string) this["ProxyPassword"];
            }
            set
            {
                this["ProxyPassword"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("")]
        public string ProxyPort
        {
            get
            {
                return (string) this["ProxyPort"];
            }
            set
            {
                this["ProxyPort"] = value;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting]
        public string ProxyUser
        {
            get
            {
                return (string) this["ProxyUser"];
            }
            set
            {
                this["ProxyUser"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("requiredFiles.xml")]
        public string RequiredFilesFile
        {
            get
            {
                return (string) this["RequiredFilesFile"];
            }
            set
            {
                this["RequiredFilesFile"] = value;
            }
        }

        [DefaultSettingValue("schedule.xml"), ApplicationScopedSetting, DebuggerNonUserCode]
        public string ScheduleFile
        {
            get
            {
                return (string) this["ScheduleFile"];
            }
        }

        [UserScopedSetting, DefaultSettingValue("1"), DebuggerNonUserCode]
        public decimal scrollStepAmount
        {
            get
            {
                return (decimal) this["scrollStepAmount"];
            }
            set
            {
                this["scrollStepAmount"] = value;
            }
        }

        [DefaultSettingValue("serverkey"), DebuggerNonUserCode, UserScopedSetting]
        public string ServerKey
        {
            get
            {
                return (string) this["ServerKey"];
            }
            set
            {
                this["ServerKey"] = value;
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("http://localhost/")]
        public string serverURI
        {
            get
            {
                return (string) this["serverURI"];
            }
            set
            {
                this["serverURI"] = value;
            }
        }

        [DefaultSettingValue("0"), DebuggerNonUserCode, UserScopedSetting]
        public decimal sizeX
        {
            get
            {
                return (decimal) this["sizeX"];
            }
            set
            {
                this["sizeX"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("0")]
        public decimal sizeY
        {
            get
            {
                return (decimal) this["sizeY"];
            }
            set
            {
                this["sizeY"] = value;
            }
        }

        [DefaultSettingValue("True"), DebuggerNonUserCode, UserScopedSetting]
        public bool statsEnabled
        {
            get
            {
                return (bool) this["statsEnabled"];
            }
            set
            {
                this["statsEnabled"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("50")]
        public int StatsFlushCount
        {
            get
            {
                return (int) this["StatsFlushCount"];
            }
            set
            {
                this["StatsFlushCount"] = value;
            }
        }

        [ApplicationScopedSetting, DebuggerNonUserCode, DefaultSettingValue("stats.xml")]
        public string StatsLogFile
        {
            get
            {
                return (string) this["StatsLogFile"];
            }
        }

        [DefaultSettingValue("3"), ApplicationScopedSetting, DebuggerNonUserCode]
        public string Version
        {
            get
            {
                return (string) this["Version"];
            }
        }

        [DefaultSettingValue("cool"), DebuggerNonUserCode, UserScopedSetting]
        public string Xmds
        {
            get
            {
                return (string) this["Xmds"];
            }
            set
            {
                this["Xmds"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("04/13/2012 15:46:00")]
        public DateTime XmdsLastConnection
        {
            get
            {
                return (DateTime) this["XmdsLastConnection"];
            }
            set
            {
                this["XmdsLastConnection"] = value;
            }
        }

        [ApplicationScopedSetting, DebuggerNonUserCode, DefaultSettingValue("900")]
        public int xmdsResetTimeout
        {
            get
            {
                return (int) this["xmdsResetTimeout"];
            }
        }
    }
}

