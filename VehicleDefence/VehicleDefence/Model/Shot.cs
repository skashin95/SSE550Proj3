using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Enables fundamental Windows Runtime functionality, Managin asyncrhonous operations
using Windows.Foundation;

namespace VehicleDefence.Model
{
    class Shot
    {
        public const double ShotPixelsPerSecond = 100;
        public Point Location { get; private set; }
        public Size ShotSize = new Size(2, 10);
        public Direction Direction { get; private set; }
        private DateTime _lastMoved;

        public Shot(Point location, Direction direction)
        {
            Location = location;
            Direction = direction;
            _lastMoved = DateTime.Now;
        }

        public void Move()
        {
            TimeSpan timeSinceLastMoved = DateTime.Now - _lastMoved;
            //TODO Figure out why we divide by 1000
            double distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
            if (Direction == Direction.Up) distance *= -1;
            Location = new Point(Location.X, Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
    }
}
