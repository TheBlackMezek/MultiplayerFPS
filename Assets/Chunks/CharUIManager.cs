using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharUIManager : MonoBehaviour {

    public Image exampleColor;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public InputField charName;

    private string charNamePrefName = "LastUsedCharName";



    private void Start()
    {
        charName.text = PlayerPrefs.GetString(charNamePrefName);
        UpdateExampleColor();
        UpdateCharName();
        if(charName.text == "")
        {
            charName.text = "Player";
        }
    }

    public void UpdateExampleColor()
    {
        exampleColor.color = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        NetBridge.Instance.localPlayerColor = exampleColor.color;
    }

    public void UpdateCharName()
    {
        if(charName.text == "")
        {
            NetBridge.Instance.avatarName = "Player";
        }
        else
        {
            NetBridge.Instance.avatarName = charName.text;
        }
        PlayerPrefs.SetString(charNamePrefName, NetBridge.Instance.avatarName);
    }

}
