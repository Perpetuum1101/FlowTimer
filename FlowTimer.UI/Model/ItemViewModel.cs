using FlowTimer.Data;
using FlowTimer.Data.Model;
using FlowTimer.UI.Utils;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlowTimer.UI.Model
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        public bool Focus;
        public event PropertyChangedEventHandler PropertyChanged;

        #region private fields

        private ViewModel _parent;
        private Timer _timer;

        private ItemDTO _item;
        private string _currentTimeText;
        private TimeSpan _currentTime;
        private bool _isChecked;
        private string _currentText;

        private const string DefaultText = "ToDo";
        private const string CompletedTask = "--";
        private const string RunTask = ">>";
        private const string StopTask = "[]";


        #endregion

        #region constructors

        public ItemViewModel(ViewModel parent)
        {
            Init(parent);
            CreateItem();
            _currentText = _item.Text;
            _currentTime = TimeSpan.FromSeconds(0);
            Focus = true;
        }

        public ItemViewModel(ViewModel parent, ItemDTO item)
        {
            Init(parent);
            _item = item;
            if(_item.Status == Status.InProgress)
            {
                _item.Status = Status.Stoped;
            }
            else if(_item.Status == Status.Completed)
            {
                IsChecked = true;
                CurrentTimeText = CompletedTask;
            }
            Text = item.Text;
            _currentText = Text;
            _currentTime = TimeSpan.FromSeconds(item.CurrentTimeSeconds);
            Focus = false;
        }

        #endregion

        #region properties

        public string Text
        {
            get { return _item.Text; }
            set { _item.Text = value; OnPropertyChanged(); }
        }
        public int TotalTime
        {
            get { return _item.TotalTimeMinutes; }
            set { _item.TotalTimeMinutes = value; OnPropertyChanged(); }
        }
        public string CurrentTimeText
        {
            get { return _currentTimeText; }
            set { _currentTimeText = value; OnPropertyChanged(); }
        }
        public bool IsChecked { get { return _isChecked; } set { _isChecked = value; OnPropertyChanged(); } }

        public ICommand CheckboxCheckedCommand { get; set; }
        public ICommand StartTimerCommand { get; set; }
        public ICommand TextEnterCommand { get; set; }

        #endregion

        #region public

        public void OnBreak()
        {
            CurrentTimeText = CurrentTimeText.Replace(">>", string.Empty);
        }

        public async Task OnCloseAsync()
        {
            if (_item.Status != Status.InProgress)
            {
                return;
            }

            _timer.Stop(true);
            _item.CurrentTimeSeconds = 0;
            _item.Status = Status.Stoped;
            await _parent.Repository.Update(_item);
        }

        #endregion

        #region protected

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region private

        #region command methods

        public void Checked(object value)
        {
            if (_item.Status == Status.Completed)
            {
                _item.Status = Status.Stoped;
                CurrentTimeText = ">>";
            }
            else
            {
                _item.Status = Status.Completed;
                _timer.Stop(true);
                _parent.IsWork = false;
                _item.CurrentTimeSeconds = 0;
                CurrentTimeText = "--";
                _parent.Sound.Play(SoundNames.Complete);
                
            }
            
            Update();
        }

        private bool CanBeChecked(object value)
        {
            var isIdleAndStopped = _parent.IsIdle && _item.Status is Status.Stoped;
            var isWorkInProgress = _parent.IsWork && _item.Status == Status.InProgress;
            var isCompleted = _item.Status == Status.Completed;
            var canStart = isIdleAndStopped || isWorkInProgress || isCompleted;

            return canStart;
        }

        private void StartTimer(object value)
        {
            if (_item.Status == Status.New || _item.Status == Status.Stoped)
            {
                _parent.IsWork = true;
                _currentTime = TimeSpan.FromSeconds(0);
                _timer.Start();
                _item.Status = Status.InProgress;
                _item.CurrentTimeSeconds = 0;
                Update();
            }
            else if (_item.Status == Status.InProgress)
            {
                _parent.IsWork = false;
                _item.Status = Status.Stoped;
                _timer.Stop(true);
                _item.CurrentTimeSeconds = (int)_currentTime.TotalSeconds;
                Update();
            }
        }

        private bool CanStartTimer(object value)
        {
            var isIdleAndNewOrStopped = _parent.IsIdle &&
                _item.Status is Status.New or Status.Stoped;
            var isWorkInProgress = _parent.IsWork && _item.Status == Status.InProgress;
            var canStart = isIdleAndNewOrStopped || isWorkInProgress;

            return canStart;
        }

        private async void TextEnter(object value)
        {
            var text = (string)value;
            if (_currentText != text)
            {
                _item.Text = text;
                _currentText = text;
                if (string.IsNullOrEmpty(text))
                {
                    _parent.Repository.Delete(_item.Id.Value);
                    _parent.Sound.Play(SoundNames.Delete);
                }
                else
                {
                    await _parent.Repository.Update(_item);
                }
            }
        }

        private bool CanTextEnter(object value)
        {
            return true;
        }

        #endregion

        private async void Update()
        {
            await _parent.Repository.Update(_item);
        }

        private void Init(ViewModel parent)
        {
            _parent = parent;
            _timer = new Timer(_parent.Dispatcher);
            CurrentTimeText = RunTask;
            CheckboxCheckedCommand = new RelayCommand(Checked, CanBeChecked);
            StartTimerCommand = new RelayCommand(StartTimer, CanStartTimer);
            TextEnterCommand = new RelayCommand(TextEnter, CanTextEnter);
            _timer.OnTick += OnTick;
            _timer.OnStop += OnStop;
        }

        private void OnTick(TimeSpan time)
        {
            CurrentTimeText = time.ToString(@"mm\:ss") + StopTask;
        }

        private void OnStop(TimeSpan time)
        {
            _currentTime = time;
            TotalTime += (int)_currentTime.TotalMinutes;
            CurrentTimeText = _currentTime.ToString(@"mm\:ss") + RunTask;
        }

        private async void CreateItem()
        {
            _item = await _parent.Repository.Create(DefaultText);
            OnPropertyChanged(nameof(Text));
        }

        #endregion
    }
}
