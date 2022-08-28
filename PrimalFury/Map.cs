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
        List<MapItem> LoadItems();
        Player LoadPlayer();
        MapParams LoadMapParams();
    }

    public interface MapItem {
        (Vector2f, Vector2f) GetMapView();

        
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
        public List<(Vector2f, Vector2f)> GetMapView() {

            var viewList = new List<(Vector2f, Vector2f)>();

            foreach (MapItem mI in _items) {

                var clipped = _mParams.MapRect.Clip(mI.GetMapView());
                viewList.Add(clipped);

            }
            viewList = viewList.Concat(
                _mParams.MapRect.GetContainerLines()
                ).ToList();
            return viewList;
        }
    }

    public struct MapParams {
        public Vector2f MapRect;
    }

    // Realisations
    public class Wall : MapItem {

        Vector2f _w1;
        Vector2f _w2;
        public (Vector2f, Vector2f) GetMapView() {
            return (_w1, _w2);
        }
        public Wall(float x1, float y1, float x2, float y2) {
            _w1 = new Vector2f(x1, y1);
            _w2 = new Vector2f(x2, y2);
            if (_w1 == _w2) {
                throw new ArgumentException("Стена не может быть точкой");
            }
        }

    }
   
}
