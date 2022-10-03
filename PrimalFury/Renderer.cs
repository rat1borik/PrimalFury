using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace PrimalFury {
    public class Renderer {

        float circleDetalisationFactor = 3;
        RenderWindow _rWnd;
        public RenderWindow RWindow { get { return _rWnd; } }

        public Vector2f RWndCenter {
            get {
                return (Vector2f)_rWnd.Size / 2;
            }
        }
        public Renderer(RenderWindow r) {
            if (r == null) throw new ArgumentNullException("renderWindow is null");
            _rWnd = r;
        }
        public void DrawLineList(List<(Vector2f, Vector2f)> list) {
            foreach (var item in list) {
                _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)item.Item1, Color.White),
                    new Vertex((Vector2f)item.Item2, Color.White)
                }, PrimitiveType.Lines);
            }
        }
        public void DrawLineList(List<(Vector2f, Vector2f)> list, Vector2f position) {
            foreach (var item in list) {
                _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)(item.Item1 + position), Color.White),
                    new Vertex((Vector2f)(item.Item2 + position), Color.White)
                }, PrimitiveType.Lines);

            }
        }
        public void Draw(List<Drawable> list, Vector2f position) {
            foreach (var item in list) {
                item.Draw(_rWnd, RenderStates.Default);
            }
        }

        public void DrawCircle(float radius, Color fillColor, Vector2f pos) {
            _rWnd.Draw(new CircleShape(radius, (uint)Math.Round(circleDetalisationFactor * radius)) {
                FillColor = fillColor,
                Position = pos - new Vector2f(radius, radius),
            });
        }

        public void DrawPoly(List<Vector2f> vecs, Color fillColor, Vector2f position = new Vector2f()) {
            var poly = new ConvexShape(4) {
                Position = position,
                FillColor = fillColor
            };
            for (int i = 0; i < vecs.Count; i++) poly.SetPoint((uint)i, vecs[i]);


            _rWnd.Draw(poly);
        }

        public void DrawPolyCC(Polygon p, Vector2f position = new Vector2f()) {
            var poly = new ConvexShape(4) {
                Position = position,
                FillColor = p.Color
            };
            for (int i = 0; i < p.Points.Length; i++) poly.SetPoint((uint)i, new Vector2f(p.Points[i].X + RWndCenter.X, RWndCenter.Y - p.Points[i].Y));


            _rWnd.Draw(poly);
        }
    }
}
