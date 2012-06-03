namespace ClientApp
{
    class MainPlayer : AdvertPlayer
    {
        public MainPlayer()
        {
            Instance = this;
        }

        public static MainPlayer _instance;

        public static MainPlayer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainPlayer();
                return _instance;
            }
            set { _instance = value; }
        }


        




    }
}
