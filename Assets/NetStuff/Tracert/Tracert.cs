using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;






public class Tracert : MonoBehaviour {

    public string[] prog;
    public string[] args;
    public GameObject nodePrefab;

    //private System.Diagnostics.Process p;
    private List<NetNode> nodes = new List<NetNode>();
    public List<string> outputs = new List<string>();
    private delegate void ThreadCall(object data);
    
    private int lastOutputCount = 0;


    void Start () {
        ThreadCall tc = ReceiveOutput;
        for(int i = 0; i < prog.Length; ++i)
        {
            System.Diagnostics.Process p;
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = prog[i];
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            
            p.StartInfo.Arguments = args[i];

            p.Start();

            Thread thread = new Thread(new ParameterizedThreadStart(tc));
            thread.Start(p);
        }
    }

    private void Update()
    {
        if(outputs.Count > lastOutputCount)
        {
            AddTracertResults(outputs[lastOutputCount]);
            ++lastOutputCount;
            BuildNetModel();
        }
    }



    private void ReceiveOutput(object data)
    {
        System.Diagnostics.Process p = (System.Diagnostics.Process)data;
        string output = p.StandardOutput.ReadToEnd();
        lock (this)
        {
            outputs.Add(output);
        }
    }

    private void AddTracertResults(string output)
    {
        string[] lines = output.Split();
        NetNode lastNode = null;
        bool firstIp = true;
        int connectionLength = 0;

        for(int i = 0; i < lines.Length; ++i)
        {
            string l = lines[i];
            if(l.Length > 0 && (l[0] == '[' || (!StrContainsLetters(l) && CountChars(l, '.') == 3)))
            {
                if(firstIp)
                {
                    firstIp = false;
                    continue;
                }

                if(l[0] == '[')
                {
                    l = l.Remove(l.Length - 1, 1);
                    l = l.Remove(0, 1);
                }

                NetNode node = FindNode(l);
                if(node == null)
                {
                    GameObject obj = Instantiate(nodePrefab);
                    node = obj.GetComponent<NetNode>();
                    node.ip = new byte[4];
                    for (int n = 0; n < 3; ++n)
                    {
                        int idx = l.IndexOf('.');
                        node.ip[n] = (byte)int.Parse(l.Substring(0, idx));
                        l = l.Remove(0, idx + 1);
                    }
                    node.ip[3] = (byte)int.Parse(l);
                    nodes.Add(node);
                }
                node.connsFromStart.Add(connectionLength);
                ++connectionLength;

                if (lastNode != null)
                {
                    lastNode.connections.Add(node);
                }

                lastNode = node;
            }
        }
        
    }

    private void BuildNetModel()
    {
        foreach (NetNode n in nodes)
        {
            n.CalcPos(NetNode.PosMode.RIVER);
        }
        foreach (NetNode n in nodes)
        {
            n.Setup();
        }
    }

    private NetNode FindNode(string ip)
    {
        foreach (NetNode n in nodes)
        {
            if (n.IpString == ip)
            {
                return n;
            }
        }
        return null;
    }

    private bool StrContainsLetters(string str)
    {
        bool ret = false;

        str = str.ToLower();
        string[] alphabet = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                             "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
        foreach(string s in alphabet)
        {
            if(str.Contains(s))
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    private int CountChars(string str, char chr)
    {
        int ret = 0;

        foreach(char c in str)
        {
            if(c == chr)
            {
                ++ret;
            }
        }

        return ret;
    }

}
