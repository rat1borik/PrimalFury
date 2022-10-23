using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using PrimalFury.Utils.MathTools;
using System.Collections;

namespace PrimalFury {
    //Test shit
    public class TestMapBuilder : IMapBuilder {
        List<IMapItem> _items;
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

        public TestMapBuilder() {
            _items = new List<IMapItem>();
            //new Wall(100, 3, 50, 40),
            //new Wall(100, 3, 51.5f, 51.5f),
            //new Wall(3, 3, 100, 100)
            _items.AddRange(new Vector2f(100, 155).GetContainerLines(new Vector2f(100, 100)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            _items.AddRange(new Vector2f(30, 30).GetContainerLines(new Vector2f(300, 150)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            _items.AddRange(new Vector2f(30, 30).GetContainerLines(new Vector2f(250, 200)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            _items.AddRange(new Vector2f(80, 100).GetContainerLines(new Vector2f(10, 10)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            _items.AddRange(new Vector2f(20, 30).GetContainerLines(new Vector2f(400, 250)).ConvertAll(new Converter<(Vector2f, Vector2f), IMapItem>((v) => new Wall(v))));
            _items.Add(new Wall(400, 100, 200, 150, Color.Red));
        }

        public List<IMapItem> LoadItems() {
            return _items;
        }

        public BSPNode<Wall> LoadBSPTree() {
            return BSPTreeGen(_items.Where(item => item is Wall).Cast<Wall>().ToList());
        }

        public BSPNode<Wall> BSPTreeGen(List<Wall> items) {
            if (items.Count == 0) return null;

            var left = new List<Wall>();
            var right = new List<Wall>();

            var divrate = new Dictionary<Wall, Dictionary<Wall, Intersection>>();

            foreach (var div in items) {
                var acc = new Dictionary<Wall, Intersection>();
                foreach (var inters in items.Where(itm => itm != div)) {
                    var it = div.GetCoords().GetIntersection(inters.GetCoords(), true);
                    if (it.HasIntersection 
                        && it.PointOfIntersection != inters.GetCoords().Item1
                        && it.PointOfIntersection != inters.GetCoords().Item2
                        && it.PointOfIntersection != div.GetCoords().Item2
                        && it.PointOfIntersection != div.GetCoords().Item1
                        ) acc.Add(inters, it);
                }
                divrate.Add(div, acc);
            }

            var sortedrate = from entry in divrate 
                             orderby entry.Value.Count ascending 
                             select entry;

            var divider = sortedrate.ToList()[0].Key;

            var interfixes = sortedrate.ToList()[0].Value;

            // divide walls

            foreach (var inters in interfixes) {

                Console.WriteLine("fixed");

                items.Add(new Wall(inters.Value.Get(), items.Find(item => item == inters.Key).V2, items.Find(item => item == inters.Key).Color));

                items.Find(item => item == inters.Key).V2 = inters.Value.Get();
            }

            foreach (var item in items.Where(item => item != divider)) {
                if (divider.GetCoords().PtFace(item.GetCoords().Item1) == Vectors.Side.Left) left.Add(item);
                else right.Add(item);
            }
            return new BSPNode<Wall>(divider, BSPTreeGen(left), BSPTreeGen(right));
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
        List<MapLine> _viewlist;
        public List<MapLine> GetLines() { return _viewlist; }
        public TestMinimap(Map m) {
            _m = m;

            _viewlist = new List<MapLine>();

            foreach (IMapItem mI in _m.Items) {

                var clipped = _m.MapParams.MapRect.Clip(mI.GetCoords());

                if (mI is Wall) _viewlist.Add(new MapLine(clipped, ((Wall)mI).Color));
                else _viewlist.Add(new MapLine(clipped, Color.White));

            }

            _viewlist = _viewlist.Concat(
                _m.MapParams.MapRect.GetContainerLines().ToList().ConvertAll(x => new MapLine(x, Color.White))
                ).ToList();

        }
        public void Draw(Renderer r, Vector2f shift) {
            r.DrawLineList(_viewlist, shift);
            r.DrawCircle(2, Color.Green, _m.Player.MapPosition + shift);
            var playerView = new List<MapLine>();
            var vn = (_m.Player.ViewNormal.Item1, _m.Player.ViewNormal.Item1 + _m.Player.ViewNormal.ToVector() * 1000);
            playerView.Add(new MapLine(_m.MapParams.MapRect.Clip(vn.Rotate(_m.Player.FieldOfView / 2)), Color.White));
            playerView.Add(new MapLine(_m.MapParams.MapRect.Clip(vn.Rotate(-_m.Player.FieldOfView / 2)), Color.White));
            playerView.Add(new MapLine(_m.MapParams.MapRect.Clip(vn.Rotate(-90)), Color.Red));
            playerView.Add(new MapLine(_m.MapParams.MapRect.Clip(vn.Rotate(90)), Color.Red));
            r.DrawLineList(playerView, shift);
        }
    }
}
