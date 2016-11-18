using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceMessage : PopUpMessage
{
    public float distance = 0.0f;
    public Transform targetTransform;
    Vector2 screenOrigin;

    public override void Start()
    {
        base.Start();

        screenOrigin = new Vector2(canvasRect.sizeDelta.x * 0.5f, canvasRect.sizeDelta.y * 0.5f);
    }

    public override void DrawMessage()
    {
        messagePrefab.gameObject.SetActive(IsViewMessage);

        if (!IsViewMessage) return;

        //表示する位置を計算する
        Quaternion cameraQuate = Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0);
        Vector3 temp = cameraQuate * (targetTransform.position - transform.position);
        Vector2 vec = new Vector2(temp.x, temp.z);
        messagePrefab.rectTransform.anchoredPosition = GetEdgePosition(vec) - screenOrigin;

        //距離を小数点2桁まで表示する
        messagePrefab.GetComponentInChildren<Text>().text = distance.ToString("F2") + "m";
    }

    //指定された方向のエッジ(画面端)の座標を返す。
    Vector2 GetEdgePosition(Vector2 vec)
    {
        Vector2 temp = screenOrigin;
        vec = vec.normalized;
        while (true)
        {
            temp += vec;
            if (IsOutScreen(temp)) return temp -= vec * 100;
        }
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
