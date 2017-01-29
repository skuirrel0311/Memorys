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

    //回ったあとに出てくるコリジョン
    [SerializeField]
    List<GameObject> atRotatedEnableCollisionList = new List<GameObject>();

    //回ったあとに消えるコリジョン
    [SerializeField]
    List<GameObject> atRotatedDesableCollisionList = new List<GameObject>();

    /// <summary>
    /// 回っている時に出てくるコリジョン
    /// </summary>
    [SerializeField]
    List<GameObject> atRotatingCollisionList = new List<GameObject>();

    [SerializeField]
    List<GameObject> onPlayerDesableCollisionList = new List<GameObject>();

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

            if (reverseCount != 0)
            {
                rotateCount++;
                if (rotateCount >= reverseCount)
                {
                    isReverse = !isReverse;
                    rotateCount = 0;
                }
            }

            if (!isReverse)
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

        for (int i = 0; i < atRotatingCollisionList.Count; i++)
        {
            atRotatingCollisionList[i].SetActive(true);
        }

        for (int i = 0; i < atRotatedEnableCollisionList.Count; i++)
        {
            atRotatedEnableCollisionList[i].SetActive(!atRotatedEnableCollisionList[i].activeSelf);
        }

        for (int i = 0; i < atRotatedDesableCollisionList.Count; i++)
        {
            //tempは指定された時だけtrueになる
            bool temp = atRotatedDesableCollisionList[i].name == "Collision" + (GameManager.I.m_GameEnd.m_destoryCancelCount).ToString();

            //アクティブをTrueにするのは回り始めたとき
            if (temp == false) atRotatedDesableCollisionList[i].SetActive(!temp);
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

        for (int i = 0; i < atRotatedDesableCollisionList.Count; i++)
        {
            //tempは指定された時だけtrueになる
            bool temp = atRotatedDesableCollisionList[i].name == "Collision" + (GameManager.I.m_GameEnd.m_destoryCancelCount - 1).ToString();

            //アクティブを切るのは回り終わったとき
            if (temp == true) atRotatedDesableCollisionList[i].SetActive(!temp);
        }

        if (onPlayer)
        {
            for (int i = 0; i < onPlayerDesableCollisionList.Count; i++)
            {
                onPlayerDesableCollisionList[i].SetActive(true);
            }

            player.GetComponent<PlayerController>().IsOnRotateIsrand = false;
        }

        for (int i = 0; i < atRotatingCollisionList.Count; i++)
        {
            atRotatingCollisionList[i].SetActive(false);
        }

        onPlayer = false;
    }

    void OnCollisionStay(Collision col)
    {
        if (!isWorkCoroutine) return;
        if (onPlayer) return;
        if (col.gameObject.tag != "Player") return;

        player = col.transform;
        if (player.GetComponent<PlayerController>().IsOnRotateIsrand) return;

        onPlayer = true;
        player.GetComponent<PlayerController>().IsOnRotateIsrand = true;
        Collider[] cols = Physics.OverlapSphere(player.position + Vector3.down, 0.5f);

        if (cols == null) return;

        if (cols[0].tag == "Wall") return;


        player.parent = cols[0].transform;

        for (int i = 0; i < onPlayerDesableCollisionList.Count; i++)
        {
            onPlayerDesableCollisionList[i].SetActive(false);
        }
    }
}
