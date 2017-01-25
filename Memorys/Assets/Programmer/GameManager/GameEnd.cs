using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameEnd
{
    //ゲームが終了している
    public static bool isGameEnd;
    public static bool isGameClear { get; private set; }

    //何回キャンセルすればゲームクリアになるか
    public static int c_MaxDestroyCalcel = 5;

    //何回ステージが崩壊したらゲームオーバーか
    private const int c_MaxDestroy = 5;

    //ゲームクリアコールバック
    public delegate void VoidCallBack();
    public VoidCallBack OnGameClearCallBack;

    //ゲームオーバーコールバック
    public VoidCallBack OnGameOverCallBack;

    public VoidCallBack OnDestroyCancelCallBack;

    /// <summary>
    /// ステージの崩壊を何回キャンセルしたか
    /// </summary>
    public int m_destoryCancelCount { get; private set; }

    /// <summary>
    ///     何回ステージが崩壊したか
    /// </summary>
    private int m_StageDestroy = 0;
    public int StageDestroyCount
    {
        get { return m_StageDestroy; }
    }


    public void Initialize()
    {
        m_destoryCancelCount = 0;
        isGameEnd = false;
        isGameClear = false;
    }

    public void Update()
    {
        if (isGameEnd)
        {
            GameManager.I.IsPlayStop = true;
            return;
        }
    }

    public void DestroyCancel(int enemyNum)
    {
        m_destoryCancelCount++;
        if (!(m_destoryCancelCount >= c_MaxDestroyCalcel))
        {
            if(enemyNum != 0)
                NotificationSystem.I.Indication("脱出まで　あと『 " + (c_MaxDestroyCalcel - m_destoryCancelCount) + " 』個！\n新たに石像が"+ enemyNum + "体起動した……");
            else
                NotificationSystem.I.Indication("脱出まで　あと『 " + (c_MaxDestroyCalcel - m_destoryCancelCount) + " 』個！");
        }
        Update();
        if (m_destoryCancelCount > c_MaxDestroyCalcel) return;
        if (OnDestroyCancelCallBack != null)
            OnDestroyCancelCallBack();
    }


    public void GameClear()
    {
        if (isGameEnd) return;
        isGameEnd = true;
        isGameClear = true;
        if (OnGameClearCallBack != null)
            OnGameClearCallBack();
    }

    public void GameOver()
    {
        if (isGameEnd) return;
        TransitionManager.I.FadeOut(1.0f);
        Analytics.CustomEvent("GameOver", new Dictionary<string, object>
        {
            { "StageNum", PlayData.StageNum},
            { "DethPosition", PlayerController.I.gameObject.transform.position},
          });
        isGameEnd = true;
        if (OnGameOverCallBack != null)
            OnGameOverCallBack();
    }
}
