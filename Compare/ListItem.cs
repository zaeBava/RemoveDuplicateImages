using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Compare
{
    public class ListItem
    {
        public Byte[] GrayscaleValue { get; set; }
        public String OriginalImagePath { get; set; }

        public ListItem(Byte[] value, string path)
        {
            GrayscaleValue = value;
            OriginalImagePath = path;
        }
    }
}
