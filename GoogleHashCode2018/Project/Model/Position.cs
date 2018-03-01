using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Model
{
    public class Position
    {
        public long X { get; set; }
        public long Y { get; set; }

        public Position(long x, long y)
        {
            X = x;
            Y = y;
        }
    }
}
