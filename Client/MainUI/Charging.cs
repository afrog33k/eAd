using System.Threading;
using System.Windows;
using ClientApp.Core;
using ClientApp.Widgets;

namespace ClientApp
{
    class Charging : AdvertPlayer
    {
        public Charging()
        {
            Instance = this;
        }
        public static string CurrentRFID;

        public static Charging _instance;
        public void Update(int time)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Thread.Sleep(5000);
                Switcher.Switch(this);
                Thread.Sleep(15000);
                Switcher.Switch(MainPlayer.Instance);
            });
        }

        public override void Pause()
        {
            (WidgetsFactory
                .Widgets["LocationInfo"] as Location).Visibility =
                   Visibility.Hidden;
            base.Pause();
        }
        public static Charging Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Charging();
                return _instance;
            }
            set { _instance = value; }
        }
    }
}
