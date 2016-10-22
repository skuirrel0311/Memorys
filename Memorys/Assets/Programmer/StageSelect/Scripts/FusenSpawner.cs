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


    public bool isSetRoot = false;

    // Use this for initialization
    void Start ()
    {
        fusens = new List<GameObject>();

        for (int i = 0;i < maxStage;i++)
        {
            fusens.Add(GameObject.Instantiate(Fusen,RightAnchor +  new Vector3(0.0f,-0.006f*i,-0.05f*i),Quaternion.identity) as GameObject);
        }
	}

    void PositionSet()
    {
        for (int i = 0; i < maxStage; i++)
        {
            int index = i - (GetComponent<SelectManager>().m_SelectNumber-1);
            index = (int)Mathf.Max(0.0f,index);
            if (GetComponent<SelectManager>().m_SelectNumber - 1 <= i)
            {
                fusens[i].transform.position = RightAnchor + new Vector3(0.0f, -0.006f * index, -0.05f * i);
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
        AnimatorStateInfo currentState = GetComponent<SelectManager>().m_BookAnim.GetCurrentAnimatorStateInfo(0);
        float duration = currentState.length;
        yield return  new  WaitForSeconds(0.34f);
        FusenRoot.transform.DetachChildren();
        PositionSet();
        GetComponent<SelectManager>().UpdateTexture();
        isSetRoot = false;
        yield return null;
    }

    public void SetAnimationRoot(int index,bool isRight)
    {
        if (isSetRoot) return;

        if (!isRight)
        {
            fusens[index].transform.position = RightAnchor+ new Vector3(0.0f, -0.006f * index, -0.05f * index);
        }

        isSetRoot = true;
        fusens[index].transform.parent = FusenRoot.transform;
        StartCoroutine("ParentDetach");
    }
}
