using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    PlayerState m_state;
     void OnTriggerEnter(Collider col)
    {
        //Debug.Log("ArmHit");
        bool isAttack = PlayerController.I.currentState == PlayerState.Attack;
        //isAttack = isAttack && m_state != PlayerState.Attack;
        //m_state = PlayerController.I.currentState;
        if (!isAttack) return;
        if(col.gameObject.tag=="Enemy")
        {
            //todo:当たった敵にダメージ
            Destroy(col.gameObject);
        }
    }
}
