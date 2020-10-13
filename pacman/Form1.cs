using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonTypes;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Net.Sockets;

namespace pacman {
    public partial class Form1 : Form {

        ClientObject clientObject;
        string[] args;
        int port;
        int _round;
        int myId;
        string myUrl;

        ServerInterface serverInterface;
        PictureBox redGhost;
        PictureBox yellowGhost;
        PictureBox pinkGhost;
        int _score;
        List<string> clients;
        List<ClientInterface> clientsInterface = new List<ClientInterface>();

        bool _readFromFile=false;
        string _pathFile;
        System.IO.StreamReader file;
        FailureDetector failureDetector;
        string serverUrl;

        public Form1(string[] a)
        {
            myUrl = a[3];
            args = a;

            string pat = @"(\w+)://(\w+):(\w+)/(\w+)";
            Match match = Regex.Match(args[3], pat);
            Group g = match.Groups[3];
            port = Int32.Parse(g.Value);
            InitializeComponent();
            label2.Visible = false;
            addMsg("ola");
            if (args.Length == 7)
            {
                _readFromFile = true;
                _pathFile=args[6];
            }
            createSessionOrConnectToSession();
            /* serverUrl = serverInterface.getUrl();
             failureDetector = new FailureDetector();
             failureDetector.addProcess(new KnownProcess(serverUrl, "server"));*/
        }

        private void connectToServer(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            serverInterface = (ServerInterface)
                Activator.GetObject(typeof(ServerInterface),
                "tcp://localhost:8086/ServerObject");
            clientObject = new ClientObject(this, serverInterface);
            RemotingServices.Marshal(clientObject, "ClientObject",
                                    typeof(ClientObject));
        }
        public string getUrl() { return myUrl; }
        public void createSessionOrConnectToSession()
        {
            connectToServer(port);
            try
            {
                serverInterface.createGameSession(myUrl, Int32.Parse(args[5]));
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Error while joining Game Session");
            }

        }
        public void updateFilePath(string pathFile)
        {
            _pathFile = pathFile;
            file = new System.IO.StreamReader(@_pathFile);
        }
        public void setIds(int i)
        {
            myId = i;
            addMsg(""+myId);
        }

        public void gameWon()
        {
            label2.Text = "YOU WIN!";
            label2.Visible = true;
        }

        public void gameOver()
        {
            label2.Text = "GAME OVER";
            label2.Visible = true;
        }

        public void updatePacman(int id, int left, int top, int score, int round)
        {
            _round = round;
            Control pacman = this.Controls["pacman" + id];
            pacman.Left = left;
            pacman.Top = top;
            if (id == myId)
                label1.Text = "Score: " + score;
            _score = score;
            if (_readFromFile)
                readKeyFile();
           // Thread t = new Thread(this.failureDetector.ping);
           // t.Start();
        }

        public void globalStatusChat()
        {
            addMsg(failureDetector.getFailureDetectorStatus());
        }
        public void readKeyFile()
        {//falta comparar com o round
            string line;
            string[] lineSplit;
            string[] separators = { ","};
            PictureBox pacman = (PictureBox)this.Controls["pacman" + myId];
            pacman.Image = Properties.Resources.Left;
            if ((line = file.ReadLine()) != null)
            {
                line = line.ToLower();
                lineSplit = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    serverInterface.updatePlayerMove(myId, "down", lineSplit[1]);
                }
                catch(Exception e)
                {
                    failureDetector.getKnownProcessById("server").addFail(new Fail());
                }
            }
        }

        public void createWalls(int id, int left, int top, int width, int height)
        {
            PictureBox picture = new PictureBox
            {
                Name = "pictureBox" + id,
                Location = new System.Drawing.Point(left, top),
                Margin = new System.Windows.Forms.Padding(4, 4, 4, 4),
                Size = new Size(width, height),
                BackColor = Color.MidnightBlue,

        };
            this.Controls.Add(picture);
        }

        public void createPacmans(int id, int left, int top)
        {
            PictureBox picture = new PictureBox
            {
                BackColor = System.Drawing.Color.Transparent,
                Image = global::pacman.Properties.Resources.Left,
                Location = new System.Drawing.Point(left, top),
                Margin = new System.Windows.Forms.Padding(0),
                Name = "pacman" + id,
                Size = new System.Drawing.Size(25, 25),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage,
                TabIndex = id


        };
            this.Controls.Add(picture);
        }

        public void createCoins(int id, int left, int top, int width, int height)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            

            PictureBox picture = new PictureBox
            {
                Name = "pictureBoxCoin" + id,
                Location = new System.Drawing.Point(left, top),
                Margin = new System.Windows.Forms.Padding(4, 4, 4, 4),
                Size = new Size(width, height),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage

        };
            ((System.ComponentModel.ISupportInitialize)(picture)).BeginInit();
            picture.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            ((System.ComponentModel.ISupportInitialize)(picture)).EndInit();
            this.Controls.Add(picture);
        }

        public void updateGhosts(List<List<int>> ghosts)
        {
            redGhost.Left = ghosts[0][0];
            redGhost.Top = ghosts[0][1];
            yellowGhost.Left = ghosts[1][0];
            yellowGhost.Top = ghosts[1][1];
            pinkGhost.Left = ghosts[2][0];
            pinkGhost.Top = ghosts[2][1];


            // this.Controls.g
        }

        public void updateCoins(List<int> coins)
        {
            for (int i = 0; i < coins.Count; i++)
            {
                ((PictureBox)this.Controls["pictureBoxCoin" + coins[i]]).Visible = false;
            }
        }


        public void createGhosts(List<List<int>> ghosts)
        {
            redGhost = new PictureBox
            {
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(4, 4, 4, 4),
                Location = new System.Drawing.Point(ghosts[0][0], ghosts[0][1]),
                Image = global::pacman.Properties.Resources.red_guy,
                Name = "redGhost",
                Size = new Size(ghosts[0][2], ghosts[0][3]),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom


        };
            yellowGhost = new PictureBox
            {
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(4, 4, 4, 4),
                Location = new System.Drawing.Point(ghosts[1][0], ghosts[1][1]),
                Image = global::pacman.Properties.Resources.yellow_guy,
                Name = "yellowGhost",
                Size = new Size(ghosts[1][2], ghosts[1][3]),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom


            };
            pinkGhost = new PictureBox
            {
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(4, 4, 4, 4),
                Location = new System.Drawing.Point(ghosts[2][0], ghosts[2][1]),
                Image = global::pacman.Properties.Resources.pink_guy,
                Name = "pinkGhost",
                Size = new Size(ghosts[2][2], ghosts[2][3]),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        
            };

            this.Controls.Add(redGhost);
            this.Controls.Add(yellowGhost);
            this.Controls.Add(pinkGhost);
        }

        private void keyisdown(object sender, KeyEventArgs e) {
            PictureBox pacman = (PictureBox) this.Controls["pacman" + myId];
            try
            {
                if (e.KeyCode == Keys.Left)
                {
                    pacman.Image = Properties.Resources.Left;
                    serverInterface.updatePlayerMove(myId, "down", "left");
                }
                if (e.KeyCode == Keys.Right)
                {
                    pacman.Image = Properties.Resources.Right;
                    serverInterface.updatePlayerMove(myId, "down", "right");
                }
                if (e.KeyCode == Keys.Up)
                {
                    pacman.Image = Properties.Resources.Up;
                    serverInterface.updatePlayerMove(myId, "down", "up");
                }
                if (e.KeyCode == Keys.Down)
                {
                    pacman.Image = Properties.Resources.down;
                    serverInterface.updatePlayerMove(myId, "down", "down");
                }
            }
            catch (Exception)
            {
                failureDetector.getKnownProcessById("server").addFail(new Fail());
            }
            if (e.KeyCode == Keys.Enter) {
                    tbMsg.Enabled = true; tbMsg.Focus();
               }
        }

        public int getId() { return myId; }

        private void keyisup(object sender, KeyEventArgs e) {
            try { 
                if (e.KeyCode == Keys.Left) {
                    serverInterface.updatePlayerMove(myId, "up", "left");
                }
                if (e.KeyCode == Keys.Right) {
                    serverInterface.updatePlayerMove(myId, "up", "right");
                }
                if (e.KeyCode == Keys.Up) {
                    serverInterface.updatePlayerMove(myId, "up", "up");
                }
                if (e.KeyCode == Keys.Down) {
                    serverInterface.updatePlayerMove(myId, "up", "down");
                }
            }
            catch (Exception)
            {
                failureDetector.getKnownProcessById("server").addFail(new Fail());
            }
        }

        public int getScore()
        {
            return _score;
        }
        
        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (tbMsg.Text.Equals("")) { }
            else if (e.KeyCode == Keys.Enter)
            {
                object message = tbMsg.Text;
                Thread t = new Thread(broadcastMessage);
                t.Start(message);

                tbMsg.Clear();
                tbMsg.Enabled = false;
                this.Focus();

            }
            
        }
        private void broadcastMessage(object msgText)
        {
            string message = "Player " + myId + ": " + ((string) msgText);
            
            for (int i = 0; i < clientsInterface.Count; i++)
                {
                    if (clients[i].Equals(myUrl))
                    {
                        addMsg(message);
                    }
                    else
                    {
                        try
                        {
                            clientsInterface[i].msgToClient(message);
                        }
                        catch (Exception e)
                        {
                            failureDetector.getKnownProcessById(i.ToString()).addFail(new Fail());
                        }
                    }
                }
        }

        public void updatePlayersUrl(List<string> urls) {
            
            /*clients = urls;
            for(int i=0; i<clients.Count; i++)
            {
                ClientInterface c = (ClientInterface)Activator.GetObject(
                                                    typeof(ClientInterface),
                                                    clients[i]);
                failureDetector.addProcess(new KnownProcess(clients[i], i.ToString()));
                clientsInterface.Add(c);
              
            }*/
            
        }

        public void addMsg(string message)
        {
            tbChat.AppendText("\r\n" + message);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
