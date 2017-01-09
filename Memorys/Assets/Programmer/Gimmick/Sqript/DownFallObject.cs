using UnityEngine;
using System.Collections;

public class DownFallObject : MonoBehaviour
{
    Timer viewTimer;
    //カメラの演出をするか？
    [SerializeField]
    bool IsCameraProduction = false;

    void Start()
    {
        viewTimer = new Timer();
    }

    void Update()
    {
        viewTimer.Update();

        if(viewTimer.IsLimitTime)
        {
            viewTimer.Stop(true);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "Player") return;

        StartCoroutine(Camera.main.GetComponent<CameraContoller>().SeeFellPlayer());

        //viewTimer.TimerStart(2.0f);
        //Debug.Log("Playerは紐なしバンジーを試みた!!");
    }

    //void OnGUI()
    //{
    //    if(viewTimer.IsWorking)
    //        GUI.TextArea(new Rect(60, 250, 250, 300), "Playerは紐なしバンジーを試みた!!");
    //}

}
