using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;


public class ShotManager : MonoBehaviour
{
    List<Bullet> bulletList;

    [SerializeField]
    GameObject bulletPrefab = null;
    [SerializeField]
    int bulletNum = 10;
    [SerializeField]
    float bulletSpeed = 5.0f;

    int bulletIndex;

    void Start()
    {
        bulletIndex = 0;
        bulletList = new List<Bullet>();
        for(int i = 0;i < bulletNum;i++)
        {
            bulletList.Add(Instantiate(bulletPrefab).GetComponent<Bullet>());
            bulletList[i].gameObject.SetActive(false);
        }
    }

    public void Shot()
    {
        bulletList[bulletIndex].gameObject.SetActive(true);
        bulletList[bulletIndex].Shot(transform, bulletSpeed);

        //カウントアップ
        bulletIndex = (bulletIndex + 1) >= bulletNum ? 0 : (bulletIndex + 1);
    }
}
