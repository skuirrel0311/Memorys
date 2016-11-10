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
    Image[] pointImages;

    [SerializeField]
    Vector2 origin = Vector2.zero;
    [SerializeField]
    Vector2 pointDistance = new Vector2(50,0);

    public void Initialize(int maxValue)
    {
        this.maxValue = maxValue;
        Value = maxValue;

        pointImages = new Image[maxValue];
        for(int i = 0;i < maxValue;i++)
        {
            pointImages[i] = (Image)Instantiate(pointImagePrefab, transform);
            pointImages[i].rectTransform.anchoredPosition = origin + (pointDistance * i);
        }
    }

    public void Update()
    {
        if (!IsCalc) return;
        for (int i = 0; i < maxValue; i++)
        {
            if (i >= value)
                pointImages[i].gameObject.SetActive(false);
            else
                pointImages[i].gameObject.SetActive(true);

        }
    }
}
