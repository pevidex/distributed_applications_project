using CommonTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Player
    {
        private int id;
        private string url;
        bool lost = false;
        bool alive = true;

        // direction player is moving in. Only one will be true
        bool goup;
        bool godown;
        bool goleft;
        bool goright;

        //pacman position
        int left;
        int top;
        int width = 25;
        int height = 25;
        Rectangle r; 

        //player speed
        int speed = 5;

        public int score = 0;

        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;

        public Player(string u, int i)
        {
            url = u;
            id = i;
            left = 11;
            top = 49;
            r = new Rectangle(left, top, width, height);

        }

        public string getUrl()
        {
            return url;
        }

        public int getId()
        {
            return id;
        }

        public void setId(int i)
        {
            id = i;
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

        public void setTop(int t)
        {
            top = t;
        }

        public int getScore()
        {
            return score;
        }

        public void setScore(int s)
        {
            score = s;
        }

        public void setGoLeft(string type)
        {
            if (type.Equals("down"))
                goleft = true;
            else
                goleft = false;
        }

        public void setGoRight(string type)
        {
            if (type.Equals("down"))
                goright = true;
            else
                goright = false;
        }

        public void setGoUp(string type)
        {
            if (type.Equals("down"))
                goup = true;
            else
                goup = false;
        }

        public void setGoDown(string type)
        {
            if (type.Equals("down"))
            {
                godown = true;
            }
            else
                godown = false;
        }

        public bool getLost()
        {
            return lost;
        }

        public bool isAlive()
        {
            return alive;
        }

        public void setAlive(bool a)
        {
            alive = a;
        }


        public void movePlayer()
        {
            if (lost)
                return;
            if (goleft)
            {
                if (left > (boardLeft))
                    left -= speed;
            }
            if (goright)
            {
                if (left < (boardRight))
                    left += speed;
            }
            if (goup)
            {
                if (top > (boardTop))
                    top -= speed;
            }
            if (godown)
            {
                if (top < (boardBottom))
                    top += speed;
            }
            r.X = left;
            r.Y = top;


        }

        public bool IntersectsWith(Rectangle x)
        {
            if (r.IntersectsWith(x))
            {
                return true;
            }
            return false;
        }

        public void GameOver()
        {
            lost = true;
            left = 8;
            top = 40;
            ClientInterface c = (ClientInterface)Activator.GetObject(
                            typeof(ClientInterface),
                            this.getUrl());
            c.gameOver();
        }
    }
}
