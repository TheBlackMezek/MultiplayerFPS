using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class Tracert : MonoBehaviour {

    public string[] prog;
    public string[] args;
    public bool[] useArgs;
    public GameObject nodePrefab;

    private System.Diagnostics.Process p;
    private List<NetNode> nodes = new List<NetNode>();
    public string output;


    void Start () {
        for(int i = 0; i < prog.Length; ++i)
        {
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = prog[i];
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            if (useArgs[i])
            {
                p.StartInfo.Arguments = args[i];
            }

            p.Start();


            StartCoroutine("ReceiveOutput");
        }
        BuildNetModel();
    }
	
	private IEnumerator ReceiveOutput()
    {
        output = p.StandardOutput.ReadToEnd();
        AddTracertResults();
        yield return null;
    }

    private void AddTracertResults()
    {
        string[] lines = output.Split();
        NetNode lastNode = null;

        for(int i = 0; i < lines.Length; ++i)
        {
            string l = lines[i];
            if(l.Length > 0 && (l[0] == '[' || (!StrContainsLetters(l) && CountChars(l, '.') == 3)))
            {
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

                if(lastNode != null)
                {
                    node.connections.Add(lastNode);
                }

                lastNode = node;
            }
        }
        
    }

    private void BuildNetModel()
    {
        foreach (NetNode n in nodes)
        {
            n.RandPos();
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
