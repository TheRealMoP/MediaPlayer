using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    internal class FileExtensions
    {
        public static readonly string[] Video = new string[] { ".avi", ".mpg", ".mkv", ".wmv" };
        public static readonly string[] Image = new string[] { ".jpg", ".png", ".bmp", ".gif", ".tif" };
        public static readonly string[] Audio = new string[] { ".mp3", ".wma", ".m4a", ".aac" };
    }
}
