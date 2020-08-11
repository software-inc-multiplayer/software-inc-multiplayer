using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Extensions
{
    public static class PathUtils
    {
        public static string AssetsPath = Path.Combine(ModController.ModFolder, "Multiplayer", "Assets");
    }
}
