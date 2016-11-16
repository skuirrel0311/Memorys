using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CircleMapManager : MapManager
{
    [SerializeField]
    Image wall;
    [SerializeField]
    float radius = 30.0f;
    List<GameObject> objects;
    List<Image> Images;

    public override void Start()
    {
        base.Start();
        List<GameObject> f_Object = new List<GameObject>();
        f_Object.AddRange(GameObject.FindGameObjectsWithTag("FieldObject"));
        objects = new List<GameObject>();
        Images = new List<Image>();
        for (int i = 0; i < f_Object.Count; i++)
        {
            if (f_Object[i].GetComponent<MeshFilter>().sharedMesh.name == "wall")
                objects.Add(f_Object[i]);
        }
        for (int i = 0; i < objects.Count; i++)
        {
            Image item = Instantiate(wall) as Image;
            item.rectTransform.parent = transform;
            Images.Add(item);
        }
    }

    public override void Update()
    {
        //playerImage.rectTransform.localRotation = ConvertMapRotation(playerObj.transform.eulerAngles);
        transform.localRotation = ConvertMapRotation(Camera.main.transform.eulerAngles);
        if (switchObj != null)
        {
            if (!IsNear(switchObj.transform.position, playerObj.transform.position, radius))
            {
                switchImage.gameObject.SetActive(false);
            }
            else
            {
                switchImage.gameObject.SetActive(true);
                switchImage.rectTransform.anchoredPosition = ConvertMapPosition(switchObj.transform.position - playerObj.transform.position);
            }
        }
        else
        {
            switchObj = GameObject.FindGameObjectWithTag("Target");
        }

        for(int i = 0;i< objects.Count;i++)
        {
            if(objects[i]==null)
            {
                Images[i].gameObject.SetActive(false);
                continue;
            }
            if (!IsNear(objects[i].transform.position, playerObj.transform.position, 30.0f))
            {
                //遠かったら表示しない。
                Images[i].gameObject.SetActive(false);
                continue;
            }
            Images[i].gameObject.SetActive(true);
            Images[i].rectTransform.anchoredPosition = ConvertMapPosition(objects[i].transform.position - playerObj.transform.position);
            Images[i].rectTransform.localRotation = ConvertMapRotation(objects[i].transform.eulerAngles - playerObj.transform.position);
        }

        DrawEnemy(playerObj.transform.position);
    }
}
