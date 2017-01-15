using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStop : MonoBehaviour
{

    [SerializeField]
    string[] StopSoundNames;

    [SerializeField]
    float duration =0.1f;

    void OnDestroy()
    {
        int d = (int)(duration *1f);
        for (int i = 0; i < StopSoundNames.Length; i++)
        {
            AkSoundEngine.ExecuteActionOnEvent(StopSoundNames[i], AkActionOnEventType.AkActionOnEventType_Stop);
        }
    }
}
