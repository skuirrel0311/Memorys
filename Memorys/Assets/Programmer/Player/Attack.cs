using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections;

public class Attack : MonoBehaviour {
    PlayerState m_state;
    GameObject m_Exposion;
    void Start()
    {
        m_Exposion = Resources.Load("ExplosionMobile")as GameObject;
    }
     void OnTriggerEnter(Collider col)
    {
        bool isAttack = PlayerController.I.currentState == PlayerState.Attack;
        //isAttack = isAttack && m_state != PlayerState.Attack;
        //m_state = PlayerController.I.currentState;
        if (!isAttack) return;
        if(col.gameObject.tag=="Enemy")
        {
            BehaviorTree tree = col.gameObject.GetComponent<BehaviorTree>();
            SharedInt hp = (SharedInt)tree.GetVariable("Hp");
            hp.SetValue(hp.Value - 1);
            //エフェクト
            Destroy(GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity), 3);
        }
    }
}
