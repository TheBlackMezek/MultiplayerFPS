using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetNode : MonoBehaviour {

    public enum PosMode
    {
        RANDOM,
        RIVER
    }

    public float posRange = 50.0f;
    public float streamStretch = 25.0f;

    public byte[] ip;
    public List<NetNode> connections;
    public LineRenderer lr;
    public List<int> connsFromStart = new List<int>();

    private float riverZPos = float.MinValue;

    public string IpString
    {
        get
        {
            string s = "";

            for (int i = 0; i < ip.Length - 1; ++i)
            {
                s += ip[i].ToString() + '.';
            }
            s += ip[ip.Length - 1].ToString();

            return s;
        }
    }

    public void CalcPos(PosMode mode)
    {
        if(riverZPos == float.MinValue)
        {
            riverZPos = Random.Range(-posRange, posRange);
        }

        if(mode == PosMode.RANDOM)
        {
            transform.position = new Vector3(Random.Range(-posRange, posRange),
                                             Random.Range(-posRange, posRange),
                                             Random.Range(-posRange, posRange));
        }
        else if(mode == PosMode.RIVER)
        {
            float avg = 0;
            foreach(int i in connsFromStart)
            {
                avg += i;
            }
            avg = avg / connsFromStart.Count;

            transform.position = new Vector3(avg * streamStretch, 0, riverZPos);
        }
    }

    public void Setup()
    {
        
        Color c = new Color(ip[0] / 255.0f,
                            ip[1] / 255.0f,
                            ip[2] / 255.0f,
                            (((ip[3] / 255.0f) / 2.0f)) + 0.5f);
    
        Material mat = GetComponent<Renderer>().material;
        mat.color = c;

        lr.startColor = c;
        lr.endColor = c;
        lr.positionCount = connections.Count * 2 + 1;
        lr.SetPosition(0, transform.position);
        for(int i = 0; i < connections.Count; ++i)
        {
            lr.SetPosition(i * 2 + 1, connections[i].transform.position);
            lr.SetPosition(i * 2 + 2, transform.position);
        }
    }

}
