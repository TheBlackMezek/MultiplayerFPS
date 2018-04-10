using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetNode : MonoBehaviour {

    public float posRange = 50.0f;

    public byte[] ip;
    public List<NetNode> connections;
    public LineRenderer lr;

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

    public void RandPos()
    {
        transform.position = new Vector3(Random.Range(-posRange, posRange),
                                         Random.Range(-posRange, posRange),
                                         Random.Range(-posRange, posRange));
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
