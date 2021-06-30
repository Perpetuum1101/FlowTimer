using FlowTimer.UI.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using FlowTimer.Data;
using System.Runtime.CompilerServices;
using System;
using System.Windows.Threading;
using FlowTimer.UI.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FlowTimer.UI.Model
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region private fields

        private ObservableCollection<ItemViewModel> _items;
        private string _breakButtonText;
        private bool _isBreak;
        private bool _isIdle;
        private bool _isWork;
        private bool _notWork;

        private readonly Repository _repository;
        private readonly Dispatcher _dispatcher;
        private readonly Timer _breakTimer;
        private readonly Sound _sound;

        private const string MiddleButtonText = "10 min";

        #endregion

        #region constructors

        public ViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _repository = new Repository();
            _breakTimer = new Timer(_dispatcher);
            _breakTimer.OnTick += OnTick;
            _breakTimer.OnStop += OnStop;
            _sound = new Sound();

            InitProperties();
        }

        #endregion

        #region properties

        public ItemViewModel TopItem => Items.Count > 0 ? Items[0] : null;
        public Dispatcher Dispatcher => _dispatcher;
        public Repository Repository => _repository;
        public Sound Sound => _sound;

        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(); }
        }
        public string BreakButtonText
        {
            get { return _breakButtonText; }
            set { _breakButtonText = value; OnPropertyChanged(); }
        }
        public bool IsBreak
        {
            get { return _isBreak; }
            set
            {
                _isBreak = value;
                OnPropertyChanged();
                IsIdle = !_isBreak;
                foreach (var item in _items)
                {
                    item.OnBreak();
                }
            }
        }
        public bool IsIdle
        {
            get { return _isIdle; }
            set { _isIdle = value; OnPropertyChanged(); }
        }
        public bool IsWork
        {
            get { return _isWork; }
            set
            {
                _isWork = value;
                OnPropertyChanged();
                IsIdle = !_isWork;
                NotWork = !_isWork;
            }
        }

        public bool NotWork
        {
            get { return _notWork; }
            set { _notWork = value; OnPropertyChanged(); }
        }

        public ICommand AddButtonCommand { get; set; }
        public ICommand StartTimerCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExportAllCommand { get; set; }

        #endregion

        #region public

        public async Task OnCloseAsync()
        {
            foreach(var item in _items)
            {
                await item.OnCloseAsync();
            }
        }

        #endregion

        #region protected

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region private

        private void InitProperties()
        {
            LoadItems();
            AddButtonCommand = new RelayCommand(Add, CanAdd);
            StartTimerCommand = new RelayCommand(StartTimer, CanStartTimer);
            ExportCommand = new RelayCommand(Export, CanExport);
            ExportAllCommand = new RelayCommand(ExportAll, CanExport);
            BreakButtonText = MiddleButtonText;
            IsIdle = true;
            NotWork = true;
        }

        private async void LoadItems()
        {
            Items = new ObservableCollection<ItemViewModel>();
            var items = await _repository.GetItems();
            foreach (var item in items)
            {
                var newItem = new ItemViewModel(this, item);
                Items.Add(newItem);
            }
        }

        private void OnTick(TimeSpan time)
        {
            BreakButtonText = time.ToString(@"mm\:ss");
        }

        private void OnStop(TimeSpan time)
        {
            IsBreak = false;
            BreakButtonText = MiddleButtonText;
            _sound.Play(SoundNames.Notify);
        }

        #region command methods

        private void StartTimer(object value)
        {
            if (IsBreak)
            {
                _breakTimer.Stop(false);
                IsBreak = false;
                BreakButtonText = MiddleButtonText;
            }
            else
            {
                IsBreak = true;
                var amount = Convert.ToInt32(value);
                _breakTimer.StartCountdown(amount);
            }
        }

        private bool CanStartTimer(object value)
        {
            return IsIdle || IsBreak;
        }

        private void Export(object value)
        {
            Report.GenerateSummary(_repository);
        }

        private void ExportAll(object value)
        {
            Report.GenerateFull(_repository);
        }

        private bool CanExport(object value)
        {
            return true;
        }


        private void Add(object value)
        {
            var item = new ItemViewModel(this);
            Items.Insert(0, item);
            _sound.Play(SoundNames.Add);
        }

        private bool CanAdd(object value)
        {
            return true;
        }

        #endregion

        #endregion
    }
}
