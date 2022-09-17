﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using PrimalFury.Utils.MathTools;

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
                FieldOfView = 80,
                StartPosition = new Vector2f(400,50)
            });
        }

        public List<IMapItem> LoadItems() {
            var items = new List<IMapItem>();
                //new Wall(100, 3, 50, 40),
                //new Wall(100, 3, 51.5f, 51.5f),
                //new Wall(3, 3, 100, 100)
            items.AddRange(new Vector2f(100, 100).GetContainerLines(new Vector2f(100, 100)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            items.AddRange(new Vector2f(30, 30).GetContainerLines(new Vector2f(300, 150)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            items.AddRange(new Vector2f(30, 30).GetContainerLines(new Vector2f(250, 200)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            items.AddRange(new Vector2f(80, 100).GetContainerLines(new Vector2f(10, 10)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            items.AddRange(new Vector2f(20, 30).GetContainerLines(new Vector2f(400, 200)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));

            return items;
        }

        public BSPNode<Wall> LoadBSPTree() {
            return null;
        }

    }
    public class TestSettingsLoader : ISettingsLoader {
        public Settings Load() {
            return new Settings {
                MinimapPosition = new Vector2f(10, 10)
            };
        }
    }
 
    public class TestMinimap : IMinimap {

        Map _m;
        List<(Vector2f, Vector2f)> _viewlist;
        public List<(Vector2f, Vector2f)> GetLines() { return _viewlist; }
        public TestMinimap(Map m) {
            _m = m;

            _viewlist = new List<(Vector2f, Vector2f)>();

            foreach (IMapItem mI in _m.Items) {

                var clipped = _m.MapParams.MapRect.Clip(mI.GetMinimapView());
                _viewlist.Add(clipped);

            }

            _viewlist = _viewlist.Concat(
                _m.MapParams.MapRect.GetContainerLines()
                ).ToList();

        }
        public void Draw(Renderer r, Vector2f shift) {
            r.DrawLineList(_viewlist, shift);
            r.DrawCircle(2, Color.Green, _m.Player.MapPosition + shift);
            var playerView = new List<(Vector2f, Vector2f)>();
            var vn = (_m.Player.ViewNormal.Item1, _m.Player.ViewNormal.Item1 + _m.Player.ViewNormal.ToVector() * 1000);
            playerView.Add(_m.MapParams.MapRect.Clip(vn.Rotate(_m.Player.FieldOfView / 2)));
            playerView.Add(_m.MapParams.MapRect.Clip(vn.Rotate(-_m.Player.FieldOfView / 2)));
            r.DrawLineList(playerView, shift);
        }
    }
}
