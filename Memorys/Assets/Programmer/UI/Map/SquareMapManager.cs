using UnityEngine;
using System.Collections;

public class SquareMapManager : MapManager
{
    public override void Update()
    {
        playerImage.rectTransform.anchoredPosition = ConvertMapPosition(playerObj.transform.position);
        playerImage.rectTransform.localRotation = ConvertMapRotation(playerObj.transform.eulerAngles);

        if (switchObj != null)
        {
            switchImage.rectTransform.anchoredPosition = ConvertMapPosition(switchObj.transform.position);
        }
        else
        {
            switchObj = GameObject.FindGameObjectWithTag("Target");
        }

        DrawEnemy(Vector3.zero);
    }
}
