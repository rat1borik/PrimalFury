using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace PrimalFury {
    internal class Renderer {

        RenderWindow _rWnd;
        public Renderer(RenderWindow r) {
            if (r == null) throw new ArgumentNullException("renderWindow is null");
            _rWnd = r;
        }
        public void DrawLineList(List<(Vector2i, Vector2i)> list) {
            foreach (var item in list) {
                _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)item.Item1, Color.White),
                    new Vertex((Vector2f)item.Item2, Color.White)
                }, PrimitiveType.Lines);
            }
        }
        public void DrawLineList(List<(Vector2i, Vector2i)> list, Vector2i position) {
            foreach (var item in list) { 
                _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)(item.Item1 + position), Color.White),
                    new Vertex((Vector2f)(item.Item2 + position), Color.White)
                }, PrimitiveType.Lines);
          
            }
        }
    }
}
