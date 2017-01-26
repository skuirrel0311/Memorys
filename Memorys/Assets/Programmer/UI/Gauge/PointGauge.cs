using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointGauge : MonoBehaviour
{
    int maxValue;
    int value=6;

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
        if(value<=2)
        {
            GetComponent<Image>().color = Color.Lerp(Color.white * 0.8f, Color.red * 0.8f,Mathf.Abs(Mathf.Sin(Time.time*3.5f)));
        }

        if (!IsCalc) return;
        for (int i = 0; i < pointImages.Length; i++)
        {
            if (i >= value)
            {
                if (pointImages[i].color== Color.white)
                {
                    pointImages[i].color = Color.red;
                    StartCoroutine(TkUtils.Delay(1.0f,ChangeImageColor, pointImages[i], Color.black * 0.7f));
                }
            }
            else if(pointImages[i].color != Color.white)
            {
                pointImages[i].color = Color.white;
            }
        }
    }

    void ChangeImageColor(Image image,Color targetColor)
    {
        Debug.Log("ChangeImageColor");
        StartCoroutine(TkUtils.DoColor(0.3f, image, targetColor));
    }
}