using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections;

public class Attack : MonoBehaviour {
    PlayerState m_state;
    GameObject m_Exposion;

     void OnTriggerEnter(Collider col)
    {
        bool isAttack = PlayerController.I.currentState == PlayerState.Attack;
        //isAttack = isAttack && m_state != PlayerState.Attack;
        //m_state = PlayerController.I.currentState;
        if (!isAttack) return;
        if(col.gameObject.tag=="Target")
        {
            GameManager.I.DestroyCancel();
            //エフェクト
            GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity);
        }
    }
}
