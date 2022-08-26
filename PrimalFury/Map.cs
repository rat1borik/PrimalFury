using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalFury
{   
    // Interfaces
    public interface IMapBuilder {
        List<MapItem> LoadItems();
        Player LoadPlayer();
    }

    // Standart classes
    public class Map : IEnumerable<MapItem>
    {
        List<MapItem> _items;
        Player _player;
        IEnumerator<MapItem> IEnumerable<MapItem>.GetEnumerator() {
            return ((IEnumerable<MapItem>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public Map(IMapBuilder mb) {
            _items = mb.LoadItems();
            _player = mb.LoadPlayer();
        }
    }
    public class MapItem { }
    

    // Realisations
    public class TestMapBuilder : IMapBuilder{

        public Player LoadPlayer() {
            return null;
        }
        public List<MapItem> LoadItems() {
            return null;
        }
    }
}
