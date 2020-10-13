using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Ghost
    {
        public int left;
        public int top;
        int width;
        int height;
        int speedx;
        int speedy;
        public Rectangle r;

        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;

        public Ghost(int l, int t, int sx, int sy)
        {
            left = l;
            top = t;
            width = 30;
            height = 30;
            speedx = sx;
            speedy = sy;
            r = new Rectangle(left, top, width, height);
        }

        public void Move()
        {
            left += speedx;
            top += speedy;
            r.X = left;
            r.Y = top;

        }

        public void IntersectsWith(Wall wall)
        {
            if (r.IntersectsWith(wall.r))
                speedx = -speedx;
        }

        public void checkBoardX()
        {
            if (left < boardLeft || left > boardRight)
                speedx = -speedx;
        }

        public void checkBoardY()
        {
            if (top < boardTop || top + height > boardBottom - 2)
            {
                speedy = -speedy;
            }
        }
        public int getLeft()
        {
            return left;
        }

        public void setLeft(int l)
        {
            left = l;
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
