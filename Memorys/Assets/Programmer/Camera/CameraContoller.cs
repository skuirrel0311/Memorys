using UnityEngine;
using System.Collections;

public class CameraContoller : MonoBehaviour
{
    [SerializeField]
    GameObject player = null;
    PlayerController controller;
    Vector3 oldPosition;

    void Start()
    {
        oldPosition = Vector3.zero;
        controller = player.GetComponent<PlayerController>();
    }
    
    void Update()
    {
        Vector3 movement = player.transform.position - oldPosition;
        //movement.y = 0;
        
        //プレイヤーが移動した分だけカメラも移動する
        transform.position += movement;
        
        oldPosition = player.transform.position;
    }
}
