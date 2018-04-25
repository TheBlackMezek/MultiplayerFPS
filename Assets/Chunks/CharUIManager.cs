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
    private string charColorPrefName = "LastUsedCharColor";



    private void Start()
    {
        charName.text = PlayerPrefs.GetString(charNamePrefName);

        Color color;
        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(charColorPrefName), out color))
        {
            redSlider.value = color.r;
            greenSlider.value = color.g;
            blueSlider.value = color.b;
        }
        

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
        PlayerPrefs.SetString(charColorPrefName, "#" + ColorUtility.ToHtmlStringRGBA(exampleColor.color));
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
