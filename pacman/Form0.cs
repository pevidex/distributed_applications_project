using System;
using CommonTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace pacman
{
    public partial class Form0 : Form
    {
        ServerInterface serverInterface;
        ClientObject clientObject;
        string[] args;
        int port;
        string url;

        public Form0(string[] a)
        {
            url = a[3];
            args = a;
            string pat = @"(\w+)://(\w+):(\w+)/(\w+)";
            Match match = Regex.Match(args[3], pat);
            Group g = match.Groups[3];
            port = Int32.Parse(g.Value);
            InitializeComponent();
            createSessionOrConnectToSession();
        }

        public string getUrl() { return url; }
        public ClientObject getClientObject()
        {
            return clientObject;
        }
        public string[] getArgs() { return args; }
        private void connectToServer(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            serverInterface = (ServerInterface)
                Activator.GetObject(typeof(ServerInterface),
                "tcp://localhost:8086/ServerObject");
            //clientObject = new ClientObject(this,serverInterface);
            RemotingServices.Marshal(clientObject, "ClientObject",
                                    typeof(ClientObject));
        }

        public void createSessionOrConnectToSession()
        {
            connectToServer(port);
            try
            {
                serverInterface.createGameSession(url, Int32.Parse(args[5]));
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Error while joining Game Session");
            }

        }

        private void Form0_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }


}