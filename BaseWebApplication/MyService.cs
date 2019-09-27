namespace BaseWebApplication
{
    public class MyService
    {
        private int _count;
        public int GetLastedCount()
        {
            _count++;
            return _count;
        }

        public void Reset()
        {
            _count = 0;
        }
    }
}
