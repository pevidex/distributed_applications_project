using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace pacman
{
    public delegate void closeForm();
    public delegate void addMsgDel(string message);
    public delegate void endGame();
    public delegate void updatePlayersUrl(List<string> urls);
    public delegate void winGame();
    public delegate void setId(int id);
    public delegate void changePacman(int id, int left, int top, int score, int round);
    public delegate void changeGhosts(List<List<int>> ghosts);
    public delegate void createPacman(int id, int left, int top);
    public delegate void createWall(int id, int l, int t, int w, int h);
    public delegate void createCoin(int id, int l, int t, int w, int h);
    public delegate void createGhost(List<List<int>> ghosts);
    public delegate void updateCoin(List<int> coins);
    public delegate void globalStatusChat();


    public class ClientObject : MarshalByRefObject, ClientInterface
    {
        Form1 gameForm;
        List<string> clients;
        ServerInterface serverInterface;

        public ClientObject(Form1 form,ServerInterface si)
        {
            serverInterface = si;
            gameForm = form;
        }
        public Form1 getForm()
        {
            return gameForm;
        }
         public void msgToClient(string message)
        {
            gameForm.Invoke(new addMsgDel(gameForm.addMsg), message);
        }

        public void updatePacmans(List<List<int>> pacmans, int round)
        {
            for(int i = 0; i < pacmans.Count; i++)
            {
                gameForm.Invoke(new changePacman(gameForm.updatePacman), pacmans[i][0], pacmans[i][1], pacmans[i][2], pacmans[i][3], round);
            }
        }

        public void definePacmans(int id, List<List<int>> pacmans)
        {
            gameForm.Invoke(new setId(gameForm.setIds), id);
            for (int i = 0; i < pacmans.Count; i++)
            {
                gameForm.Invoke(new createPacman(gameForm.createPacmans), pacmans[i][0], pacmans[i][1], pacmans[i][2]);
            }
        }

        public void defineWalls(List<List<int>> walls)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                gameForm.Invoke(new createWall(gameForm.createWalls), i, walls[i][0], walls[i][1], walls[i][2], walls[i][3]);
            }   
        }

        public void defineCoins(List<List<int>> coins)
        {
            for (int i = 0; i < coins.Count; i++)
            {
                gameForm.Invoke(new createCoin(gameForm.createCoins), i, coins[i][0], coins[i][1], coins[i][2], coins[i][3]);
            }
        }

        public void updateGhosts(List<List<int>> ghosts)
        {
                gameForm.Invoke(new changeGhosts(gameForm.updateGhosts), ghosts);
        }

        public void defineGhosts(List<List<int>> ghosts)
        {
                gameForm.Invoke(new createGhost(gameForm.createGhosts),ghosts);
        }

        public void updateCoins(List<int> coins)
        {
            gameForm.Invoke(new updateCoin(gameForm.updateCoins), coins);
        }
        public void crash()
        {
            Application.Exit();
        }
        public void globalStatus()
        {
            gameForm.Invoke(new globalStatusChat(gameForm.globalStatusChat));
        }

        public void gameWon()
        {
            gameForm.Invoke(new winGame(gameForm.gameWon));
        }

        public void gameOver()
        {
            gameForm.Invoke(new endGame(gameForm.gameOver));
        }

        public void notifyGameStart()
        {
            
        }

        public void updatePlayersOnClient(List<string> urls)
        {
            clients = urls;
            gameForm.BeginInvoke(new updatePlayersUrl(gameForm.updatePlayersUrl), urls);
        }
        
        public string ping()
        {
            return "Alive";
        }
        public override object InitializeLifetimeService()
        {
            //return base.InitializeLifetimeService();
            return null;
        }

    }
}
