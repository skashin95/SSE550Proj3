using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleDefence.ViewModel
{
    using View;
    using Model;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Windows.Foundation;
    using DispatcherTimer = Windows.UI.Xaml.DispatcherTimer;
    using FrameworkElement = Windows.UI.Xaml.FrameworkElement;

    class VehicleDefenceViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FrameworkElement>
                    _sprites = new ObservableCollection<FrameworkElement>();
        public INotifyCollectionChanged Sprites { get { return _sprites; } }

        public bool GameOver { get { return _model.GameOver; } }

        private readonly ObservableCollection<object> _lives = new ObservableCollection<object>();
        public INotifyCollectionChanged Lives { get { return _lives; } }

        public bool Paused { get; set; }
        private bool _lastPaused = true;

        public static double Scale { get; private set; }

        public int Score { get; private set; }

        public Size PlayAreaSize
        {
            set
            {
                Scale = value.Width / 405;
                _model.UpdateAllMilitaryVehicles();

                RecreateScanLines();
            }
        }

        private readonly AircraftModel _model = new AircraftModel();
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private FrameworkElement _playerControl = null;
        private bool _playerFlashing = false;

        private readonly Dictionary<Aircraft, FrameworkElement> _aircrafts = new Dictionary<Aircraft, FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _shotAircrafts = new Dictionary<FrameworkElement, DateTime>();
        private readonly Dictionary<Shot, FrameworkElement> _shots = new Dictionary<Shot, FrameworkElement>();
        private readonly List<FrameworkElement> _scanLines = new List<FrameworkElement>();

        public void AircaftsViewModel()
        {
            Scale = 1;

            _model.MilitaryVehicleChanged += ModelVehicleChangedEventHandler;
            _model.ShotMoved += ModelShotMovedEventHandler;
            
            _timer.Interval = TimeSpan.FromMilliseconds(66);
            _timer.Tick += TimerTickEventHandler;

            _model.EndGame();
        }

        

        public void StartGame()
        {
            Paused = false;
            foreach (var aircraft in _aircrafts.Values) _sprites.Remove(aircraft);
            foreach (var shot in _shots.Values) _sprites.Remove(shot);
            _model.StartGame();
            OnPropertyChanged("GameOver");
            _timer.Start();
        }

        private void RecreateScanLines()
        {
            foreach (FrameworkElement scanLine in _scanLines)
                if (_sprites.Contains(scanLine))
                    _sprites.Remove(scanLine);
            _scanLines.Clear();
            for (int y = 0; y < 300; y += 2)
            {
                FrameworkElement scanLine = VehicleDefenceHelper.ScanLineFactory(y, 400, Scale);
                _scanLines.Add(scanLine);
                _sprites.Add(scanLine);
            }
        }

        void TimerTickEventHandler(object sender, object e)
        {
            if (_lastPaused != Paused)
            {
                OnPropertyChanged("Paused");
                _lastPaused = Paused;
            }
            if (!Paused)
            {
                if (_leftAction.HasValue && _rightAction.HasValue)
                    _model.MovePlayer(_leftAction > _rightAction ? Direction.Left : Direction.Right);
                else if (_leftAction.HasValue)
                    _model.MovePlayer(Direction.Left);
                else if (_rightAction.HasValue)
                    _model.MovePlayer(Direction.Right);
            }

            _model.Update(Paused);

            if (Score != _model.Score)
            {
                Score = _model.Score;
                OnPropertyChanged("Score");
            }

            if (_model.Lives >= 0)
            {
                while (_lives.Count > _model.Lives)
                    _lives.RemoveAt(0);
                while (_lives.Count < _model.Lives)
                    _lives.Add(new object());
            }

            foreach (FrameworkElement control in _shotAircrafts.Keys.ToList())
            {
                DateTime elapsed = _shotAircrafts[control];
                if (DateTime.Now - elapsed > TimeSpan.FromSeconds(.5)) {
                    _sprites.Remove(control);
                    _shotAircrafts.Remove(control);
                }
            }

            if (_model.GameOver)
            {
                OnPropertyChanged("GameOver");
                _timer.Stop();
            }
        }

        void ModelVehicleChangedEventHandler(object sender, MilitaryVehicleChangedEventArgs e)
        {
            if (!e.Killed)
            {
                if (e.MilitaryVehcileUpdated is Aircraft)
                {
                    Aircraft aircraft = e.MilitaryVehcileUpdated as Aircraft;
                    if (!_aircrafts.ContainsKey(aircraft))
                    {
                        FrameworkElement aircraftControl = VehicleDefenceHelper.AircraftControlFactory(aircraft, Scale);
                        _aircrafts[aircraft] = aircraftControl;
                        _sprites.Add(aircraftControl);
                    }
                    else
                    {
                        FrameworkElement aircraftControl = _aircrafts[aircraft];
                        VehicleDefenceHelper.MoveElementOnCanvas(aircraftControl, aircraft.Location.X * Scale, aircraft.Location.Y * Scale);
                        VehicleDefenceHelper.ResizeElement(aircraftControl, aircraft.Size.Width * Scale, aircraft.Size.Height * Scale);
                      
                    }
                }
                else if (e.MilitaryVehcileUpdated is Player)
                {
                    if (_playerFlashing)
                    {
                        _playerFlashing = false;
                        AnimatedImage control = _playerControl as AnimatedImage;
                        if (control != null)
                            control.StopFlashing();
                    }

                    Player player = e.MilitaryVehcileUpdated as Player;
                    if (_playerControl == null)
                    {
                        _playerControl = VehicleDefenceHelper.PlayerControlFactory(player, Scale);
                        _sprites.Add(_playerControl);
                    }
                    else
                    {
                        VehicleDefenceHelper.MoveElementOnCanvas(_playerControl, player.Location.X * Scale, player.Location.Y * Scale);
                        VehicleDefenceHelper.ResizeElement(_playerControl, player.Size.Width * Scale, player.Size.Height * Scale);
                    }
                }
            }
            else
            {
                if (e.MilitaryVehcileUpdated is Aircraft) 
                {
                    Aircraft aircraft = e.MilitaryVehcileUpdated as Aircraft;
                    if (!_aircrafts.ContainsKey(aircraft)) return;
                    AnimatedImage aircraftControl = _aircrafts[aircraft] as AnimatedImage;
                    if (aircraftControl != null)
                    {
                        aircraftControl.AircraftShot();
                        _shotAircrafts[aircraftControl] = DateTime.Now;
                        _aircrafts.Remove(aircraft);
                    }
                }
                else if (e.MilitaryVehcileUpdated is Player)
                {
                    AnimatedImage control = _playerControl as AnimatedImage;
                    if (control != null)
                        control.StartFlashing();
                    _playerFlashing = true;
                }
            }
        }

        void ModelShotMovedEventHandler(object sender, ShotMovedEventArgs e)
        {
            if (!e.Disappeared)
            {
                if (!_shots.ContainsKey(e.Shot))
                {
                    FrameworkElement shotControl = VehicleDefenceHelper.ShotControlFactory(e.Shot, Scale);
                    _shots[e.Shot] = shotControl;
                    _sprites.Add(shotControl);
                }
                else
                {
                    FrameworkElement shotControl = _shots[e.Shot];
                    VehicleDefenceHelper.MoveElementOnCanvas(shotControl, e.Shot.Location.X * Scale, e.Shot.Location.Y * Scale);
                }
            }
            else
            {
                if (_shots.ContainsKey(e.Shot))
                {
                    FrameworkElement shotControl = _shots[e.Shot];
                    _sprites.Remove(shotControl);
                    _shots.Remove(e.Shot);
                }
            }
        }

        
        private DateTime? _leftAction = null;
        private DateTime? _rightAction = null;

        internal void KeyDown(Windows.System.VirtualKey virtualKey)
        {
            if (virtualKey == Windows.System.VirtualKey.Space)
                _model.FireShot();

            if (virtualKey == Windows.System.VirtualKey.Left)
                _leftAction = DateTime.Now;

            if (virtualKey == Windows.System.VirtualKey.Right)
                _rightAction = DateTime.Now;
        }

        internal void KeyUp(Windows.System.VirtualKey virtualKey)
        {
            if (virtualKey == Windows.System.VirtualKey.Left)
                _leftAction = null;

            if (virtualKey == Windows.System.VirtualKey.Right)
                _rightAction = null;
        }

        internal void LeftGestureStarted()
        {
            _leftAction = DateTime.Now;
        }

        internal void LeftGestureCompleted()
        {
            _leftAction = null;
        }

        internal void RightGestureStarted()
        {
            _rightAction = DateTime.Now;
        }

        internal void RightGestureCompleted()
        {
            _rightAction = null;
        }

        internal void Tapped()
        {
            _model.FireShot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
