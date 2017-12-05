using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Enables fundamental Windows Runtime functionality, Managin asyncrhonous operations
using Windows.Foundation;

namespace VehicleDefence.Model
{
    class Player : MilitaryVehicle
    {
        public static readonly Size PlayerSize = new Size(25, 15);
        const double PixelsToMove = 10;

        public Player()
            : base(new Point(PlayerSize.Width, AircraftModel.PlayAreaSize.Height - AircraftModel.PlayAreaSize.Height * 3), Player.PlayerSize)
        {
            Location = new Point(Location.X, AircraftModel.PlayerAreaSize.Height - PlayerSize.Height * 3);
        }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    if(Location.X > PlayerSize.Width)
                        Location = new Point(Location.X - PixelsToMove, Location.Y);
                    break;
                default:
                    if (Location.X < AircraftModel.PlayerAreaSize.Width - PlayerSize.Width * 2)
                        Location = new Point(Location.X + PixelsToMove, Location.Y);
                    break;
            }
        }
    }
}
