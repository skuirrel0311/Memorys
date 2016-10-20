using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FusenSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject Fusen;
    [SerializeField]
    GameObject FusenRoot;
    [SerializeField]
    public Vector3 LeftAnchor = new Vector3(0,0,0);
    [SerializeField]
    public Vector3 RightAnchor = new Vector3(0,0,0);
    [SerializeField]
    int maxStage=1;

    [HideInInspector]
    public List<GameObject> fusens;

    bool isSetRoot = false;
    // Use this for initialization
    void Start ()
    {
        fusens = new List<GameObject>();
	    for(int i = 0;i < maxStage;i++)
        {
            fusens.Add(GameObject.Instantiate(Fusen,RightAnchor +  new Vector3(0.0f,-0.006f*i,-0.05f*i),Quaternion.identity) as GameObject);
        }
	}

    void PositionSet()
    {
        for (int i = 0; i < maxStage; i++)
        {
            if (GetComponent<SelectManager>().m_SelectNumber - 1 <= i)
            {
                fusens[i].transform.position = RightAnchor + new Vector3(0.0f, -0.006f * i, -0.05f * i);
            }
            else
            {
                fusens[i].transform.position = LeftAnchor+ new Vector3(0.0f, -0.01f, -0.05f * i);
            }
            fusens[i].transform.rotation = Quaternion.identity;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
    
    IEnumerator ParentDetach()
    {
        if (isSetRoot) yield return null;
        isSetRoot = true;
        yield return  new  WaitForSeconds(1.0f);
        FusenRoot.transform.DetachChildren();
        PositionSet();
        isSetRoot = false;
        yield return null;
    }

    public void SetAnimationRoot(int index)
    {
        fusens[index-1].transform.parent = FusenRoot.transform;
        StartCoroutine("ParentDetach");
    }
}
