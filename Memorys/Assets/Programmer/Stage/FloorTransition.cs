using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum FloorState
{
     VLOW, LOW, NORMAL, HEGHT,VHEGHT
}

public class FloorTransition : MonoBehaviour
{

    private const float MaxHeght = 2.0f;
    private const float MaxLow = -2.0f;
    private bool isTransition = false;
    private float m_Timer;
    private FloorState m_FloorState;


    // Use this for initialization
    void Start()
    {
        m_Timer = 0;
        m_FloorState = (FloorState)Random.Range(1, 4);
        if (m_FloorState == FloorState.NORMAL)
        {
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        }
        else if (m_FloorState == FloorState.HEGHT)
        {
            transform.position = new Vector3(transform.position.x, MaxHeght, transform.position.z);
        }
        else if(m_FloorState == FloorState.VHEGHT)
        {
            transform.position = new Vector3(transform.position.x, MaxHeght*2, transform.position.z);
        }
        else if (m_FloorState == FloorState.LOW)
        {
            transform.position = new Vector3(transform.position.x,MaxLow, transform.position.z);
        }
        else if (m_FloorState == FloorState.VLOW)
        {
            transform.position = new Vector3(transform.position.x, MaxLow*2.0f, transform.position.z);
        }
    }

    void Update()
    {
        if (!isTransition) return;

        m_Timer += Time.deltaTime;
        if (m_Timer > 2.0f)
        {
            StartCoroutine("FloorMove");
        }

    }

    private void SetFloorState()
    {
        if (m_FloorState == FloorState.VHEGHT)
        {
            m_FloorState --;
        }
        else if (m_FloorState == FloorState.LOW)
        {
            m_FloorState++;
        }
        else
        {
            int r = Random.Range(0, 2);
            r *= 2;
            r -= 1;
            m_FloorState = m_FloorState+r;
        }
    }

    public void FloorTrans()
    {
        if (isTransition) return;
        transform.DOShakePosition(2.0f,0.025f,25,90.0f,false,false);
        isTransition = true;
        m_Timer = 0.0f;
        SetFloorState();
        //Renderer r = GetComponent<Renderer>();
        //r.material.EnableKeyword("_EMISSION");
        //r.material.SetColor("_EmissionColor", Color.red);
    }

    //座標を変化させる
    private IEnumerator FloorMove()
    {
        float t = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.zero; 

        if (m_FloorState == FloorState.NORMAL)
        {
            targetPos = new Vector3(startPos.x, 0.0f, startPos.z);
        }
        else if (m_FloorState == FloorState.HEGHT)
        {
            targetPos = new Vector3(startPos.x, MaxHeght, startPos.z);
        }
        else if (m_FloorState == FloorState.LOW)
        {
            targetPos = new Vector3(startPos.x, MaxLow, startPos.z);
        }
        else if (m_FloorState == FloorState.VHEGHT)
        {
            targetPos = new Vector3(startPos.x, MaxHeght*2.0f, startPos.z);
        }
        else if (m_FloorState == FloorState.VLOW)
        {
            targetPos = new Vector3(startPos.x, MaxLow*2.0f, startPos.z);
        }

        while (true)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            if (t > 1.0f) break;
            yield return null;
        }
        //Renderer r = GetComponent<Renderer>();
        //r.material.EnableKeyword("_EMISSION");
        //r.material.SetColor("_EmissionColor", Color.black);
        isTransition = false;
    }
}
