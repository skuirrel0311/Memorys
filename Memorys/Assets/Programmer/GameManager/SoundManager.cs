using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager I;

    private void Start()
    {
        I = this;
    }

    public static void PlaySound(string str)
    {
        AkSoundEngine.PostEvent(str,I.gameObject);
    }

    private void OnDestroy()
    {
        I = null;
    }
}
