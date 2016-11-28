using UnityEngine;
using System.Collections;

public class PlayerFootSteps : MonoBehaviour
{
    static bool isPlay;
    static float timer;
    void Start()
    {
        isPlay = false;
    }
    void Update()
    {
        if (isPlay)
        {
            timer += Time.deltaTime;
            if (timer >= 0.4f)
            {
                isPlay = false;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (PlayerController.I.currentState != PlayerState.Move) return;
        if (isPlay) return;
        Debug.Log(isPlay);
        Debug.Log(timer);
        if (col.gameObject.tag!="Player")
        {
            AkSoundEngine.PostEvent("Footsteps",gameObject);
            isPlay = true;
            timer = 0.0f;
        }
    }
}
