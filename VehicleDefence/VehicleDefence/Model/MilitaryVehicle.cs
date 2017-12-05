using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Enables fundamental Windows Runtime functionality, Managin asyncrhonous operations
using Windows.Foundation;

namespace VehicleDefence.Model
{
    class MilitaryVehicle
    {
        public Point Location { get; protected set;  }
        public Size Size { get; private set; }
        public Rect Area { get { return new Rect(Location, Size); } }
        public MilitaryVehicle(Point location, Size size)
        {
            Location = location;
            Size = size;
        }
    }
}
