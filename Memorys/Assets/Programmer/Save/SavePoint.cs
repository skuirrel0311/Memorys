using UnityEngine;
using System.Collections;

public class SavePoint : MonoBehaviour {

    SaveManager m_saveManager;
    int m_pointNumber;
    bool isSaved;

	// Use this for initialization
	void Start ()
    {
        m_saveManager = gameObject.transform.parent.GetComponent<SaveManager>();
        string pointname = gameObject.name;
        m_pointNumber = int.Parse(pointname.Remove(0, 4));

        if (m_saveManager.GetNowPoint()<m_pointNumber)
        {
            isSaved = false;
        }
        else
        {
            isSaved = true;
            GetComponent<Renderer>().material.color = Color.blue;
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (isSaved) return;
        if (col.tag != "Player") return;
        m_saveManager.PointSave(m_pointNumber);
        isSaved = true;
        GetComponent<Renderer>().material.color = Color.blue;
    }
}
