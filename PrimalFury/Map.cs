using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using PrimalFury.Utils;

namespace PrimalFury
{   
    // Interfaces
    public interface IMapBuilder {
        List<MapItem> LoadItems();
        Player LoadPlayer();
        MapParams LoadMapParams();
    }

    public interface MapItem {
        (Vector2i, Vector2i) GetMapView();

        
    }

    // Standart classes
    public class Map : IEnumerable<MapItem>
    {
        List<MapItem> _items;
        Player _player;
        MapParams _mParams;

        IEnumerator<MapItem> IEnumerable<MapItem>.GetEnumerator() {
            return ((IEnumerable<MapItem>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public Map(IMapBuilder mb) {
            _mParams = mb.LoadMapParams();
            _items = mb.LoadItems();
            _player = mb.LoadPlayer();
        }
        public MapParams MapParams {
            get { return _mParams; }
        }
        public List<(Vector2i, Vector2i)> GetMapView() {

            var viewList = new List<(Vector2i, Vector2i)>();

            foreach (MapItem mI in _items) {

                var clipped = Vectors.Clip(_mParams.MapRect, mI.GetMapView());
                viewList.Add(clipped);

            }
            return viewList;
        }
    }

    public struct MapParams {
        public Vector2i MapRect;
    }

    // Realisations
    public class Wall : MapItem {

        Vector2i _w1;
        Vector2i _w2;
        public (Vector2i, Vector2i) GetMapView() {
            return (_w1, _w2);
        }
        public Wall(int x1, int y1, int x2, int y2) {
            _w1 = new Vector2i(x1, y1);
            _w2 = new Vector2i(x2, y2);
            if (_w1 == _w2) {
                throw new ArgumentException("Стена не может быть точкой");
            }
        }

    }
   
}
