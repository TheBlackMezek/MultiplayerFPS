using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTest : MonoBehaviour {

    public Renderer renderer;



    private void Start()
    {
        string url = "https://i.redd.it/kbrpe1rsk3r01.jpg";
        WWW www = new WWW(url);
        StartCoroutine(GetImage(www));
    }

    public IEnumerator GetImage(WWW www)
    {
        yield return www;
        
        renderer.material.mainTexture = www.texture;
    }

}
