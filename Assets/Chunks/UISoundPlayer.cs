using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundPlayer : MonoBehaviour {

    public AudioClip normalClick;
    public AudioClip gameStartClick;
    public AudioClip sliderNoise;

    public float normalClickVolume;
    public float sliderNoiseVolume;

    public AudioSource audioSource;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void PlayNormalClick()
    {
        audioSource.PlayOneShot(normalClick, normalClickVolume);
    }

    public void PlayGameStartClick()
    {
        audioSource.PlayOneShot(gameStartClick);
    }

    public void PlaySliderNoise()
    {
        audioSource.PlayOneShot(sliderNoise, sliderNoiseVolume);
    }

}
