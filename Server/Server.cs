using System;
using CommonTypes;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Diagnostics;
using Server;
using System.Text.RegularExpressions;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            ServerObject serverObject = new ServerObject(args);
            RemotingServices.Marshal(serverObject, "ServerObject",
                                    typeof(ServerObject));
            System.Console.WriteLine("<enter> para sair ...");
            System.Console.ReadLine();
        }
    }


    public class ServerObject : MarshalByRefObject, ServerInterface
    {
        string id;
        int msecperround = 30; //valor padrao
        int maxPlayersServer;

        FailureDetector failureDetector;

        GameObject game;
        int maxPlayersClient;
        int currentPlayers=0;
        bool lobby = false;

        List<Player> players;
        List<string> messages;

        string myUrl;

        public ServerObject(string[] args)
        {
            myUrl =args[3];
            id = args[1];
            msecperround = Int32.Parse(args[4]);
            maxPlayersServer = Int32.Parse(args[5]);
            System.Console.WriteLine("Server Criado com argumentos");
            players = new List<Player>();
            game = new GameObject(msecperround);
            messages = new List<string>();
            failureDetector = new FailureDetector();
            game.setFailureDetector(failureDetector);
        }

        public int getMsec() { return msecperround; }

        
        public void createPlayer(string url) {
            Player player = new Player(url, currentPlayers);
            failureDetector.addProcess(new KnownProcess(url,currentPlayers.ToString()));
            currentPlayers++;
            System.Console.WriteLine("Connectado player" + currentPlayers);
            players.Add(player);
            if (currentPlayers == maxPlayersServer)
            {
                Console.WriteLine("Notifying");
                notifyAllPlayers();
                //game.startGame(players);
            }
        }
        public void crash()
        {
            Environment.Exit(0);
        }
        public string getUrl()
        {
            return myUrl;
        }
        public void globalStatus()
        {
            Console.WriteLine(failureDetector.getFailureDetectorStatus());
        }
        public int getAlivePlayers()
        {
            int count=0;
            for (int i = 0; i < players.Count; i++)
                if (!players[i].getLost())
                    count++;
            return count;
        }
        public void createGameSession(string url, int numberOfPlayers)
        {
            if (!lobby)
            {
                lobby = true;
                Console.WriteLine("Lobby created");
                maxPlayersClient = numberOfPlayers; }
            if (game.isGameRunning())
            {
                if (currentPlayers >= maxPlayersServer)
                {
                    System.Console.WriteLine("full lobby");
                    return;
                }
            }
            System.Console.WriteLine("Joining Session");
            createPlayer(url);

        }
        public string ping()
        {
            return "Alive";
        }

        public void notifyAllPlayers() {
            List<string> clientsUrl = new List<string>();
            for (int i = 0; i < maxPlayersServer; i++)
                clientsUrl.Add(players[i].getUrl());
            for (int i = 0; i < maxPlayersServer; i++)
            {
                try
                {
                    Console.WriteLine(players[i].getUrl());
                    ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[i].getUrl());
                    c.notifyGameStart();
                   // c.updatePlayersOnClient(clientsUrl);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                    //make thread to try again with this client
                    players[i].setAlive(false);
                }
            }
            Console.WriteLine("All players notified");
        }

        public void updatePlayerMove(int id, string type, string direction)
        {
            game.updatePlayerMove(id, type, direction);
        }
        
    }

    public class GameObject
    {

        private bool gameRunning = false;

        FailureDetector failureDetector;

        //remote objects to update client
        List<List<int>> pacmans = new List<List<int>>();
        List<List<int>> ghostsRemote = new List<List<int>>();
        List<List<int>> wallsRemote = new List<List<int>>();
        List<List<int>> coinsRemote = new List<List<int>>();
        List<int> coinsRemoved = new List<int>();

        List<Player> players;
        List<Coin> coins;
        List<Wall> walls;
        Wall wall1;
        Wall wall2;
        Wall wall3;
        Wall wall4;
        Ghost redGhost;
        Ghost yellowGhost;
        Ghost pinkGhost;

        public int _msecperround;

        int round;
        public void setFailureDetector(FailureDetector f)
        {
            failureDetector = f;
        }
        public GameObject(int msecperround)
        {
            round = 0;
            coins = new List<Coin>();
            walls = new List<Wall>();
            _msecperround = msecperround;
            //wall bounds
            wall1 = new Wall(88, 40);
            wall2 = new Wall(248, 40);
            wall3 = new Wall(128, 240);
            wall4 = new Wall(288, 240);

            walls.Add(wall1);
            walls.Add(wall2);
            walls.Add(wall3);
            walls.Add(wall4);

            createCoins();


            //ghost position
            redGhost = new Ghost(160, 70, 5, 0);
            yellowGhost = new Ghost(200, 270, 5, 0);
            pinkGhost = new Ghost(320, 70, 5, 5);
        }

        public bool isGameRunning()
        {
            return gameRunning;
        }

        public void startGame(List<Player> playerList)
        {
            Console.WriteLine("GAME STARTING");
            gameRunning = true;
            players = playerList;

            //initiate list of pacmans 
            for (int i = 0; i < players.Count; i++)
            {
                pacmans.Add(new List<int> { players[i].getId(), players[i].getLeft(), players[i].getTop(), players[i].getScore() });
            }

            //initiate list of walls
            for (int i = 0; i < walls.Count; i++)
            {
                wallsRemote.Add(new List<int> { walls[i].getLeft(), walls[i].getTop(), walls[i].getWidth(), walls[i].getHeight() });
            }

            //initiate list of walls
            for (int i = 0; i < coins.Count; i++)
            {
                coinsRemote.Add(new List<int> { coins[i].getLeft(), coins[i].getTop(), coins[i].getWidth(), coins[i].getHeight() });
            }

            ghostsRemote.Add(new List<int> { redGhost.getLeft(), redGhost.getTop(), redGhost.getWidth(), redGhost.getHeight() });
            ghostsRemote.Add(new List<int> { yellowGhost.getLeft(), yellowGhost.getTop(), yellowGhost.getWidth(), yellowGhost.getHeight() });
            ghostsRemote.Add(new List<int> { pinkGhost.getLeft(), pinkGhost.getTop(), pinkGhost.getWidth(), pinkGhost.getHeight() });


            for (int i = 0; i < players.Count; i++)
            {
                try
                {
                    ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[i].getUrl());
                    c.definePacmans(i, pacmans);
                    c.defineWalls(wallsRemote);
                    c.defineGhosts(ghostsRemote);
                    c.defineCoins(coinsRemote);
                }
                catch(Exception e)
                {
                    Console.WriteLine("ERRO2");
                    failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                    players[i].setAlive(false);
                }
            }

            game();
        }

        //create coins on server
        private void createCoins()
        {
            int left = 8;
            int top = 40;
            bool coinInWall;
            int id = 0;
            while (left < 330)
            {
                while (top < 330)
                {
                    coinInWall = false;
                    Coin c = new Coin(id, left, top);

                    for (int i = 0; i < walls.Count; i++)
                        if (c.r.IntersectsWith(walls[i].r))
                        {
                            coinInWall = true;
                            id--;
                        }
                    if (!coinInWall)
                        coins.Add(c);
                    top += 40;
                    id++;
                }
                left += 40;
                top = 40;
            }
        }

        private void game()
        {
            while (gameRunning)
            {
                Thread.Sleep(_msecperround);
                gameTick();
                sendUpdatePacmans();
                sendUpdateGhosts();
                round++;
            }
        }

        public void updatePlayerMove(int id, string type, string direction)
        {
            switch (direction)
            {
                case "left":
                    players[id].setGoLeft(type);
                    break;
                case "right":
                    players[id].setGoRight(type);
                    break;
                case "up":
                    players[id].setGoUp(type);
                    break;
                case "down":
                    players[id].setGoDown(type);
                    break;
                default:
                    break;
            }
        }
        private void updateRemoteGhosts()
        {
            ghostsRemote[0][0] = redGhost.getLeft();
            ghostsRemote[0][1] = redGhost.getTop();
            ghostsRemote[1][0] = yellowGhost.getLeft();
            ghostsRemote[1][1] = yellowGhost.getTop();
            ghostsRemote[2][0] = pinkGhost.getLeft();
            ghostsRemote[2][1] = pinkGhost.getTop();

        }
        private void gameTick()
        {

            //move player
            for(int i = 0; i < players.Count; i++)
            {
                players[i].movePlayer();
            }

            //move ghosts
            redGhost.Move();
            yellowGhost.Move();
            pinkGhost.Move();

            updateRemoteGhosts();

            redGhost.IntersectsWith(wall1);
            redGhost.IntersectsWith(wall2);

            yellowGhost.IntersectsWith(wall3);
            yellowGhost.IntersectsWith(wall4);


            //moving ghosts and bumping with the walls end
            //for loop to check walls, ghosts and points
            for (int i = 0; i < players.Count; i++)
            {
                Player p = players[i];
                for (int j = 0; j < walls.Count; j++)
                {
                    if (p.IntersectsWith(walls[j].r))
                    {
                        p.GameOver();
                    }
                }

                if (p.IntersectsWith(redGhost.r) || p.IntersectsWith(yellowGhost.r) || p.IntersectsWith(pinkGhost.r))
                {
                    p.GameOver();
                }


                for (int j = 0; j < coins.Count; j++)
                    if (p.IntersectsWith(coins[j].r))
                    {
                        coinsRemoved.Add(coins[j].getId());
                        coins.RemoveAt(j);
                        p.score++;
                    }
                
            }
            if (coinsRemoved.Count > 0)
            {
                sendUpdateCoins();
            }

            

            pinkGhost.IntersectsWith(wall1);
            pinkGhost.IntersectsWith(wall2);
            pinkGhost.IntersectsWith(wall3);
            pinkGhost.IntersectsWith(wall4);
            pinkGhost.checkBoardX();
            pinkGhost.checkBoardY();

            if (coins.Count == 0)
            {
                calculateWinner();
                gameRunning = true;
                Thread.Sleep(10000);
                //TODO Close Game Clean Variables
                }

        }

        private void sendUpdatePacmans()
        {
            for(int i = 0; i < players.Count; i++)
            {
                pacmans[i][0] = i;
                pacmans[i][1] = players[i].getLeft();
                pacmans[i][2] = players[i].getTop();
                pacmans[i][3] = players[i].getScore();
            }

            for(int i = 0; i < players.Count; i++)
            {
                try
                {
                    ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[i].getUrl());
                    c.updatePacmans(pacmans, round);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERRO3"+i);
                    failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                    players[i].setAlive(false);
                }
            }
       

        }
        private void sendUpdateGhosts()
        {
            for (int i = 0; i < players.Count; i++)
            {
                try
                {
                    ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[i].getUrl());
                    c.updateGhosts(ghostsRemote);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERRO4"+ i);
                    failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                    players[i].setAlive(false);
                }
            }

        }

        private void sendUpdateCoins()
        {
            for (int i = 0; i < players.Count; i++)
            {
                try
                {
                    ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[i].getUrl());
                    c.updateCoins(coinsRemoved);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERRO5");
                    failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                    players[i].setAlive(false);
                }
            }
            coinsRemoved.Clear();
        }

        //we don't check ties
        private void calculateWinner()
        {
            int maxCoins = 0;
            int playerId = 0;

            for(int i = 0; i < players.Count; i++)
            {
                Player p = players[i];
                if(p.score > maxCoins)
                {
                    maxCoins = p.score;
                    playerId = p.getId();
                }
            }

            try
            {
                ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[playerId].getUrl());
                c.gameWon();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO6");
                failureDetector.getKnownProcessById(playerId.ToString()).addFail(new Fail());
                players[playerId].setAlive(false);
            }

            for(int i = 0; i < players.Count && players[i].getId()!=playerId; i++)
            {
                try
                {
                    ClientInterface c = (ClientInterface)Activator.GetObject(typeof(ClientInterface), players[i].getUrl());
                    c.gameOver();
                }
                catch(Exception e)
                {
                    Console.WriteLine("ERRO7");
                    failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                    players[i].setAlive(false);
                }
            }

        }

    }

}
