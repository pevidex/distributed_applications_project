using CommonTypes;
using pacman;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        static void Main(string[] args)
        {
            PuppetMasterObject puppet = new PuppetMasterObject();
            string line;
            System.Console.Write("comandos:\r\n 1) Start x clients\r\n 2) Global Status\r\n 3) Read From Script (available scripts: scripts1)\r\n 4) Crash PID\r\nInsira comando: ");
            line = System.Console.ReadLine();
            while (!line.Equals("exit"))
            {
                line = puppet.justToMakeEasier(line);
                puppet.processCommand(line);
                System.Console.Write("\r\ncomandos:\r\n 1) Start x clients\r\n 2) Global Status\r\n 3) Read From Script (available scripts: scripts1)\r\n 4) Crash PID\r\nInsira comando: ");
                line = System.Console.ReadLine();
            }
        }
        
    }
    class PuppetMasterObject : PuppetMasterObjectInterface
    {
        Dictionary<string, string> mapa = new Dictionary<string, string>(); //(ID,Url)
        List<string> clientsUrl = new List<string>();
        List<string> serversUrl = new List<string>();

        public void globalStatus()
        {
            for(int i = 0; i < clientsUrl.Count; i++)
            {
                ClientInterface c = (ClientInterface)Activator.GetObject(
                                                typeof(ClientInterface),
                                                clientsUrl[i]);
                c.globalStatus();
            }
            for(int i = 0; i < serversUrl.Count; i++)
            {
                ServerInterface s = (ServerInterface)Activator.GetObject(
                                                 typeof(ServerInterface),
                                                 serversUrl[i]);
                s.globalStatus();
            }
        }

        public void processCommand(string line)
        {
            string[] linesplit = line.Split(null);
            if (linesplit[0].Equals("skip")) return;
            else if (linesplit[0].Equals("GlobalStatus")) //globalstatus on client!!! improve globalstatus on server
                globalStatus();
            else if (linesplit[0].Equals("Crash")) //CRASH PID
            {
                if (serversUrl.Contains(mapa[linesplit[1]]))
                {
                    ServerInterface s = (ServerInterface)Activator.GetObject(
                                                 typeof(ServerInterface),
                                                 mapa[linesplit[1]]);
                    s.crash();
                    serversUrl.Remove(mapa[linesplit[1]]);
                    mapa.Remove(linesplit[1]);
                }
                else if (clientsUrl.Contains(mapa[linesplit[1]]))
                {
                    ClientInterface c = (ClientInterface)Activator.GetObject(
                                                typeof(ClientInterface),
                                                mapa[linesplit[1]]);
                    c.crash();
                    clientsUrl.Remove(mapa[linesplit[1]]);
                    mapa.Remove(linesplit[1]);
                }
            }
            else if (linesplit[0].Equals("LocalState")) { /*todo*/}
            else if (linesplit[0].Equals("StartServer") || linesplit[0].Equals("StartClient"))
            {
                if (linesplit.Length < 5)
                    return;
                PCSInterface pcsInterface = (PCSInterface)
                    Activator.GetObject(typeof(PCSInterface),
                    linesplit[2]);
                if (pcsInterface == null)
                {
                    System.Console.WriteLine("null");
                    return;
                }
                if (linesplit[0].Equals("StartClient"))
                {
                    Console.WriteLine("Starting Client");
                    mapa.Add(linesplit[1], linesplit[3]);
                    clientsUrl.Add(linesplit[3]);
                    pcsInterface.startClient(line);
                }
                else if (linesplit[0].Equals("StartServer"))
                {
                    Console.WriteLine("Starting Server");
                    mapa.Add(linesplit[1], linesplit[3]);
                    serversUrl.Add(linesplit[3]);
                    pcsInterface.startServer(line);
                }
            }
        }
        public string justToMakeEasier(string line)
        {
            string path;
            int players;
            List<string> clients = new List<string>();
            string scripts1 = "..\\..\\..\\scripts1.txt";
            string server = "StartServer server1 tcp://localhost:11000/PCS tcp://localhost:8086/ServerObject 30 ";
            clients.Add("StartClient client1 tcp://localhost:11000/PCS tcp://localhost:8080/ClientObject 30 ");
            clients.Add("StartClient client2 tcp://localhost:11000/PCS tcp://localhost:8081/ClientObject 30 ");
            clients.Add("StartClient client3 tcp://localhost:11000/PCS tcp://localhost:8082/ClientObject 30 ");
            clients.Add("StartClient client4 tcp://localhost:11000/PCS tcp://localhost:8083/ClientObject 30 ");
            clients.Add("StartClient client5 tcp://localhost:11000/PCS tcp://localhost:8084/ClientObject 30 ");
            switch (Int32.Parse(line))
            {
                case 1:
                        System.Console.Write("x: ");
                        line = System.Console.ReadLine();
                        players = Int32.Parse(line);
                        processCommand(server+players);
                        for(int i = 0; i<players; i++) { clients[i] = clients[i] + players; processCommand(clients[i]); }
                    return "skip";
                case 2:
                    return "GlobalStatus";
                case 3: //not tested
                    System.Console.Write("script name: ");
                    path = System.Console.ReadLine();
                    if (path.Equals("scripts1"))
                        path = scripts1;
                    System.IO.StreamReader file = new System.IO.StreamReader(@path);
                    while ((line = file.ReadLine()) != null)
                    {
                       Console.WriteLine("Command processing: "+line);
                       processCommand(line);
                    }
                    return "skip";
                case 4:
                    Console.WriteLine("Available PIDS (Write the name, not the number): ");
                    int a = 1;
                    foreach (KeyValuePair<string, string> entry in mapa)
                    {
                        Console.WriteLine(a + ") " + entry.Key);
                        a++;
                    }
                   
                    System.Console.Write("client PID: ");
                    string pid = System.Console.ReadLine();
                    return "Crash " + pid;
                default:
                    Console.WriteLine("skip");
                    return line;
            }
        }
    }
}
