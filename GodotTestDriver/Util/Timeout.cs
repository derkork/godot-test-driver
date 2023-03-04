using System;

namespace GodotTestDriver.Util
{
    public readonly struct Timeout
    {
        private readonly float _seconds;
        private readonly long _start;

        public Timeout(float seconds)
        {
            _seconds = seconds;
            _start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public bool IsReached => DateTimeOffset.Now.ToUnixTimeMilliseconds() - _start > 1000f * _seconds;
    }
}