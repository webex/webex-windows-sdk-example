using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebexSDK;

namespace KitchenSink
{
    public class TimelineMessage
    {
        public Message MessageInfo{ get; set; }

        public List<File> Files { get; set; }

        public class File
        {
            public string Name { get; set; }
            public UInt64 Size { get; set; }
            public string ThumbnailPath { get; set; }
        }
    }
}
