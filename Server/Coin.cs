using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Coin
    {
        int left;
        int id;
        int top;
        int width;
        int height;
        public Rectangle r;

        public Coin(int num, int l, int t)
        {
            id = num;
            left = l;
            top = t;
            width = 15;
            height = 15;
            r = new Rectangle(left, top, width, height);
        }

        public int getId()
        {
            return id;
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
