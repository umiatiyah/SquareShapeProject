using System;
using System.Collections.Generic;
using System.Text;

namespace SquareShapeProject.Models
{
    public class ColorModel
    {
        public string ColorName { get; set; }
    }
    public class ColorResponse : ColorModel
    {
        public string Message { get; set; }
    }
}
