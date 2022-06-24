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
        static string debugText="";
        static void Main(string[] args)
        {
            // get Desktop params
            WindowWidth = VideoMode.DesktopMode.Width;
            WindowHeight = VideoMode.DesktopMode.Height;

            window=new RenderWindow(new SFML.Window.VideoMode(WindowWidth,WindowHeight), "PrimalFury", Styles.Fullscreen);

            window.SetVerticalSyncEnabled(true);
            //window.SetMouseCursorVisible(false);

            window.KeyPressed += Window_KeyPressed;
            window.Closed += Window_Closed;
            window.MouseMoved += Window_MouseMoved;

            var circle = new CircleShape(500f, 100)
            {
                FillColor = Color.Yellow
            };
            var debugInfo = new Text() {
                Font = new Font("font.otf"),
                CharacterSize = 14,
                FillColor = Color.Green,
                Position = new Vector2f(WindowWidth-200,0)
            };

            // Start the game loop
            Mouse.SetPosition(new Vector2i((int)WindowWidth / 2, (int)WindowHeight / 2), window);
            while (window.IsOpen)
            {
                //debug
                debugText= String.Format("X: {0}, Y: {1}\nKey pressed: {2} : {3} times", MouseX, MouseY, PreviousKey, PreviousKeyCount);


                debugInfo.DisplayedString = debugText;
                debugInfo.Position = new Vector2f(WindowWidth-(debugText.Split('\n').OrderByDescending(s=>s.Length).ToArray()[0].Length*(debugInfo.CharacterSize/2)) - 20, 0);
                //circle.Position= new Vector2f(MouseX,MouseY);
                circle.Position = new Vector2f(circle.Position.X+ (int)Math.Round(kMouse * XDiff), circle.Position.Y + (int)Math.Round(kMouse*YDiff));
                window.Clear();
                // Process events
                window.DispatchEvents();


                // Draw
                #if DEBUG
                    window.Draw(debugInfo);
                #endif
                window.Draw(circle);

                // Finally, display the rendered frame on screen
                window.Display();
            }


        }

        private static void Window_MouseMoved(object sender, MouseMoveEventArgs e) {
            XDiff = (int)(e.X - WindowWidth / 2);
            YDiff = (int)(e.Y - WindowHeight / 2);
            MouseX = (uint)e.X;
            MouseY = (uint)e.Y;
            if(((Window)sender).HasFocus())
            Mouse.SetPosition(new Vector2i((int)WindowWidth / 2, (int)WindowHeight / 2), (Window)sender);
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
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
        }
    }
}
