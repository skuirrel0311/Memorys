using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotificationSystem : MonoBehaviour {

    public static NotificationSystem I;

    [SerializeField]
    private GameObject m_NotificationImage;
    [SerializeField]
    private Text m_text;

	// Use this for initialization
	void Awake ()
    {
        I = this;
	}

    public void Indication(string message)
    {
        m_NotificationImage.SetActive(true);
        m_text.text = message;
        StartCoroutine("TimeOut");
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
                img.color += Color.black * Time.deltaTime;
            }
            else if (time > 2.3f)
            {
                img.color -= Color.black * Time.deltaTime;
                m_text.color -= Color.white * Time.deltaTime;
            }

            if(time>=3.0f)
            {
                break;
            }
            yield return null;
        }
        m_NotificationImage.SetActive(false);
        yield return null;
    }

}
