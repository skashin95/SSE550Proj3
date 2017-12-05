using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }
        
        public void MovePlayer()
        {
            // Write after Player class is done
        }
        
        public void Update()
        {
            
        }
        
        public void MoveShots()
        {
            
        }
        
        public void NextWave()
        {
            
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
