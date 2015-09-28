using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatrice.BLL
{
    //public enum SoundType
    //{
    //    TTS,
    //    MP3,
    //    Wave,
    //}

    public class Slide
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string MoviePath { get; set; }
        public string TitlePath { get; set; }
        public bool Loop { get; set; }
        public bool AutoPlay { get; set; }
    }
}
