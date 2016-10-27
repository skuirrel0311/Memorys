using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    AudioSource audioSource;
    PlayerController controller;

    float max = 0.0f;
    float min = 0.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        //ボリュームは(1 ～ 0) lengthも(1 ～ 0)
        audioSource.volume = MyInputManager.GetAxis(MyInputManager.Axis.LeftStick).magnitude;
    }
}
