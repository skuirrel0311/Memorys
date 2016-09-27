using UnityEngine;
using System.Collections;

public class CameraContoller : MonoBehaviour
{
    [SerializeField]
    GameObject player = null;
    [SerializeField]
    float distance = 7;    //カメラとプレイヤーの距離
    [SerializeField]
    float height = 3;
    [SerializeField]
    float rotationSpeed = 250;
    
    PlayerController controller;
    float rotation;

    void Start()
    {
        rotation = 270;
        controller = player.GetComponent<PlayerController>();
        transform.position = GetCameraPositoin(rotation);
    }
    
    void Update()
    {
        float leftStickX = Input.GetAxis("Horizontal2") * -1;
        //Debug.Log("leftStickX = " + leftStickX);
        
        rotation += leftStickX * rotationSpeed * Time.deltaTime;
        transform.position = GetCameraPositoin(rotation);
        transform.LookAt(player.transform.position + Vector3.up);
    }

    //指定された角度のときのカメラの位置を返す
    Vector3 GetCameraPositoin(float rotation)
    {
        Vector3 cameraPosition = player.transform.position;
        float temp = rotation * Mathf.Deg2Rad;
        cameraPosition.x += Mathf.Cos(temp) * distance;
        cameraPosition.z += Mathf.Sin(temp) * distance;
        cameraPosition.y += height;
        return cameraPosition;
    }
}
