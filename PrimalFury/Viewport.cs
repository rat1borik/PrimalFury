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
        public Vector2f v1;
        public Vector2f v2;
        public Vector2f v3;
        public Vector2f v4;
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
        public List<Wall> GetViewport() {
            var drawables  = new List<Polygon>();
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

            // Wall projection in mapcoords basis

            //foreach (var w in sortedWalls) {
            //    var coords = w.GetCoords();
            //    var ht1 = 1000 / (coords.Item1 - _player.MapPosition).Dot(_player.ViewDirection);
            //    var ht2 = 1000 / (coords.Item2 - _player.MapPosition).Dot(_player.ViewDirection);
            //    var crd1 = coords.Item1.X;
            //    var crd2 = coords.Item2.X;
                
            //        //Math.Pow((coords.Item1 - _player.MapPosition).Dot(_player.ViewDirection), 2) + Math.Pow((coords.Item1 - _player.MapPosition).Length(), 2);
            //}

            // Screen-cast projection with fish-eye correction

            // Done
            return sortedWalls;
        }
    }
}
