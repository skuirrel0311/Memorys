using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEnd
{
    //ゲームが終了している
    public static bool isGameEnd;

    //何回キャンセルすればゲームクリアになるか
    private const int c_MaxDestroyCalcel = 5;

    //何回ステージが崩壊したらゲームオーバーか
    private const int c_MaxDestroy = 5;

    //ゲームクリアコールバック
    public delegate void GameClearCallBack();
    public GameClearCallBack OnGameClearCallBack;

    //ゲームオーバーコールバック
    public delegate void GameOverCallBack();
    public GameOverCallBack OnGameOverCallBack;

    /// <summary>
    /// ステージの崩壊を何回キャンセルしたか
    /// </summary>
    private int m_destoryCancelCount=0;

    /// <summary>
    ///     何回ステージが崩壊したか
    /// </summary>
    private int m_StageDestroy=0;
    public int StageDestroyCount
    {
        get { return m_StageDestroy; }
    }


    public void Initialize()
    {
        m_destoryCancelCount = 0;
        isGameEnd = false;
    }

    public void Update()
    {
        if (isGameEnd)
        {
            if (MyInputManager.GetButtonDown(MyInputManager.Button.A))
            {
                Time.timeScale = 1.0f;
                SceneManager.LoadSceneAsync("StageSelect");
            }
            return;
        }
        //ゲームがクリアされた
        if (m_destoryCancelCount >= c_MaxDestroyCalcel)
        {
            GameClear();
        }
    }

    public void DestroyCancel()
    {
        m_destoryCancelCount++;
        if (!(m_destoryCancelCount >= c_MaxDestroyCalcel))
            NotificationSystem.I.Indication("崩壊を止めた！　あと"+(c_MaxDestroyCalcel-m_destoryCancelCount)+"回!\nターゲットが別の場所に出現！");
            Update();
    }

    public void StageDestroy()
    {
        m_StageDestroy++;
        NotificationSystem.I.Indication("ステージが崩壊！　\n別の場所にターゲットが出現");
    }

    public void GameClear()
    {
        isGameEnd = true;
        Time.timeScale = 0.1f;
        if (OnGameClearCallBack != null)
            OnGameClearCallBack();
    }

    public void GameOver()
    {
        isGameEnd = true;
        Time.timeScale = 0.1f;
        Debug.Log("GameOver");
        if (OnGameOverCallBack != null)
            OnGameOverCallBack();
    }
}
