using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointGauge : MonoBehaviour
{
    int maxValue;
    int value;

    public int Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            IsCalc = true;
        }
    }
    bool IsCalc = false;
    [SerializeField]
    Image pointImagePrefab = null;
    [SerializeField]
    Image[] pointImages;

    [SerializeField]
    Vector2 origin = Vector2.zero;

    public void Update()
    {
        if (!IsCalc) return;
        for (int i = 0; i < pointImages.Length; i++)
        {
            if (i >= value)
                pointImages[i].gameObject.SetActive(false);
            else
                pointImages[i].gameObject.SetActive(true);
        }
    }
}