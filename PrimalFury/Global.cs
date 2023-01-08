using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using PrimalFury.Utils.MathTools;
using System.Collections.Concurrent;

namespace PrimalFury {
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }
    }
    static class Global {

        public const float COLLIDE_DISTANCE = 2;

        // Map and builder
        static Map testMap;
        static IMinimap testMiniMap;
        static IMapBuilder testBuilder;
        static Viewport vp;
        static Renderer testRenderer;

        // Settings

        static Settings settings;

        // Mouse
        static uint MouseX = 0, MouseY = 0;
        static int XDiff = 0, YDiff = 0;

        // Window
        static RenderWindow window;
        static uint WindowWidth, WindowHeight;

        // Debug info
        static string PreviousKey = "";
        static int PreviousKeyCount = 0;
        static string MouseKeyPressed = "";
        static string debugText = "";

        //HUD Parameters
        static Sprite crosshair;
        static uint crosshairRectX = 25, crosshairRectY = 25;
        static string fileCrosshair = "crosshair.png";


        //Main Animations
        static Animation<(Vector2f, float)> camShake;

        static void Main(string[] args) {
            PrepareWindow();

            settings = new TestSettingsLoader().Load();

            // Load all resources temp shit
            Vector2u crosshairRect = new Vector2u(crosshairRectX, crosshairRectY);
            Image crosshairImg = new Image(fileCrosshair);
            var debugFont = new Font("font.otf");

            crosshair = new Sprite() {
                Texture = new Texture(fileCrosshair),
                Position = new Vector2f((int)WindowWidth / 2 - crosshairRect.X / 2, (int)WindowHeight / 2 - crosshairRect.Y / 2),
                Scale = new Vector2f((float)crosshairRect.X / (float)crosshairImg.Size.X, (float)crosshairRect.Y / (float)crosshairImg.Size.Y)
            };

            var debugInfo = new Text() {
                Font = debugFont,
                CharacterSize = 14,
                FillColor = Color.Green,
                Position = new Vector2f(WindowWidth - 200, 0)
            };


            PrepareMap();


            // Start the game loop
            Mouse.SetPosition(new Vector2i((int)WindowWidth / 2, (int)WindowHeight / 2), window);

            var shakeShift = new Vector2f(0, 0);
            FixedSizedQueue<Vector2f> shakeTrace = new FixedSizedQueue<Vector2f>(256);
            camShake = new Animation<(Vector2f, float)>((x)=>(new Vector2f((0.5f - x),(float)Math.Pow(0.5f - x, 2))*100, x), 1000, true, true, 0.25f);
            while (window.IsOpen) {
                //debug
                debugText = String.Format("X: {0}, Y: {1}\nKey pressed: {2} : {3} times\nMouse key pressed: {4}\nVelocityX: {5}\nVelocityY: {6}\n", MouseX, MouseY, PreviousKey, PreviousKeyCount, MouseKeyPressed, testMap.Player.VelocityX, testMap.Player.VelocityY);


                debugInfo.DisplayedString = debugText;
                debugInfo.Position = new Vector2f(WindowWidth - (debugText.Split('\n').OrderByDescending(s => s.Length).ToArray()[0].Length * (debugInfo.CharacterSize / 2)) - 20, 0);

                if (window.HasFocus()) {
                    ProvideInput();
                }

                if (testMap.Player.VelocityX != 0 || testMap.Player.VelocityY != 0) {
                    //camShake.Start();
                    if (testMap.Player.IsRunning) {
                        camShake.Duration = 500;
                    } else {
                        camShake.Duration = 1000;
                    }
                } else camShake.Freeze();

                if (camShake.State == AnimationState.Running) {
                    var t = camShake.Get();
                    shakeShift = t.Item1;
                    shakeTrace.Enqueue(shakeShift);
                    //Console.WriteLine(t.Item2);
                }

                window.Clear();
                window.DispatchEvents();

                // Calculate screen-based polys
                var vecs = vp.GetViewport();

                foreach (var v in vecs) 
                    testRenderer.DrawPoly(v, default, shakeShift, true);

                RenderHUD();

                foreach (var v in shakeTrace)
                    testRenderer.DrawPoint(new MapLine((v, v), Color.White), new Vector2f(WindowWidth / 2, WindowHeight / 2));
                testRenderer.DrawLine(new MapLine((new Vector2f(0, 0), shakeShift), Color.Magenta), new Vector2f(WindowWidth / 2, WindowHeight / 2));

                // Draw
#if DEBUG
                window.Draw(debugInfo);
#endif

                // Finally, display the rendered frame on screen
                window.Display();
            }


        }
        public static void ProvideInput() {
            testMap.Player.IsRunning = Keyboard.IsKeyPressed(Keyboard.Key.LShift);

            testMap.Player.VelocityY = GetVelocityChanges((Keyboard.Key.W, Keyboard.Key.S), testMap.Player.VelocityY, testMap.Player.VelocityLimit, testMap.Player.AccelerationRate);
            testMap.Player.VelocityX = GetVelocityChanges((Keyboard.Key.A, Keyboard.Key.D), testMap.Player.VelocityX, testMap.Player.VelocityLimit, testMap.Player.AccelerationRate);

            var dCoord = testMap.Player.ViewDirection * testMap.Player.VelocityY + testMap.Player.ViewDirection.Rotate(-90) * testMap.Player.VelocityX;

            PerformCollide(ref dCoord);

            testMap.Player.MapPosition = testMap.Player.MapPosition + dCoord;
        }

        public static float GetVelocityChanges((Keyboard.Key, Keyboard.Key) keys, float velocity, float velLimit, float accRate) {
            if (Keyboard.IsKeyPressed(keys.Item1) || Keyboard.IsKeyPressed(keys.Item2)) {
                if (Keyboard.IsKeyPressed(keys.Item1))
                    velocity = Math.Abs(velocity) > velLimit && velocity > 0 ? velocity - accRate : velocity + accRate;
                if (Keyboard.IsKeyPressed(keys.Item2))
                    velocity = Math.Abs(velocity) > velLimit && velocity < 0 ? velocity + accRate : velocity - accRate;
            } else {
                velocity = (float)Math.Round(Math.Sign(velocity) * (Math.Abs(velocity) - accRate), 4);
            }

            return velocity;
        }

        public static void PerformCollide(ref Vector2f deltaCoord) {
            if (deltaCoord.X == 0 && deltaCoord.Y == 0) return;

            var collidables = testMap.Items.Where(item => item.isCollidable());

            foreach (var item in collidables) {

                ////Normal, distance
                var vec = (testMap.Player.MapPosition + deltaCoord).GetSausageDistance(item.GetCoords()).Normalize();

                var coords = item.GetCoords();

                if (vec.Item2 <= COLLIDE_DISTANCE) {
                    //var mutation = deltaCoord.Cross(vec.Item1) * vec.Item1;
                    //deltaCoord = deltaCoord - mutation;
                    deltaCoord = new Vector2f();
                    testMap.Player.VelocityX = 0;
                    testMap.Player.VelocityY = 0;
                }

                //var shiftCut = (testMap.Player.MapPosition, testMap.Player.MapPosition + deltaCoord);

                //var its = (testMap.Player.MapPosition, testMap.Player.MapPosition + deltaCoord).GetIntersection(coords);
                //if (its.HasIntersection) {
                //    var face = (coords.Item1 - its.PointOfIntersection).GetVecByOrtoBasis(deltaCoord).X < 0;
                //    var base4basis = ((face ? coords.Item1 : coords.Item2) - its.PointOfIntersection).Normalize().Item1;
                //    var basis = (base4basis, base4basis.Rotate(90));
                //    var newBasisVec = (deltaCoord - its.PointOfIntersection).GetVecByBasis(basis);

                //    var correctionVec = (newBasisVec - new Vector2f(0, newBasisVec.Y + COLLIDE_DISTANCE)).RestoreVecByBasis(basis);

                //    deltaCoord = deltaCoord + correctionVec;
                //}
            }
        }

        private static void PrepareWindow() {
            WindowWidth = VideoMode.DesktopMode.Width;
            WindowHeight = VideoMode.DesktopMode.Height;
            
            window = new RenderWindow(new SFML.Window.VideoMode(WindowWidth, WindowHeight), "PrimalFury", Styles.None);

            window.SetVerticalSyncEnabled(true);
            window.SetMouseCursorVisible(false);
            window.KeyPressed += Window_KeyPressed;

            window.Closed += Window_Closed;
            window.MouseMoved += Window_MouseMoved;
            window.MouseButtonPressed += Window_MouseButtonPressed;

            testRenderer = new Renderer(window);
        }

        private static void PrepareMap() {
            testBuilder = new TestMapBuilder();
            testMap = new Map(testBuilder);
            testMiniMap = new TestMinimap(testMap);
            vp = new Viewport((Vector2f)window.Size, testMap);
            testMap.Player.ViewDirection = new Vector2f(0, 1);
        }

        private static void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
            MouseKeyPressed = e.Button.ToString();
        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e) {
            XDiff = (int)(e.X - WindowWidth / 2);
            YDiff = (int)(e.Y - WindowHeight / 2);
            MouseX = (uint)e.X;
            MouseY = (uint)e.Y;
            if (((Window)sender).HasFocus())
                Mouse.SetPosition(new Vector2i((int)WindowWidth / 2, (int)WindowHeight / 2), (Window)sender);
            else {
                XDiff = 0; YDiff = 0;
            }

            testMap.Player.ViewDirection = testMap.Player.ViewDirection.Rotate(XDiff * 0.02f);
        }

        private static void Window_Closed(object sender, EventArgs e) {
            var window = (Window)sender;
            window.Close();
        }

        private static void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e) {
            var window = (Window)sender;
            if (PreviousKey != e.Code.ToString()) {
                PreviousKeyCount = 0;
            }
            PreviousKey = e.Code.ToString();
            PreviousKeyCount += 1;
            if (e.Code == Keyboard.Key.Escape) {
                window.Close();
            }
        }

        private static void RenderHUD() {
            var minimap = testMiniMap.GetLines();
            testMiniMap.Draw(testRenderer, settings.MinimapPosition);

            //testRenderer.DrawLineList(minimap, settings.MinimapPosition);

            // window.Draw(new Vertex[] { new Vertex(new Vector2f(100, 100), Color.Red) , new Vertex(new Vector2f(200, 200), Color.Red) , new Vertex(new Vector2f(400, 500), Color.Red) }, PrimitiveType.LinesStrip);
            //window.Draw(crosshair);
        }
    }
}
