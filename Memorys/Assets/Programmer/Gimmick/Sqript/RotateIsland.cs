using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//スイッチを押すたびに島が回転します
public class RotateIsland : MonoBehaviour
{
    [SerializeField]
    float plusValue = 90.0f;
    [SerializeField]
    float rotateTime = 7.0f;

    float currentRotateY = 0.0f;
    float targetRotateY = 0.0f;

    Rigidbody body;
    Coroutine coroutine;
    bool isWorkCoroutine = false;

    bool onPlayer = false;
    Transform player;

    /// <summary>
    /// 逆回転するまでの回数(0だと逆回転しなくなります)
    /// </summary>
    [SerializeField]
    int reverseCount = 0;
    int rotateCount = 0;

    [SerializeField]
    bool isReverse = false;

    [SerializeField]
    List<GameObject> atRotateCollisionList = new List<GameObject>();

    void Start()
    {
        currentRotateY = transform.localEulerAngles.y;
        targetRotateY = currentRotateY;

        GameManager.I.OnPushSwitch += () =>
        {
            if (isWorkCoroutine)
            {
                StopCoroutine(coroutine);
            }

            if(reverseCount != 0)
            {
                rotateCount++;
                if(rotateCount >= reverseCount)
                {
                    isReverse = !isReverse;
                    rotateCount = 0;
                }
            }

            if(!isReverse)
                targetRotateY = targetRotateY + plusValue;
            else
                targetRotateY = targetRotateY - plusValue;

            isWorkCoroutine = true;
            gameObject.AddComponent<Rigidbody>();
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            coroutine = StartCoroutine("Rotate");
        };
    }

    void Update()
    {

    }

    IEnumerator Rotate()
    {
        float t = 0.0f;
        float startRotateY = currentRotateY;
        
        for(int i = 0;i< atRotateCollisionList.Count;i++)
        {
            atRotateCollisionList[i].SetActive(true);
        }

        while (true)
        {
            t += Time.deltaTime;
            currentRotateY = TkUtils.FloatLerp(startRotateY, targetRotateY, t / rotateTime);
            transform.localRotation = Quaternion.Euler(0, currentRotateY, 0);
            if (t > rotateTime) break;
            yield return null;
        }
        isWorkCoroutine = false;
        Destroy(body);
        if (onPlayer)
        {
            player.parent = null;
        }

        for (int i = 0; i < atRotateCollisionList.Count; i++)
        {
            atRotateCollisionList[i].SetActive(false);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (!isWorkCoroutine) return;
        if (onPlayer) return;
        if (col.gameObject.tag != "Player") return;

        onPlayer = true;
        player = col.transform;

        Collider[] cols = Physics.OverlapSphere(player.position + Vector3.down, 0.5f);

        if (cols == null) return;

        player.parent = cols[0].transform;
    }
}
