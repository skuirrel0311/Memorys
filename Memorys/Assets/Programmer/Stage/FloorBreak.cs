using UnityEngine;
using System.Collections;

public class FloorBreak : MonoBehaviour {
   
    
	// Use this for initialization
	void Start ()
    {
        Rigidbody[] rigs = transform.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigs.Length; i++)
        {
            Vector3 vec = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            rigs[i].AddRelativeTorque(vec*10.0f);
        }
        Destroy(gameObject,3.0f);
	}
}
