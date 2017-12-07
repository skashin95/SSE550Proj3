using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace VehicleDefence.Model
{
    class AircraftModel
    {
        public readonly static Size PlayAreaSize = new Size(400, 300);
        public const int MaximumPlayerShots = 3;
        
        private readonly Random _random = new Random();
        


        public int Score { get; private set; }
        public int Wave { get; private set; }
        public int Lives { get; private set; }
        
        public bool GameOver { get; private set; }
        
        private DateTime? _playerDied = null;
        public bool PlayerDying { get { return _playerDied.HasValue; } }
        
        private Player _player;
        
        private readonly List<Aircraft> _aircrafts = new List<Aircraft>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly List<Shot> _aircraftShots = new List<Shot>();
        
        private Direction _aircraftDirection = Direction.Left;
        private bool _justMovedDown = false;
        
        private DateTime _lastUpdated = DateTime.MinValue;
        
                
        public AircraftModel()
        {
            EndGame();
        }
        
        public void EndGame()
        {
            GameOver = true;
        }
        
        public void StartGame()
        {
            GameOver = false;
         

            foreach (Aircraft aircraft in _aircrafts)
                OnMilitaryVehicleChanged(aircraft, true);
            _aircrafts.Clear();

            foreach (Shot shot in _playerShots)
                OnShotMoved(shot, true);
            _playerShots.Clear();
            _aircraftShots.Clear();

            
            _player = new Player();
            OnMilitaryVehicleChanged(_player, false);

            Lives = 2;
            Wave = 0;

            NextWave();
                        
        }
        
        public void FireShot()
        {
            if (GameOver) return;

            var playersShots =
                from Shot shot in _playerShots
                where shot.Direction == Direction.Up
                select shot;

            if (_playerShots.Count() < MaximumPlayerShots)
            {
                Point shotLocation = new Point(_player.Location.X + _player.Area.Width / 2, _player.Location.Y);
                Shot shot = new Shot(shotLocation, Direction.Up);
                _playerShots.Add(shot);
                OnShotMoved(shot, false);
            }
        }
        
        public void MovePlayer(Direction direction)
        {
            // Write after Player class is done
            if (_playerDied.HasValue) return;
            _player.Move(direction);
            // TODO need to check if this needs to be vehical, aircraft, or military vehicle
            OnMilitaryVehicleChanged(_player, false);
        }
        
        public void Update(bool paused)
        {
            if(!paused)
            {
                if (_aircrafts.Count() == 0) NextWave();
                if (!_playerDied.HasValue)
                {
                    MoveAircrafts();
                    MoveShots();
                    ReturnFire();
                    CheckForAircraftCollisions();
                    CheckForPlayerCollisions();
                }
                else if (_playerDied.HasValue && (DateTime.Now - _playerDied > TimeSpan.FromSeconds(2.5)))
                {
                    _playerDied = null;
                    // TODO need to check if this needs to be vehical, aircraft, or military vehicle
                    OnMilitaryVehicleChanged(_player, false);
                }
            }
        }
        
        public void MoveShots()
        {
            foreach (Shot shot in _playerShots)
            {
                shot.Move();
                OnShotMoved(shot, false);
            }

            var outOfBounds =
                from shot in _playerShots
                where (shot.Location.Y < 10 || shot.Location.Y > PlayAreaSize.Height - 10)
                select shot;

            foreach (Shot shot in outOfBounds.ToList())
            {
                _playerShots.Remove(shot);
                OnShotMoved(shot, true);
            }
        }
        
        public void NextWave()
        {
            Wave++;
            _aircrafts.Clear();
            for(int row = 0; row <= 5; row++)
                for (int column = 0; column < 11; column++)
                {
                    Point location = new Point(column * Aircraft.AircraftSize.Width * 1.4, row * Aircraft.AircraftSize.Height * 1.4);
                    Aircraft aircraft;
                    switch (row)
                    {
                        case 0:
                            aircraft = new Aircraft(AircraftType.Blimp, location, 50);
                            break;
                        case 1:
                             aircraft = new Aircraft(AircraftType.CargoPlane, location, 40);
                            break;
                        case 2: 
                             aircraft = new Aircraft(AircraftType.Helicopter, location, 30);
                            break;
                        default:
                             aircraft = new Aircraft(AircraftType.FighterJet, location, 10);
                            break;                  
                     }
                    _aircrafts.Add(aircraft);
                    OnMilitaryVehicleChanged(aircraft, false);
                }
        }
        
        public void CheckForPlayerCollisions()
        {
            bool removeAllShots = false;
            var result = from aircraft in _aircrafts where aircraft.Area.Bottom > _player.Area.Top + _player.Size.Height select aircraft;
            if (result.Count() > 0)
                EndGame();

            var shotsHit =
                from shot in _playerShots
                where shot.Direction == Direction.Down && _player.Area.Contains(shot.Location)
                select shot;
            if (shotsHit.Count() > 0)
            {
                Lives--;
                if (Lives >= 0)
                {
                    _playerDied = DateTime.Now;
                    OnMilitaryVehicleChanged(_player, true);
                    removeAllShots = true;
                }
                else
                    EndGame();
            }
            if (removeAllShots)
                foreach (Shot shot in _playerShots.ToList())
                {
                    _playerShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
        }


        public void CheckForAircraftCollisions()
        {

            List<Shot> shotsHit = new List<Shot>();
            List<Aircraft> aircraftsKilled = new List<Aircraft>();
            foreach (Shot shot in _playerShots)
            {
                var result = from aircraft in _aircrafts
                             where aircraft.Area.Contains(shot.Location) == true && shot.Direction == Direction.Up
                             select new { AircraftKilled = aircraft, ShotHit = shot };
                if (result.Count() > 0)
                {
                    foreach (var o in result)
                    {
                        shotsHit.Add(o.ShotHit);
                        aircraftsKilled.Add(o.AircraftKilled);
                    }
                }
            }

            foreach (Aircraft aircraft in aircraftsKilled)
            {
                Score += aircraft.Score;
                _aircrafts.Remove(aircraft);
                OnMilitaryVehicleChanged(aircraft, true);
            }

            foreach (Shot shot in shotsHit)
            {
                _playerShots.Remove(shot);
                OnShotMoved(shot, true);
            }
        }
            
        
        public void MoveAircrafts()
        {
            double millisecondsBetweenMovements = Math.Min(10 - Wave, 1) * (2 * _aircrafts.Count());
            if (DateTime.Now - _lastUpdated > TimeSpan.FromMilliseconds(millisecondsBetweenMovements))
            {
                _lastUpdated = DateTime.Now;

                var aircraftsTouchingLeftBoundary = from aircraft in _aircrafts where aircraft.Area.Left < Aircraft.HorizontalInterval select aircraft;
                var aircraftsTouchingRightBoundary = from aircraft in _aircrafts where aircraft.Area.Right > PlayAreaSize.Width - (Aircraft.HorizontalInterval * 2) select aircraft;

                if (!_justMovedDown)
                {
                    if (aircraftsTouchingLeftBoundary.Count() > 0)
                    {
                        foreach (Aircraft aircraft in _aircrafts)
                        {
                            aircraft.Move(Direction.Down);
                            OnMilitaryVehicleChanged(aircraft, false);
                        }
                        _aircraftDirection = Direction.Right;
                    }
                    else if (aircraftsTouchingRightBoundary.Count() > 0)
                    {
                        foreach (Aircraft aircraft in _aircrafts)
                        {
                            aircraft.Move(Direction.Down);
                            OnMilitaryVehicleChanged(aircraft, false);
                        }
                        _aircraftDirection = Direction.Left;
                    }
                    _justMovedDown = true;
                }
                else
                {
                    _justMovedDown = false;
                    foreach (Aircraft aircraft in _aircrafts)
                    {
                        aircraft.Move(_aircraftDirection);
                        OnMilitaryVehicleChanged(aircraft, false);
                    }
                }
            }
        }
        
        public void ReturnFire()
        {
            if (_aircrafts.Count() == 0) return;

            var aircraftsShots =
                from Shot shot in _playerShots
                where shot.Direction == Direction.Down
                select shot;

            if (aircraftsShots.Count() > Wave + 1 || _random.Next(10) < 10 - Wave)
                return;

            var result =
                from aircraft in _aircrafts
                group aircraft by aircraft.Area.X into invaderGroup
                orderby invaderGroup.Key descending
                select invaderGroup;

            var randomGroup = result.ElementAt(_random.Next(result.ToList().Count()));
            var bottomAircraft = randomGroup.Last();

            Point shotLocation = new Point(bottomAircraft.Area.X + bottomAircraft.Area.Width / 2, bottomAircraft.Area.Bottom + 2);
            Shot aircraftShot = new Shot(shotLocation, Direction.Down);
            _playerShots.Add(aircraftShot);
            OnShotMoved(aircraftShot, false);
        }
        
        
        public void UpdateAllMilitaryVehicles()
        {
            foreach (Shot shot in _playerShots)
                OnShotMoved(shot, false);
            foreach (Aircraft vehicle in _aircrafts)
                OnMilitaryVehicleChanged(vehicle, false);
            OnMilitaryVehicleChanged(_player, false);
            
        }

        public event EventHandler<MilitaryVehicleChangedEventArgs> MilitaryVehicleChanged;

        protected void OnMilitaryVehicleChanged(MilitaryVehicle vehicleUpdated, bool killed)
        {
            EventHandler<MilitaryVehicleChangedEventArgs> vehicleChanged = MilitaryVehicleChanged;
            if (vehicleChanged != null)
                vehicleChanged(this, new MilitaryVehicleChangedEventArgs(vehicleUpdated, killed));
        }

        public event EventHandler<ShotMovedEventArgs> ShotMoved;

        protected void OnShotMoved(Shot shot, bool disappeared)
        {
            EventHandler<ShotMovedEventArgs> shotMoved = ShotMoved;
            if (shotMoved != null)
                shotMoved(this, new ShotMovedEventArgs(shot, disappeared));
        }

        
    }
}
