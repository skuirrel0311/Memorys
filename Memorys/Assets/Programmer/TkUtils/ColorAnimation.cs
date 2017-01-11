using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorAnimation : MonoBehaviour {

    Image m_image;
    bool isAlphaLoop = true;

	// Use this for initialization
	void Start () {
        m_image=GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isAlphaLoop)
        {
            if (m_image != null)
                m_image=GetComponent<Image>();
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b,Mathf.Abs(Mathf.Cos(Time.time))+0.3f);
        }
	}
}
