using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(-0.2f,0.2f,-0.2f);
	}
}
