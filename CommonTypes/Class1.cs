using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface ServerInterface 
    {
        void createGameSession(string url, int numOfPlayers);

        void updatePlayerMove(int playerId, string moveType, string direction);

        void globalStatus();

        string ping();

        string getUrl();
        void crash();
    }
    public interface ClientInterface 
    {
        void globalStatus();

        void gameWon();

        string ping();

        void crash();

        void gameOver();

        void msgToClient(string message);

        //na definicao de walls e dado o id ao jogador
        void defineWalls(List<List<int>> walls);

        void defineCoins(List<List<int>> coins);

        void updatePacmans(List<List<int>> pacmans, int round);

        void updateGhosts(List<List<int>> pacmans);

        void updateCoins(List<int> coins);

        void defineGhosts(List<List<int>> ghosts);

        void definePacmans(int i, List<List<int>> pacmans);

        void notifyGameStart();

        void updatePlayersOnClient(List<string> urls);
    }
    public interface PCSInterface
    {
        void startClient(string args);
        void startServer(string args);
    }
    public class PuppetMasterObjectInterface : MarshalByRefObject
    {
    }
}
