namespace Game
{
    public static class Frame
    {
        private static int _rate = 1;

        public static int Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }
    }
}
