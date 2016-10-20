using UnityEngine;
using System.Collections;

public class MirrorWall : MonoBehaviour {
    
    //通せんぼする方向
    [SerializeField]
    bool IsGuardX;
    [SerializeField]
    bool IsGuardY;
    [SerializeField]
    bool IsGuardZ;

    BoxCollider frontCollider;

    void Start () {
        frontCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //後ろから来た場合は通せんぼしない
    bool IsInFront()
    {
        return false;
    }
}
