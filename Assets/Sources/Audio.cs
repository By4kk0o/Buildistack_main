using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioListener audioListener;
    public List<AudioSource> audioSources;


    public void OnActive()
    {
        AudioListener.pause = true;
    }
    public void OnUnactive()
    {
        AudioListener.pause = false;
    }
}
