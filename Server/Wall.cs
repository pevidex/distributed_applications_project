using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Wall
    {
        int left;
        int top;
        int width;
        int height;
        public Rectangle r; 

        public Wall(int l, int t)
        {
            left = l;
            top = t;
            width = 15;
            height = 95;
            r = new Rectangle(left, top, width, height);
        }

        public int getLeft()
        {
            return left;
        }

        public int getTop()
        {
            return top;
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }
    }
}
