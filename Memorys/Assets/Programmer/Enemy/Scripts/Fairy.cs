using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class Fairy : MonoBehaviour
{
    //警戒しているか？
    public bool IsWarning { get; private set; }
    bool oldIsWarning;

    //見失ったか？
    bool IsLostTarget;
    Vector3 lostPosition;

    //警戒度(どの程度警戒しているか)
    //public float Alertness { get; private set; }
    public float Alertness;

    BehaviorTree m_tree;
    PlayerController player;
    
    void Start()
    {
        m_tree = GetComponent<BehaviorTree>();
        player = PlayerController.I;

        IsWarning = false;
        Alertness = 0.0f;
        IsLostTarget = false;
        lostPosition = Vector3.zero;
    }

    void Update()
    {
        //警戒度がたまりやすくする
        if ((bool)m_tree.GetVariable("IsSeePlayer").GetValue())
            Alertness += Time.deltaTime * 3.0f;
        else
            Alertness -= Time.deltaTime;

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);

        //一度警戒すると警戒が解けるまでに時間がかかる
        if (IsWarning)
            IsWarning = Alertness > 0.1f;
        else
            IsWarning = Alertness > 1.5f;

        //見失ったか？
        if (IsWarning == false && oldIsWarning == true)
        {
            IsLostTarget = true;
            lostPosition = player.transform.position;
        }

        if (IsWarning == true)
            IsLostTarget = false;

        oldIsWarning = IsWarning;
    }

    public void GetTragetPosition()
    {
        if (IsLostTarget)
            m_tree.GetVariable("TargetPosition").SetValue(player.transform.position);
        else
            m_tree.GetVariable("TargetPosition").SetValue(lostPosition);
    }

    //プレイヤーの乗っている床を１段上げる
    public void Magic()
    {
        FloorTransition floor = GetPlayerUnderFloor();
        if(floor == null)
        {
            Debug.Log("floor is null");
            return;
        }

        if (floor.isTransition) return;
        floor.Raise();
    }

    FloorTransition GetPlayerUnderFloor()
    {
        Collider[] cols = Physics.OverlapSphere(player.transform.position + Vector3.down, 1.0f);

        for(int i = 0;i<cols.Length;i++)
        {
            FloorTransition temp = cols[i].GetComponent<FloorTransition>();

            if (temp != null) return temp;
        }

        return null;
    }
}
