using UnityEngine;
using System.Collections.Generic;

public class FusenSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject fusen;
    [SerializeField]
    Vector3 LeftAnchor = new Vector3(0,0,0);
    [SerializeField]
    Vector3 RightAnchor = new Vector3(0,0,0);
    [SerializeField]
    int maxStage=1;

    List<GameObject> fusens;
	// Use this for initialization
	void Start ()
    {
        fusens = new List<GameObject>();
	    for(int i = 0;i < maxStage;i++)
        {
            fusens.Add(GameObject.Instantiate(fusen,RightAnchor +  new Vector3(0.0f,-0.006f*i,-0.05f*i),Quaternion.identity) as GameObject);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
