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
                MapRect = new Vector2f(500, 300)
            };
        }

        public Player LoadPlayer() {
            return new Player(new PlayerParams {
                FieldOfView = 90
            });
        }

        public List<MapItem> LoadItems() {
            return new List<MapItem>{
                //new Wall(100, 3, 50, 40),
                new Wall(100, 3, 51.5f, 51.5f),
                new Wall(3, 3, 100, 100)
                
            };
        }

    }
    public class TestSettingsLoader : ISettingsLoader {
        public Settings Load() {
            return new Settings {
                MinimapPosition = new Vector2f(1, 1)
            };
        }
    }
}
