using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using PrimalFury.Utils.MathTools;

namespace PrimalFury
{   
    // Interfaces
    public interface IMapBuilder {
        List<IMapItem> LoadItems();
        Player LoadPlayer();
        MapParams LoadMapParams();
        BSPNode<Wall> LoadBSPTree();
    }

    public interface IMapItem {
        (Vector2f, Vector2f) GetMinimapView();

        
    }
    public interface IMinimap {
        List<(Vector2f, Vector2f)> GetLines();
        void Draw(Renderer r, Vector2f shift);
    }

    // Standart classes
    public class Map
    {
        List<IMapItem> _items;
        BSPNode<Wall> _bspTree;
        Player _player;
        MapParams _mParams;

        public Map(IMapBuilder mb) {
            _mParams = mb.LoadMapParams();
            _items = mb.LoadItems();
            _bspTree = mb.LoadBSPTree();
            _player = mb.LoadPlayer();
            if (!_mParams.MapRect.Contains(_player.MapPosition)) {
                _player.MapPosition = new Vector2f();
            }
        }
        public MapParams MapParams {
            get { return _mParams; }
        }

        public Player Player {
            get { return _player; }
        }

        public List<IMapItem> Items { 
            get { return _items; } 
        }

    }

    // Structs
   
    public struct MapParams {
        public Vector2f MapRect;
    }

    // Realisations
    public class Wall : IMapItem {

        Vector2f _w1;
        Vector2f _w2;
        public (Vector2f, Vector2f) GetMinimapView() {
            return (_w1, _w2);
        }
        public Wall(float x1, float y1, float x2, float y2) {
            _w1 = new Vector2f(x1, y1);
            _w2 = new Vector2f(x2, y2);
            if (_w1 == _w2) {
                throw new ArgumentException("Стена не может быть точкой");
            }
        }
        public Wall(Vector2f wCoord1, Vector2f wCoord2) {
            _w1 = wCoord1;
            _w2 = wCoord2;
            if (_w1 == _w2) {
                throw new ArgumentException("Стена не может быть точкой");
            }
        }
        public Wall((Vector2f , Vector2f ) w) {
            _w1 = w.Item1;
            _w2 = w.Item2;
            if (_w1 == _w2) {
                throw new ArgumentException("Стена не может быть точкой");
            }
        }

    }
   
}
