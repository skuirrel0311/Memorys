using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsSound : MonoBehaviour
{

    // 現在存在しているオブジェクト実体の記憶領域
    static UtilsSound _instance = null;

    // オブジェクト実体の参照（初期参照時、実体の登録も行う）
    static UtilsSound instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<UtilsSound>()); }
    }

    void Awake()
    {

        // ※オブジェクトが重複していたらここで破棄される

        // 自身がインスタンスでなければ自滅
        if (this != instance)
        {
            Destroy(gameObject);
            return;
        }

        // 以降破棄しない
        DontDestroyOnLoad(gameObject);

    }

    static void Play(string eventName)
    {
        AkSoundEngine.PostEvent(eventName, instance.gameObject);
    }

    public static void  SE_Decision()
    {
        Play("Menu_Decision");
    }

    public static void SE_MenuPage()
    {
        Play("Menu_Page");
    }

    public static void SE_Select()
    {
        Play("Menu_Select");
    }

    public static void SE_Cancel()
    {
        Play("Menu_Cancel");
    }

}
