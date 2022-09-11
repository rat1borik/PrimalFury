﻿using System;
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
            MapPosition = p.StartPosition;
        }
        public Vector2f MapPosition {
            get; set;
        }
        public float FieldOfView {
            get {
                return _pParams.FieldOfView;
            }
        }
        public Vector2f ViewDirection {
            get; set;
        }
        public (Vector2f, Vector2f) ViewNormal {
            get {
                return (this.MapPosition, this.MapPosition + this.ViewDirection);
            }
        }

}
    public struct PlayerParams {
        public float FieldOfView; // Degrees
        public Vector2f StartPosition; // Mapcoords
    }
}
