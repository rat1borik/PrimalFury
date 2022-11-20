using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

using PrimalFury.Utils.MathTools;

namespace PrimalFury {
    public struct Polygon {
        private Vector2f _v1;
        private Vector2f _v2;
        private Vector2f _v3;
        private Vector2f _v4;
        private Color _c;

        public Polygon(List<Vector2f> vecs, Color c) {
            if (vecs.Count != 4) throw new ArgumentException("Incorrect input");
            _v1 = vecs[0];
            _v2 = vecs[1];
            _v3 = vecs[2];
            _v4 = vecs[3];
            _c = c;
        }

        public Vector2f[] Points {
            get{
                return new Vector2f[] {_v1, _v2, _v3, _v4};
            }
        }
        public Color Color {
            get {
                return _c;
            }
        }
    }

    // This class combines all stuff need to convert map items in screen-basis polygons
    // sorted in correct draw order using Player, Map, RenderWindow objects
    public class Viewport {

        Vector2f _vpRect;
        Map _m;
        Player _player;

        public Viewport(Vector2f vpRect, Map m) {
            _vpRect = vpRect;
            _m = m;
            _player = m.Player;
        }
        public List<Polygon> GetViewport() {

            var drawables = new List<Polygon>();
            var sortedWalls = new List<Wall>();

            // Traverse BSP tree using player position
            foreach (var item in _m.BSPTree.Traverse((rt) => {
                if (rt?.GetCoords().PtFace(_player.MapPosition) == Vectors.Side.Left) return true;
                return false;
            })) {
                sortedWalls.Add(item);
            }

            // Sort in back order to perform back to front draw
            sortedWalls.Reverse();

            // Cutting all invisible or
            // TODO: partially visible walls
            sortedWalls = sortedWalls.Where(wall => _player.ViewNormal.Rotate(-90).CutFace(wall.GetCoords()) != Vectors.Side.Right
                                    && _player.ViewRange[0].CutFace(wall.GetCoords()) != Vectors.Side.Left
                                    && _player.ViewRange[1].CutFace(wall.GetCoords()) != Vectors.Side.Right).ToList();

            // Define player-view vector
            //Console.WriteLine("===================");
            //sortedWalls.ForEach(w => Console.WriteLine(w.ToString()));
            var playerPos = new Vector3f(_player.MapPosition.X, _player.MapPosition.Y, _player.Height);
            var projSurfDistance = (float)(_vpRect.X / 2 / Math.Tan((_player.FieldOfView / 2).ToRadians()));
            //var projSurfVec = (_player.ViewNormal.Item2 * projSurfDistance,
            //    (_player.ViewNormal.Item2 * projSurfDistance, _player.ViewNormal.Item1).ToVector().Rotate(90));
            var playerSurf = _player.ViewNormal.Rotate(-90);
            var polys = new List<Polygon>();

            // Wall projection in mapcoords basis

            foreach (var w in sortedWalls) {
                //var coords = w.GetCoords();
                //var ht1 = 1000 / (coords.Item1 - _player.MapPosition).Dot(_player.ViewDirection);
                //var ht2 = 1000 / (coords.Item2 - _player.MapPosition).Dot(_player.ViewDirection);
                //var crd1 = coords.Item1.X;
                //var crd2 = coords.Item2.X

                //Math.Pow((coords.Item1 - _player.MapPosition).Dot(_player.ViewDirection), 2) + Math.Pow((coords.Item1 - _player.MapPosition).Length(), 2);
                var newBasis = (_player.ViewDirection.Rotate(-90), _player.ViewDirection);
                var newwl = (w.V1, w.V2);

                //clipping
                //var i = _player.ViewRangeRight.GetIntersection(newwl, true);

                //if (i.HasIntersection) {
                //    if(_player.ViewRangeRight.PtFace(newwl.Item1) == Vectors.Side.Right) {
                //        newwl.Item1 = i.PointOfIntersection;
                //    } else {
                //        newwl.Item2 = i.PointOfIntersection;
                //    }
                //}

                //i = _player.ViewRangeLeft.GetIntersection(newwl, true);

                //if (i.HasIntersection) {
                //    if (_player.ViewRangeLeft.PtFace(newwl.Item1) == Vectors.Side.Left) {
                //        newwl.Item1 = i.PointOfIntersection;
                //    } else {
                //        newwl.Item2 = i.PointOfIntersection;
                //    }
                //}
                var frontside = (playerSurf.Item1 + _player.ViewDirection, playerSurf.Item2 + _player.ViewDirection);

                var i = frontside.GetIntersection(newwl, true);
                    
                if (i.HasIntersection){
                    if(frontside.PtFace(newwl.V1) == Vectors.Side.Right && i.PointOfIntersection != newwl.V2) {
                        newwl.V1 = i.PointOfIntersection;
                    } else if (i.PointOfIntersection != newwl.V1) {
                        newwl.V2 = i.PointOfIntersection;
                    }
                }

                //Console.WriteLine(newwl.ToString());
                // init new wall
                var wl = new Wall(newwl);

                

                


                var wcoords = new List<Vector3f>{
                     (new Vector3f(wl.V1.X, wl.V1.Y, 0) - playerPos).GetVecByBasis(newBasis),
                     (new Vector3f(wl.V1.X, wl.V1.Y, wl.Height) - playerPos).GetVecByBasis(newBasis),
                     (new Vector3f(wl.V2.X, wl.V2.Y, wl.Height) - playerPos).GetVecByBasis(newBasis),
                     (new Vector3f(wl.V2.X, wl.V2.Y, 0) - playerPos).GetVecByBasis(newBasis),                     
                     
                };

                //polys.Add(new Polygon(wcoords.ConvertAll(el => {
                //    var surf = new Vector2f(el.X, el.Y);
                //    return new Vector2f(
                //        projSurfDistance / surf.Length() * Math.Sign(_player.ViewDirection.Cross(surf)) * (float)Math.Sqrt(1 - Math.Pow(_player.ViewDirection.Cos2V(surf), 2)) * surf.Length(),
                //        projSurfDistance / (float)Math.Sqrt(Math.Pow(_player.ViewDirection.Cos2V(surf) * surf.Length(), 2) + el.Z * el.Z) * el.Z
                //    );
                //}),w.Color));

                polys.Add(new Polygon(wcoords.ConvertAll(el => {
                    return new Vector2f(
                        el.Y == 0 ? Math.Sign(-el.X) * _vpRect.X / 2 : - el.X * projSurfDistance / el.Y,
                        el.Y == 0 ? Math.Sign(el.Z) * _vpRect.Y / 2 : el.Z * projSurfDistance / el.Y
                    );
                }), w.Color));


            }

            // Done
            return polys;
        }
    }
}
