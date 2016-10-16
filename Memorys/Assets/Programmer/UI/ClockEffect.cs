using UnityEngine;
using System.Collections;

public class ClockEffect : MonoBehaviour {

    private float timer = 0.0f;
    private float angle = 0.0f;
    private RectTransform rect;

	// Use this for initialization
	void Start ()
    {
        timer = 0.0f;
        angle = 0.0f;
        rect= transform.GetChild(0).GetComponent<RectTransform>();
        rect.rotation = Quaternion.identity;
	}
    void OnEnable()
    {
        timer = 0.0f;
        angle = 0.0f;
        rect = transform.GetChild(0).GetComponent<RectTransform>();
        rect.rotation = Quaternion.identity;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        rect.rotation = Quaternion.Euler(0.0f,0.0f,-(timer/2.0f)*360.0f);
        if (timer > 2.0f) gameObject.SetActive(false);
	}
}
