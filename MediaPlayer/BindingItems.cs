using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MediaPlayer
{
    internal class AudioFile
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Duration { get; set; }
        public StorageFile StorageFile { get; set; }
    }
}
