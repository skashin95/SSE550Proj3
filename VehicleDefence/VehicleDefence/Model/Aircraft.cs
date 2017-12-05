using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleDefence.Model
{
    using Windows.Foundation;
    
    class Aircraft : MilitaryVehicle
    {
        public static readonly Size AircraftSize = new Size(15, 15);
        public const int HorizontalInterval = 5;
        public const int VerticalInterval = 15;
        
        public AircraftType AircraftType { get; private set; }
        public int Score { get; private set; }
        
        public Aircraft (AircraftType aircraftType, Point location, int score) : base(location, Aircraft.AircraftSize)
        {
            this.AircraftType = aircraftType;
            this.Score = score;
        }
        
        public override Move (Direction aircraftDirection)
        {
            switch (aircraftDirection)
                {
                    case Direction.Right:
                        Location = new Point(Location.X + HorizontalInterval, Location.Y);
                        break;
                        
                    case Direction.Left:
                        Location = new Point(Location.X - HorizontalInterval, Location.Y);
                        break;                        
                        
                    default:
                        Location = new Point(Location.X, Location.Y + VerticalInterval);
                        break;
                }       
        }
        
    }
}
