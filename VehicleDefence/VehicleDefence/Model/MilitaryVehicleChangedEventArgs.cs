using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace VehicleDefence.Model
{
    class MilitaryVehicleChangedEventArgs : EventArgs
    {
        public MilitaryVehicle MilitaryVehcileUpdated { get; private set; }
        public bool Killed { get; private set; }

        public MilitaryVehicleChangedEventArgs(MilitaryVehicle militaryVehicleUpdated, bool killed)
        {
            MilitaryVehcileUpdated = militaryVehicleUpdated;
            Killed = killed;
        }
    }
}
