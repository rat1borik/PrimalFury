using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace PrimalFury {
    public class Player {
        
        PlayerParams _pParams;
        public Player(PlayerParams p) {
            _pParams = p;
        }
    }
    public struct PlayerParams {
        public int FieldOfView; // Degrees
        public Vector2i StartPosition; // Mapcoords 
    }
}
