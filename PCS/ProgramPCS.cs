using CommonTypes;
using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PCS
{
    class ProgramPCS
    {
        static void Main(string[] args)
        {
            int port = 11000;
            //System.Console.WriteLine("INSERT PORT:");
            //port = Int32.Parse(System.Console.ReadLine());
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            PCS pcs = new PCS();
            RemotingServices.Marshal(pcs, "PCS",
                typeof(PCS));
            System.Console.WriteLine("<enter> para sair ...");
            System.Console.ReadLine();
        }
    }
    public class PCS : MarshalByRefObject,PCSInterface
    {
             Process myProcess;
            
            public void startClient(string args)
            {
            string path = "..\\..\\..\\pacman\\bin\\Debug\\pacman.exe";
            myProcess = Process.Start(path, args);
            System.Console.WriteLine("Client Started");
            }

            public void startServer(string args)
            {
            string path = "..\\..\\..\\Server\\bin\\Debug\\Server.exe";
            myProcess = Process.Start(path, args);
            System.Console.WriteLine("Server Started");
        }
    }
}
