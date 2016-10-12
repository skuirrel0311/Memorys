using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;


public class ShotManager : MonoBehaviour
{
    List<Bullet> bulletList;

    [SerializeField]
    GameObject bulletPrefab = null;
    [SerializeField]
    float bulletSpeed = 5.0f;
    [SerializeField]
    GameObject shotPosition = null;


    public void Shot()
    {
        GameObject g = (GameObject)Instantiate(bulletPrefab,shotPosition.transform.position, shotPosition.transform.rotation);
        g.GetComponent<Bullet>().Shot(bulletSpeed);
        Destroy(g, 3.0f);
    }
}
