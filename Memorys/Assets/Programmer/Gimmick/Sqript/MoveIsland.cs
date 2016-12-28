using UnityEngine;

//このスクリプトをアタッチするオブジェクトにrigid body が必要
//床の範囲のトリガーもアタッチが必要
public class MoveIsland : MonoBehaviour
{
    [SerializeField]
    Transform[] wayPoints = null;

    [SerializeField]
    float speed = 0.05f;

    Vector3 velocity;
    Vector3 nextPosition;

    int index;
    //行きか帰りか
    bool isReturn;
    

    void Start()
    {
        index = 0;
        nextPosition = wayPoints[index].position;

        velocity = Vector3.Normalize(nextPosition - transform.position) * speed;
    }

    void Update()
    {
        if (Vector3.Distance(nextPosition, transform.position) < 0.1f)
        {
            index = isReturn ? index - 1 : index + 1;

            //インデックスが範囲外になった
            if (index > wayPoints.Length - 1 || index < 0)
            {
                //行き帰りを反転させる
                isReturn = !isReturn;
                //nextPositionの更新は次のフレームでやる
            }
            else
            {
                nextPosition = wayPoints[index].position;
                velocity = Vector3.Normalize(nextPosition - transform.position) * speed;
            }
        }

        transform.Translate(velocity, Space.World);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.parent = null;
        }
    }
}
