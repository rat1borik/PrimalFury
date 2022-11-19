using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

using PrimalFury.Utils.MathTools;

namespace PrimalFury {
    public class Player {
        
        PlayerParams _pParams;
        Vector2f _vDir;
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
            get { return _vDir; }
            set { _vDir = value; }
        }
        public (Vector2f, Vector2f) ViewNormal {
            get {
                return (this.MapPosition, this.MapPosition + this.ViewDirection);
            }
        }

        public List<(Vector2f,Vector2f)> ViewRange {
            get {
                return new List<(Vector2f, Vector2f)> {
                    this.ViewNormal.Rotate(this.FieldOfView / 2),
                    this.ViewNormal.Rotate(-this.FieldOfView / 2)
                };
            }
        }
        public (Vector2f, Vector2f) ViewRangeRight {
            get {
                return this.ViewNormal.Rotate(-this.FieldOfView / 2);
            }
        }

        public (Vector2f, Vector2f) ViewRangeLeft {
            get {
                return this.ViewNormal.Rotate(this.FieldOfView / 2);
            }
        }

        public float Height {
            get {
                return 30;
            }
        }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }

        public float VelocityLimit {
            get {
                return 1;
            }
        }

        public float AccelerationRate {
            get {
                return 0.1f;
                //return 0.05f;
            }
        }
    }
    public struct PlayerParams {
        public float FieldOfView; // Degrees
        public Vector2f StartPosition; // Mapcoords
    }
}
