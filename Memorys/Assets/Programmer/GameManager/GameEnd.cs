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
        //ゲームがクリアされた
        if (m_destoryCancelCount >= c_MaxDestroyCalcel)
        {
        }
    }

    public void DestroyCancel()
    {

        m_destoryCancelCount++;
        if (!(m_destoryCancelCount >= c_MaxDestroyCalcel))
            NotificationSystem.I.Indication("脱出まで　あと" + (c_MaxDestroyCalcel - m_destoryCancelCount) + "回!\nターゲットが別の場所に出現！");
        Update();
        if (m_destoryCancelCount > c_MaxDestroyCalcel) return;
        if (OnDestroyCancelCallBack != null)
            OnDestroyCancelCallBack();
    }

    public void StageDestroy()
    {
        m_StageDestroy++;
        NotificationSystem.I.Indication("ステージが崩壊！　\n別の場所にターゲットが出現");
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
        isGameEnd = true;
        if (OnGameOverCallBack != null)
            OnGameOverCallBack();
    }
}
