using System.Windows.Threading;

namespace Launcher.Utils
{
    public class UpdateTimer
    {
        private readonly DispatcherTimer _timer;
        
        public UpdateTimer(TimeSpan interval, Action? tickAction)
        {
            _timer = new DispatcherTimer
            {
                Interval = interval
            };

            if (tickAction != null)
            {
                _timer.Tick += (s, e) => tickAction();
            }
        }

        public void StartTimer()
        {
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        public void SetInterval(TimeSpan interval)
        {
            _timer.Interval = interval;
        }
    }
}
