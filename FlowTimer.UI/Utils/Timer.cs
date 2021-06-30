using System;
using System.Windows.Threading;

namespace FlowTimer.UI.Utils
{
    public class Timer
    {
        public event Action<TimeSpan> OnTick;
        public event Action<TimeSpan> OnStop;

        private TimeSpan _time;
        private DispatcherTimer _timer;
        private readonly Dispatcher _dispatcher;
        private static readonly TimeSpan _tickAmount = TimeSpan.FromSeconds(1);

        public Timer(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Start()
        {
            _time = TimeSpan.FromMinutes(0);
            _timer = new DispatcherTimer(_tickAmount, DispatcherPriority.Normal, TickUp, _dispatcher);
            _timer.Start();
        }

        public void StartCountdown(int amount)
        {
            _time = TimeSpan.FromMinutes(amount);
            _timer = new DispatcherTimer(_tickAmount, DispatcherPriority.Normal, TickDown, _dispatcher);
            _timer.Start();
        }

        public void Stop(bool invoke)
        {
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
            }
            if (invoke)
            {
                OnStop?.Invoke(_time);
            }
        }

        private void TickDown(object sender, EventArgs e)
        {
            OnTick?.Invoke(_time);
            if (_time == TimeSpan.Zero)
            {
                _timer.Stop();
                OnStop?.Invoke(_time);
            }
            _time = _time.Add(TimeSpan.FromSeconds(-1));
        }

        private void TickUp(object sender, EventArgs e)
        {
            OnTick?.Invoke(_time);
            _time = _time.Add(TimeSpan.FromSeconds(1));
        }
    }
}
