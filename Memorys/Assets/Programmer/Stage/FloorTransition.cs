using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum FloorState
{
    NORMAL, HEGHT, LOW
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
    }

    void Update()
    {
        if (!isTransition) return;

        m_Timer += Time.deltaTime;
        if (m_Timer > 5.0f)
        {
            StartCoroutine("FloorMove");
        }

    }

    private void SetFloorState()
    {
        if (m_FloorState == FloorState.NORMAL)
        {
            int r = Random.Range(0, 2);
            r++;
            m_FloorState = (FloorState)r;
        }
        else
        {
            m_FloorState = FloorState.NORMAL;
        }
    }

    public void FloorTrans()
    {
        if (isTransition) return;
        transform.DOShakePosition(5.0f,0.1f,10,90.0f,false,false);
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
