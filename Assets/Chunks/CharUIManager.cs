using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharUIManager : MonoBehaviour {

    public Image exampleColor;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;



    private void Start()
    {
        UpdateExampleColor();
    }

    public void UpdateExampleColor()
    {
        exampleColor.color = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        NetBridge.Instance.localPlayerColor = exampleColor.color;
    }

}
