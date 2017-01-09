using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalObject : PopUpMessage
{
    Transform arrow;
    LineRenderer lineRenderer;

    public override void Start()
    {
        arrow = GameObject.FindGameObjectWithTag("Player").transform.GetChild(2);
        //lineRenderer = arrow.GetComponentInChildren<LineRenderer>();

        if (arrow == null)
        {
            Debug.Log("lineRenderer is null");
            return;
        }
        arrow.gameObject.SetActive(false);
        base.Start();
    }

    public override void Update()
    {
        if (arrow == null) return;
        if(CanGameEnd)
        {
            //arrowの向きを決める。
            Vector3 vec = Vector3.Normalize(transform.position - arrow.position);

            //lineを描画する。
            arrow.gameObject.SetActive(true);
            arrow.rotation =Quaternion.Euler(0, Mathf.Atan2(vec.x,vec.z)*Mathf.Rad2Deg,0);
        }
    }

    bool CanGameEnd
    {
        get{ return GameManager.I.m_GameEnd.m_destoryCancelCount >= GameEnd.c_MaxDestroyCalcel; }
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "Player") return;
        GameManager.I.m_GameEnd.GameClear();
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag != "Player") return;
    }
}
