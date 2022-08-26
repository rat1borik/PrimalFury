using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace PrimalFury {
    //Test shit
    public class TestMapBuilder : IMapBuilder {
        public MapParams LoadMapParams() {
            return new MapParams {
                MapRect = new Vector2i(100, 100)
            };
        }

        public Player LoadPlayer() {
            return new Player(new PlayerParams {
                FieldOfView = 90
            });
        }

        public List<MapItem> LoadItems() {
            return new List<MapItem>{
                new Wall(1, 1, 103, 103),
                new Wall(103, 1, 1, 103)
            };
        }
    }
}
