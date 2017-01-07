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
        lineRenderer = arrow.GetComponentInChildren<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.Log("lineRenderer is null");
            return;
        }
        lineRenderer.gameObject.SetActive(false);
        base.Start();
    }

    public override void Update()
    {
        if (lineRenderer == null) return;
        if(CanGameEnd)
        {
            //arrowの向きを決める。
            Vector3 vec = Vector3.Normalize(transform.position - arrow.position);

            //lineを描画する。
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.SetPosition(0, arrow.position + (vec * 0.3f));
            lineRenderer.SetPosition(1, arrow.position + (vec * 3.0f));
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
        //IsViewMessage = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag != "Player") return;


    }
}
