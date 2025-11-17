namespace HighElixir
{
    public struct IntervalCounter
    {
        private int _counter;
        private int _current;
        public IntervalCounter(int count, bool skipFirstSuccess = true)
        {
            _counter = count;
            _current = skipFirstSuccess ? 0 : count;
        }

        public bool Check
        {
            get
            {
                if (_current == _counter)
                {
                    _current = 0;
                    return true;
                }
                _current++;
                return false;
            }
        }

        public int Current => _current;
        public int Interval => _counter;
        public void Reset() => _current = _counter;
        public void Reset(int interval)
        {
            _counter = interval;
            Reset();
        }
    }
}