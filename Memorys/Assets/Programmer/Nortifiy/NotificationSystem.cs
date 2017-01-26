using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotificationSystem : MonoBehaviour {

    public static NotificationSystem I;

    [SerializeField]
    private GameObject m_NotificationImage;
    [SerializeField]
    private Text m_text;
    private float m_defaultFontSize;

	// Use this for initialization
	void Awake ()
    {
        I = this;
        m_defaultFontSize = m_text.fontSize;
	}

    public void Indication(string message,float fontsize = 30)
    {
        m_NotificationImage.SetActive(true);
        m_text.fontSize = (int)fontsize;
        m_text.text = message;
        StartCoroutine("TimeOut");
        //TimeOut().MoveNext();
    }

    IEnumerator TimeOut()
    {
        float time=0.0f;
        Image img = m_NotificationImage.GetComponent<Image>();
        img.color = Color.clear;
        m_text.color = Color.white;
        while (true)
        {
            time += Time.deltaTime;
            if (time <= 0.7f)
            {
                img.color += Color.white * Time.deltaTime;
            }
            else if (time > 2.3f)
            {
                img.color -= Color.white * Time.deltaTime;
                m_text.color -= Color.white * Time.deltaTime;
            }

            if(time>=3.0f)
            {
                break;
            }
            yield return null;
        }
        m_NotificationImage.SetActive(false);
        Debug.Log("TimeOutBreak");
    }
}
