using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MapController : MonoBehaviour
{


    GameObject reducionMap;
    GameObject expansionMap; 

    void Start()
    {
        reducionMap = transform.GetChild(0).gameObject;
        expansionMap = transform.GetChild(1).gameObject;

        reducionMap.SetActive(true);

    }

    void Update()
    {
   
        if(MyInputManager.GetButtonDown(MyInputManager.Button.RightShoulder))
        {
            //切り替え
            bool temp = reducionMap.activeSelf;
            reducionMap.SetActive(expansionMap.activeSelf);
            expansionMap.SetActive(temp);
        }
    }
}
