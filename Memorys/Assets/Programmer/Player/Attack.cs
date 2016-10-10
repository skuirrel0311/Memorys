using UnityEngine;
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
            //todo:当たった敵にダメージ
            Destroy(col.gameObject);

            //エフェクト
            Destroy(GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity), 3);
        }
    }
}
