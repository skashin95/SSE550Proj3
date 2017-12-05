using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleDefence.Model
{
    using Windows.Foundation;
    
    class ShotMovedEventArgs : EventArgs
    {
        public Shot Shot { get; private set; }
        public bool Disappeared  { get; private set; }
        
        pubic ShotMovedEventArgs (Shot shot, bool disappeared0
        {
            Shot = shot;
            Disappeared = disappeared;
            
        }
    }
}
