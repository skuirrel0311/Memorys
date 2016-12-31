using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks.Movement;

//スイッチを押すたびに島が回転します
public class RotateIsland : MonoBehaviour
{
    [SerializeField]
    float plusValue = 90.0f;
    [SerializeField]
    float rotateTime = 7.0f;

    float currentRotateY = 0.0f;
    float targetRotateY = 0.0f;

    Coroutine coroutine;
    bool isWorkCoroutine = false;

    bool onPlayer = false;
    Transform player;

    void Start()
    {
        GameManager.I.OnPushSwitch += () =>
        {
            if (isWorkCoroutine)
            {
                StopCoroutine(coroutine);
                currentRotateY = transform.localEulerAngles.y;
                targetRotateY = targetRotateY + plusValue;
            }
            else
            {
                currentRotateY = transform.localEulerAngles.y;
                targetRotateY = currentRotateY + plusValue;
            }
            isWorkCoroutine = true;
            coroutine = StartCoroutine("Rotate");
        };
    }

    void Update()
    {

    }

    IEnumerator Rotate()
    {
        float t = 0.0f;
        while (true)
        {
            t += Time.deltaTime;
            
            transform.rotation = Quaternion.Euler(0, MovementUtility.FloatLerp(currentRotateY, targetRotateY, t / rotateTime), 0);
            if (t > rotateTime) break;
            yield return null;
        }
        isWorkCoroutine = false;
        if (onPlayer)
        {
            player.parent = null;
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
