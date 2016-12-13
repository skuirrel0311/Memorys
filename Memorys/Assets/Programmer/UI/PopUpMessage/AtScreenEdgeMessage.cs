using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ターゲットの方向を知らせるUI
public class AtScreenEdgeMessage : PopUpMessage
{
    //ターゲットの位置
    public Transform targetTransform;

    //スクリーンの中心座標
    protected Vector2 screenOrigin;

    public override void Start()
    {
        base.Start();
        
        screenOrigin = new Vector2(canvasRect.sizeDelta.x * 0.5f, canvasRect.sizeDelta.y * 0.5f);
    }

    public override void DrawMessage()
    {
        if (messagePrefab == null) return;
        messagePrefab.gameObject.SetActive(IsViewMessage);

        if (!IsViewMessage) return;
        if (targetTransform == null) return;

        messagePrefab.rectTransform.anchoredPosition = GetEdgePosition(targetTransform.position) - screenOrigin;
    }

    //指定された方向のエッジ(画面端)の座標を返す。
    protected Vector2 GetEdgePosition(Vector3 targetPosition)
    {
        Vector2 vec = ToEdgeVector(targetPosition).normalized;
        Vector2 temp = screenOrigin;
        while (true)
        {
            temp += vec;
            //if (IsOutScreen(temp)) return temp -= vec * 100;
            if (IsOutScreen(temp)) break;
        }
        Vector2 messageSize = messagePrefab.GetComponent<RectTransform>().sizeDelta * 0.5f;

        Vector2 max = new Vector2(temp.x + (messageSize.x * 0.5f), temp.y + (messageSize.y * 0.5f));
        if (IsOutScreen(max))
        {
            vec = VectorForInScreen(max);
        }
        temp += vec;

        vec = Vector2.zero;
        Vector2 min = new Vector2(temp.x - (messageSize.x * 0.5f), temp.y - (messageSize.y * 0.5f));
        if (IsOutScreen(min)) vec = VectorForInScreen(min);
        temp += vec;

        return temp;
    }

    //点を画面内に移動するためのベクトルを返します
    Vector2 VectorForInScreen(Vector2 point)
    {
        Vector2 vec = Vector2.zero;
        if (point.x < 0) vec.x = 0.0f - point.x;
        if (point.y < 0) vec.y = 0.0f - point.y;
        if (point.x > canvasRect.sizeDelta.x) vec.x = canvasRect.sizeDelta.x - point.x;
        if (point.y > canvasRect.sizeDelta.y) vec.y = canvasRect.sizeDelta.y - point.y;

        return vec;

    }

    Vector2 ToEdgeVector(Vector3 targetPosition)
    {
        Quaternion cameraQuate = Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0);
        //カメラに写すのだからカメラ(カメラの所持者)からターゲットへのベクトルを用いる
        Vector3 temp = cameraQuate * (targetPosition - transform.position);
        return new Vector2(temp.x, temp.z);
    }

    bool IsOutScreen(Vector2 pos)
    {
        if (pos.x < 0) return true;
        if (pos.y < 0) return true;
        if (pos.x > canvasRect.sizeDelta.x) return true;
        if (pos.y > canvasRect.sizeDelta.y) return true;

        return false;
    }
}
