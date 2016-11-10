using UnityEngine;
using System.Collections;

public class CircleMapManager : MapManager
{
    [SerializeField]
    float radius = 30.0f;

    public override void Update()
    {
        playerImage.rectTransform.localRotation = ConvertMapRotation(playerObj.transform.eulerAngles);

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

        DrawEnemy(playerObj.transform.position);
    }
}
