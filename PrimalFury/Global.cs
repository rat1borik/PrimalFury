using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace PrimalFury
{
    class Global
    {
        // Map and builder
        static Map testMap;
        static TestMapBuilder testBuilder;
        static Renderer testRenderer;

        //Settings

        static Settings settings;

        // Mouse
        static uint MouseX = 0, MouseY = 0; 
        static int  XDiff=0,YDiff=0;
        static double kMouse=0.75;

        // Window
        static RenderWindow window;
        static uint WindowWidth, WindowHeight;

        // Debug info
        static string PreviousKey = "";
        static int PreviousKeyCount = 0;
        static string MouseKeyPressed = "";
        static string debugText="";

        //HUD Parameters
        static Sprite crosshair;
        static uint crosshairRectX=25, crosshairRectY = 25;
        static string fileCrosshair= "crosshair.png";

        static void Main(string[] args)
        {
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

            var circle = new CircleShape(500f, 100) {
                FillColor = Color.Yellow
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
            while (window.IsOpen)
            {
                //debug
                debugText= String.Format("X: {0}, Y: {1}\nKey pressed: {2} : {3} times\nMouse key pressed: {4}", MouseX, MouseY, PreviousKey, PreviousKeyCount, MouseKeyPressed);


                debugInfo.DisplayedString = debugText;
                debugInfo.Position = new Vector2f(WindowWidth-(debugText.Split('\n').OrderByDescending(s=>s.Length).ToArray()[0].Length*(debugInfo.CharacterSize/2)) - 20, 0);
                //circle.Position= new Vector2f(MouseX,MouseY);
                circle.Position = new Vector2f(circle.Position.X+ (int)Math.Round(kMouse * XDiff), circle.Position.Y + (int)Math.Round(kMouse*YDiff));


                window.Clear();
                //window.Clear(Color.White);
                // Process events
                window.DispatchEvents();


                // Draw
                #if DEBUG
                    window.Draw(debugInfo);
                #endif

                window.Draw(circle);

                var minimap = testMap.GetMapView();

                var inters = Utils.MathTools.Vectors.Intersection(minimap[0],minimap[1],true);
                
                // window.Draw(new Vertex[] { new Vertex(new Vector2f(100, 100), Color.Red) , new Vertex(new Vector2f(200, 200), Color.Red) , new Vertex(new Vector2f(400, 500), Color.Red) }, PrimitiveType.LinesStrip);

                testRenderer.DrawLineList(minimap,settings.MinimapPosition);

                if (inters.Item2){
                    window.Draw(new CircleShape(4f, 12) {
                        FillColor = Color.Red,
                        Position = inters.Item1 + (Vector2f)settings.MinimapPosition - new Vector2f(2,2)
                    });
                    window.Draw(new CircleShape(4f, 12) {
                        FillColor = Color.Red,
                        Position = inters.Item1 + (Vector2f)settings.MinimapPosition - new Vector2f(2, 2)
                    });
                }
                RenderHUD();

                
                // Finally, display the rendered frame on screen
                window.Display();
            }


        }

        private static void PrepareWindow() {
            WindowWidth = VideoMode.DesktopMode.Width;
            WindowHeight = VideoMode.DesktopMode.Height;

            window = new RenderWindow(new SFML.Window.VideoMode(WindowWidth, WindowHeight), "PrimalFury", Styles.Fullscreen);

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
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Close();
        }

        private static void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            var window = (Window)sender;
            if(PreviousKey!= e.Code.ToString()) {
                PreviousKeyCount = 0;
            }
            PreviousKey = e.Code.ToString();
            PreviousKeyCount += 1;
            if (e.Code == Keyboard.Key.Escape){
                window.Close();
            }
        }
        private static void RenderHUD() {
            window.Draw(crosshair);
        }
    }
}
