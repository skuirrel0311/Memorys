using UnityEngine;
using System.Collections;

public class RecoveryItem : MonoBehaviour
{
    [SerializeField]
    int recoveryPoint = 3;

    [SerializeField]
    GameObject healEffect = null;
    Transform[] itemEffect;
    [SerializeField]
    float rotationSpeed = 100;

    /// <summary>
    /// 緯度
    /// </summary>
    [SerializeField]
    float latitude = 0;
    /// <summary>
    /// 経度
    /// </summary>
    [SerializeField]
    float longitude = 0;

    [SerializeField]
    bool IsCheap = true;

    void Start()
    {
        itemEffect = new Transform[3];
        itemEffect[0] = transform.GetChild(0);
        itemEffect[1] = transform.GetChild(0).GetChild(0);
        itemEffect[2] = transform.GetChild(0).GetChild(1);

        if(IsCheap)
        {
            Destroy(itemEffect[1].gameObject);
            Destroy(itemEffect[2].gameObject);
        }
    }

    void Update()
    {
        if (IsCheap) return;
        latitude -= Time.deltaTime * rotationSpeed;
        longitude += Time.deltaTime * rotationSpeed;
        itemEffect[1].transform.position = itemEffect[0].position + CameraContoller.SphereCoordinate(longitude, latitude, 0.7f);
        itemEffect[2].transform.position = itemEffect[0].position + CameraContoller.SphereCoordinate(longitude, latitude + 180, 0.7f);
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.tag != "Player") return;

    //    //回復
    //    col.gameObject.GetComponent<PlayerOverlap>().Recovery(recoveryPoint);
    //    GameObject obj = (GameObject)Instantiate(healEffect, transform.position + Vector3.down, transform.rotation);
    //    obj.transform.parent = col.gameObject.transform;
    //    obj.transform.localPosition += Vector3.back * 0.4f;
    //    Destroy(obj, 3.0f);
    //    Destroy(gameObject);
    //}
}
