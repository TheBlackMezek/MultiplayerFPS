using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtUIManager : MonoBehaviour {
    
    public Image colorImage;


    private void Start()
    {
        Invoke("Init",0.1f);
    }

    public void Init()
    {
        if(GlobalVars.localPlacer != null)
        {
            GlobalVars.localPlacer.onColorChange += UpdateColorImage;
        }
        else
        {
            Invoke("Init", 0.1f);
        }
    }


    private void UpdateColorImage()
    {
        colorImage.color = GlobalVars.localPlacer.GetArtColor();
    }

}
