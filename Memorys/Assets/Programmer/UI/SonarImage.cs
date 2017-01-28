using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SonarImage : MonoBehaviour {

    [SerializeField]
    Image[] m_sonerImage = null;

    int point;

	// Use this for initialization
	void Start ()
    {
        point = m_sonerImage.Length;
	}
	
	// Update is called once per frame
	void Update ()
    {
        int pow = PlayerController.I.GetComponent<SoundWaveFinder>().power;
        for (int i = 0; i < m_sonerImage.Length; i++)
        {
            if (i >= pow)
                m_sonerImage[i].gameObject.SetActive(false);
            else
               m_sonerImage[i].gameObject.SetActive(true);
        }
    }
}
