using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum FloorState
{
    VLOW, LOW, NORMAL, HEGHT, VHEGHT
}

public class FloorTransition : MonoBehaviour
{
    private float MaxHeight = 2.0f;
    private float MaxLow = -2.0f;
    private float BaseHeight = 0.0f;
    public bool isTransition = false;
    bool oldIsTransition = false;
    private float m_Timer;
    private FloorState m_FloorState;
    private IEnumerator e_FloorMove;

    Coroutine coroutine;

    Renderer[] m_renderers;

    MaterialPropertyBlock block;

    // Use this for initialization
    private void Awake()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void Start()
    {
        BaseHeight = transform.position.y;
        MaxHeight = BaseHeight + 2.0f;
        MaxLow = BaseHeight - 2.0f;

        m_Timer = 0;

        if (GameManager.I.IsFlat) return;

        m_FloorState = (FloorState)Random.Range(1, 4);
        transform.position = GetTargetPosition(m_FloorState, transform.position);
    }

    void Update()
    {
        //コルーチンが終わったタイミング
        if (isTransition == false && oldIsTransition == true)
        {
            StopCoroutine(coroutine);
            //参照を外す
            coroutine = null;
        }


        oldIsTransition = isTransition;

        //m_Timer += Time.deltaTime;
        //if (m_Timer > 2.0f)
        //{
        //    e_FloorMove.MoveNext();
        //}

    }

    private void SetFloorState()
    {
        if (m_FloorState == FloorState.VHEGHT)
        {
            m_FloorState--;
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
            m_FloorState = m_FloorState + r;
        }
    }

    public void FloorTrans()
    {
        if (isTransition) return;
        //transform.DOShakePosition(2.0f, 0.025f, 25, 90.0f, false, false);
        isTransition = true;
        m_Timer = 0.0f;


        //e_FloorMove = FloorMove();
        SetFloorState();
        coroutine = StartCoroutine("FloorMove");


    }

    void ChangeColor(Color color)
    {
        for (int i =0; i < m_renderers.Length;i++)
        {
            block.SetColor("_EmitColor", color);
            m_renderers[i].SetPropertyBlock(block);
        }
    }

    //座標を変化させる
    private IEnumerator FloorMove()
    {
        float t = 0.0f;
        float startY = transform.position.y;
        float targetY = GetTargetPosition(m_FloorState, transform.position).y;
        float currentY = startY;

        Color targetColor = Color.black;
        if (startY < targetY)
        {
            targetColor = Color.red;
        }
        else
        {
            targetColor = Color.blue;
        }

        transform.DOShakePosition(2.0f, 0.025f, 25, 90.0f, false, false);
        // yield return new WaitForSeconds(2.0f);

        while (true)
        {
            t += Time.deltaTime;
            ChangeColor(targetColor * t * 0.5f);
            if (t > 2.0f) break;
            yield return null;
        }
        t = 0.0f;
        while (true)
        {
            t += Time.deltaTime;
            currentY = BehaviorDesigner.Runtime.Tasks.Movement.MovementUtility.FloatLerp(startY, targetY, t);
            transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
            if (t > 1.0f) break;
            yield return null;
        }
        StartCoroutine(ColorReset(targetColor,0.3f));
        isTransition = false;
    }

    IEnumerator ColorReset(Color baseColor, float duration)
    {
        float t = 0.0f;
        while(true)
        {
            t += Time.deltaTime;
            ChangeColor(baseColor*(1-t/duration));
            if (t>=duration) break;
            yield return null;
        }
        ChangeColor(Color.black);
    }


    Vector3 GetTargetPosition(FloorState state, Vector3 startPos)
    {
        if (state == FloorState.NORMAL)
        {
            return new Vector3(startPos.x, BaseHeight, startPos.z);
        }
        else if (state == FloorState.HEGHT)
        {
            return new Vector3(startPos.x, MaxHeight, startPos.z);
        }
        else if (state == FloorState.LOW)
        {
            return new Vector3(startPos.x, MaxLow, startPos.z);
        }
        else if (state == FloorState.VHEGHT)
        {
            return new Vector3(startPos.x, MaxHeight + 2.0f, startPos.z);
        }
        else if (state == FloorState.VLOW)
        {
            return new Vector3(startPos.x, MaxLow - 2.0f, startPos.z);
        }

        return Vector3.zero;
    }

    //上げる
    public void Raise()
    {
        if (isTransition) return;

        int temp = (int)m_FloorState;
        temp++;

        temp = Mathf.Clamp(temp, (int)FloorState.VLOW, (int)FloorState.VHEGHT);

        if (temp != (int)m_FloorState)
        {
            isTransition = true;
            m_FloorState = (FloorState)temp;
            coroutine = StartCoroutine("FloorMove");
        }
    }
}
