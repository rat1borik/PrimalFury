using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace PrimalFury {
    public struct Settings { 
        public Vector2f MinimapPosition{ get; set; }
    }

    interface ISettingsLoader {
        Settings Load();
    }
}
