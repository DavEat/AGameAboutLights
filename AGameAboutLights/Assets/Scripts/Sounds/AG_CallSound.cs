using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_CallSound : AG_Singleton<AG_CallSound> {

    public AudioSource buttonPress;
    public AudioClip buttonPressSrc;
    public AudioSource music;

    public void PlayButtonPress()
    {
        buttonPress.PlayOneShot(buttonPressSrc);
    }
}
