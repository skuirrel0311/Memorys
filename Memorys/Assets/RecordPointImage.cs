using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecordPointImage : MonoBehaviour
{

    [SerializeField]
    RecordOfAction m_RecrdOfAction;

    private List<Transform> m_PointImages;

    void UpdateImage()
    {
        for (int i = 0; i < m_PointImages.Count; i++)
        {
            if (i < m_RecrdOfAction.RecordIndex)
            {
                m_PointImages[i].GetComponent<Image>().color = Color.red;
            }
            else
            {
                m_PointImages[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        m_PointImages = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (transform != child)
            {
                //targetはGameObject型
                m_PointImages.Add(child);
            }
        }

        UpdateImage();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImage();
    }

}
