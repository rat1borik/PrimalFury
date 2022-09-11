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
                Position = pos - new Vector2f(radius,radius),
            });
        }
    }
}
