﻿using System;
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
        public void DrawPoint(MapLine item) {
            _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)item.Points[0], item.Color),
                }, PrimitiveType.Points);
        }

        public void DrawPoint(MapLine item, Vector2f position) {
            _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)item.Points[0] + position, item.Color),
                }, PrimitiveType.Points);
        }

        public void DrawLine(MapLine item) {
                _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)item.Points[0], item.Color),
                    new Vertex((Vector2f)item.Points[1], item.Color)
                }, PrimitiveType.Lines);
        }

        public void DrawLine(MapLine item, Vector2f position) {
            _rWnd.Draw(new Vertex[] {
                    new Vertex((Vector2f)(item.Points[0] + position), item.Color),
                    new Vertex((Vector2f)(item.Points[1] + position), item.Color)
                }, PrimitiveType.Lines);
        }
        public void DrawLineList(List<MapLine> list) {
            foreach (var item in list) {
                DrawLine(item);
            }
        }
        public void DrawLineList(List<MapLine> list, Vector2f position) {
            foreach (var item in list) {
                DrawLine(item, position);

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

        public void DrawPoly(Polygon p, Vector2f position = new Vector2f(), Vector2f shift = new Vector2f(), bool centerScreen = false) {
            var poly = new ConvexShape(4) {
                Position = position,
                FillColor = p.Color
            };
            for (int i = 0; i < p.Points.Length; i++) poly.SetPoint((uint)i, new Vector2f(p.Points[i].X + (centerScreen ? RWndCenter.X : 0) + shift.X, (centerScreen ? RWndCenter.Y : 0) + (centerScreen ? -1 :1) * p.Points[i].Y + shift.Y));


            _rWnd.Draw(poly);
        }
    }
}
