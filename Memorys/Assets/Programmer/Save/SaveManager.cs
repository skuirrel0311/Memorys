using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour {

    public static SaveManager I;
    SavePoint[] savePoints;

    void Awake()
    {
        PlayerPrefs.DeleteKey("SavePoint");
    }
	// Use this for initialization
	void Start ()
    {
        savePoints = gameObject.GetComponentsInChildren<SavePoint>();
        Debug.Log(savePoints[0].transform.position,savePoints[0].gameObject);
        I = this;
	}

    /// <summary>
    /// ゲーム中のセーブポイントの保存
    /// </summary>
    /// <param name="savePointName"></param>
    public void PointSave(int num)
    {
        Debug.Log("Save:"+num);
        PlayerPrefs.SetInt("SavePoint",num);
        PlayerPrefs.Save();
    }

    public int GetNowPoint()
    {
        return PlayerPrefs.GetInt("SavePoint");
    }

    public void Respawn()
    {
        int n = GetNowPoint();
        PlayerController.I.GetComponent<PlayerOverlap>().Death();
        if (n==0)
        {
            PlayerController.I.transform.position = Vector3.zero;
            return;
        }
        Debug.Log("SavePointRespawn"+ savePoints[n - 1].transform.localPosition);
        PlayerController.I.transform.position = savePoints[n-1].transform.localPosition;
    }	
}
