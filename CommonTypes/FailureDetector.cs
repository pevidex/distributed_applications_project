using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public class FailureDetector
    {
        List<KnownProcess> knownProcesses;

        public FailureDetector()
        {
            knownProcesses = new List<KnownProcess>();
        }
        public string getFailureDetectorStatus()
        {
            string ok="OK: ";
            string suspicious="     suspicious: ";
            for (int i = 0; i < knownProcesses.Count; i++)
            {
                if (knownProcesses[i].getStatus().Equals("OK"))
                    ok = ok + knownProcesses[i].getId() + ", ";
                else
                    suspicious = suspicious + knownProcesses[i].getId() + ", ";
            }
            return ok+suspicious;

        }
        public void ping()
        {   
            for (int i = 0; i < knownProcesses.Count; i++)
            {
                string alive;
                try
                {
                    if (knownProcesses[i].getId().Equals("server"))
                    {
                        ServerInterface s = (ServerInterface)Activator.GetObject(
                                                       typeof(ServerInterface),
                                                       knownProcesses[i].getUrl());
                        //alive = s.ping();
                    }
                    else
                    {
                        ClientInterface c = (ClientInterface)Activator.GetObject(
                                                           typeof(ClientInterface),
                                                           knownProcesses[i].getUrl());
                       // alive = c.ping();
                    }
                   // if (!alive.Equals("alive")) {
                        //todo
                   // }
                }
                catch (Exception e) {
                    knownProcesses[i].addFail(new Fail());
                }
            }
        }
        public List<KnownProcess> getKnownProcesses()
        {
            return knownProcesses;
        }
        public KnownProcess getKnownProcessByIndex(int i)
        {
            return knownProcesses[i];
        }
        public KnownProcess getKnownProcessByUrl(string url)
        {
            for (int i = 0; i < knownProcesses.Count; i++)
                if (knownProcesses[i].getUrl().Equals(url))
                {
                    return knownProcesses[i];
                }
            return null;
        }
        public KnownProcess getKnownProcessById(string id)
        {
            for (int i = 0; i < knownProcesses.Count; i++)
                if (knownProcesses[i].getId().Equals(id))
                {
                    return knownProcesses[i];
                }
            return null;
        }
        public FailureDetector(List<KnownProcess> knownProcessesArg)
        {
            knownProcesses = knownProcessesArg;
        }
        public void addProcess(KnownProcess knownProcess)
        {
            knownProcesses.Add(knownProcess);
        }
        public void removeProcess(string url)
        {
            for (int i = 0; i < knownProcesses.Count; i++)
                if (knownProcesses[i].getUrl().Equals(url))
                {
                    knownProcesses.RemoveAt(i);
                }
        }
    }
    public class KnownProcess
    {
        string id;
        string url;
        string status;
        List<Fail> fails;

        public KnownProcess(string _url, string _id)
        {
            fails = new List<Fail>();
            id = _id;
            url = _url;
            status = "OK";
        }
        public string getStatus()
        {
            return status;
        }
        public void addFail(Fail fail)
        {
            status="Suspicious";
            fails.Add(fail);
        }
        public string getUrl()
        {
            return url;
        }
        public void setStatus(string _status)
        {
            status = _status;
        }

        public string getId()
        {
            return id;
        }
    }
    public class Fail{
    }
}