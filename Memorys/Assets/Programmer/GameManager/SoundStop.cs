using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStop : MonoBehaviour
{

    [SerializeField]
    string[] StopSoundNames;

    void OnDestroy()
    {
        for (int i = 0; i < StopSoundNames.Length; i++)
        {
            AkSoundEngine.ExecuteActionOnEvent(StopSoundNames[i], AkActionOnEventType.AkActionOnEventType_Stop);
        }
    }
}
