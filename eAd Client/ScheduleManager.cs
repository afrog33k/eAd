namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.Properties;
    using eAd.DataViewModels;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    public class ScheduleManager
    {
        private CacheManager _cacheManager;
        private Collection<LayoutSchedule> _currentSchedule;
        private Collection<LayoutSchedule> _layoutSchedule;
        private string _location;

        public ScheduleManager(CacheManager cacheManager, string scheduleLocation)
        {
            this._cacheManager = cacheManager;
            this._location = scheduleLocation;
            this._layoutSchedule = new Collection<LayoutSchedule>();
            this._currentSchedule = new Collection<LayoutSchedule>();
            this.IsNewScheduleAvailable();
        }

        private static LayoutSchedule CreateLayoutSchedule(ScheduleLayout layout)
        {
            LayoutSchedule schedule = new LayoutSchedule {
                LayoutFile = layout.File
            };
            string s = schedule.LayoutFile.TrimEnd(".xml".ToCharArray());
            schedule.LayoutFile = Settings.Default.LibraryPath + @"\Layouts\" + s + ".mosaic";
            schedule.ID = int.Parse(s);
            if (schedule.NodeName != "default")
            {
                schedule.Priority = layout.Priority;
                schedule.FromDate = DateTime.Parse(layout.FromDate);
                schedule.ToDate = DateTime.Parse(layout.ToDate);
                int scheduleId = -1;
                if (layout.ScheduleId != -1)
                {
                    scheduleId = layout.ScheduleId;
                }
                if (scheduleId != -1)
                {
                    schedule.Scheduleid = scheduleId;
                }
            }
            return schedule;
        }

        private ScheduleModel GetSchedule()
        {
            if (File.Exists(this._location))
            {
                using (FileStream stream = File.Open(this._location, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ScheduleModel));
                    return (ScheduleModel) serializer.Deserialize(stream);
                }
            }
            return new ScheduleModel { Items = new List<ScheduleLayout>() };
        }

        private bool IsNewScheduleAvailable()
        {
            lock (this._layoutSchedule)
            {
                if ((this._layoutSchedule.Count == 0) || this.RefreshSchedule)
                {
                    try
                    {
                        this.LoadScheduleFromFile();
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(new LogMessage("IsNewScheduleAvailable", string.Format("Unable to load schedule from disk: {0}", exception.Message)), LogType.Error.ToString());
                        this.SetEmptySchedule();
                    }
                    this.RefreshSchedule = false;
                }
                Collection<LayoutSchedule> collection = this.LoadNewSchdule();
                bool flag = false;
                if (this._currentSchedule.Count == 0)
                {
                    flag = true;
                }
                foreach (LayoutSchedule schedule in this._currentSchedule)
                {
                    if (!collection.Contains(schedule))
                    {
                        flag = true;
                    }
                }
                this._currentSchedule = collection;
             
                collection = null;
                return flag;
            }
        }

        private Collection<LayoutSchedule> LoadNewSchdule()
        {
            Collection<LayoutSchedule> collection = new Collection<LayoutSchedule>();
            Collection<LayoutSchedule> collection2 = new Collection<LayoutSchedule>();
            LayoutSchedule item = new LayoutSchedule();
            foreach (LayoutSchedule schedule2 in this._layoutSchedule)
            {
                try
                {
                    if (!this._cacheManager.IsValidLayout(schedule2.ID + ".mosaic"))
                    {
                        continue;
                    }
                }
                catch
                {
                    Trace.WriteLine("Unable to determine if layout is valid or not");
                    continue;
                }
                if (schedule2.NodeName == "default")
                {
                    item = schedule2;
                }
                else if ((schedule2.FromDate <= DateTime.Now) && (schedule2.ToDate >= DateTime.Now))
                {
                    if (schedule2.Priority != 0)
                    {
                        collection2.Add(schedule2);
                        continue;
                    }
                    collection.Add(schedule2);
                }
            }
            if (collection2.Count > 0)
            {
                return collection2;
            }
            if (collection.Count == 0)
            {
                collection.Add(item);
            }
            return collection;
        }

        private void LoadScheduleFromFile()
        {
            this._layoutSchedule.Clear();
            ScheduleModel model = this.GetSchedule();
            List<ScheduleLayout> second = (from s in model.Items
                where s.Type == "General"
                select s).ToList<ScheduleLayout>();
            ScheduleLayout layout = model.Items.Except<ScheduleLayout>(second).FirstOrDefault<ScheduleLayout>();
            if (layout != null)
            {
                this.ProfileLayout = CreateLayoutSchedule(layout);
            }
            if (second.Count == 0)
            {
                this.SetEmptySchedule();
            }
            else
            {
                foreach (ScheduleLayout layout2 in second)
                {
                    LayoutSchedule item = CreateLayoutSchedule(layout2);
                    this._layoutSchedule.Add(item);
                }
                second = null;
                model = null;
            }
        }

        private void SetEmptySchedule()
        {
            this._layoutSchedule.Clear();
            LayoutSchedule item = new LayoutSchedule {
                LayoutFile = Settings.Default.LibraryPath + @"\Default.xml",
                ID = 0,
                Scheduleid = 0
            };
            this._layoutSchedule.Add(item);
        }

        public Collection<LayoutSchedule> CurrentSchedule
        {
            get
            {
                return this._currentSchedule;
            }
        }

        public bool NewScheduleAvailable
        {
            get
            {
                return this.IsNewScheduleAvailable();
            }
        }

        public LayoutSchedule ProfileLayout { get; set; }

        public bool RefreshSchedule { get; set; }
    }
}

