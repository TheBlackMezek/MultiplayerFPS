using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSound : MonoBehaviour {

    public AudioSource src;

	public void PlaySound(AudioClip clip)
    {
        src.clip = clip;
        src.Play();
        Invoke("Deactivate", clip.length);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
