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
        
        private Direction _invaderDirection = Direction.Left;
        private bool _justMovedDown = false;
        
        private DateTime _lastUpdated = DateTime.MinValue;
        
        public event EventHandler VehicleChanged;        
        public event EventHandler ShotMoved;
        
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
            
            // Clear collections
            _aircrafts.Clear();     
            _playerShots.Clear();
            _aircraftShots.Clear();
            

            
            // Add from page 815
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
                onShotMoved(shot, false);
            }
        }
        
        public void MovePlayer()
        {
            // Write after Player class is done
            if (_playerDied.HasValue) return;
            _player.Move(direction);
            // TODO need to check if this needs to be vehical, aircraft, or military vehicle
            OnVehicleChanged(_player, false);
        }
        
        public void Update()
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
                    OnAircraftChanged(_player, false);
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
                            
                    }
                }
        }
        
        public void CheckForPlayerCollisions()
        {
            
        }
        
        
        public void CheckForAircraftCollisions()
        {
            
        }
        
        public void MoveAircrafts()
        {
            
        }
        
        public void ReturnFire()
        {
            
        }
        
        
        public void UpdateAllMilitaryVehicles()
        {
            
        }
    }
}
