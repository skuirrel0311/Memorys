using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public static int c_MaxStage =10;
    public Animator m_BookAnim;
    [SerializeField]
    int MaxStage = 1;
    [SerializeField]
    Texture2D[] pages;
    [SerializeField]
    Renderer m_BookModel;
    [SerializeField]
    Renderer m_PageModel;

    [SerializeField]
    GameObject CameraObject;

    private Material[] m_Materials;
    //ページのマテリアルはアサインの順番が逆
    private Material[] m_PageMaterials;
    private SelectManager m_selectManager;
    public int m_SelectNumber = 1;
    bool isInputAxis;

    [SerializeField]
    bool isR_L;

    [SerializeField]
    Text m_BestTimeText;

    [SerializeField]
    ClearRankData rankData;

    [SerializeField]
    GameObject[] m_Stars = new GameObject[3];

    MySceneManager m_mySceneManager;

    bool isNext;
    // Use this for initialization
    void Start()
    {
        PlayData.StageNum = 1;
        m_SelectNumber = 1;
        isInputAxis = false;
        m_Materials = m_BookModel.materials;
        m_PageMaterials = m_PageModel.materials;
        isR_L = false;
        UpdateTexture();
        c_MaxStage = MaxStage;
        m_mySceneManager = GetComponent<MySceneManager>();
        isNext = false;
        BestTimeUpdate();
    }

    void BestTimeUpdate()
    {
        float time =  PlayerPrefsManager.I.GetClearTime(m_SelectNumber);
        m_BestTimeText.text = TkUtils.PlasticTime((int)time);

        if(time<rankData.Elements[m_SelectNumber-1].BestTime)
        {
            m_Stars[2].SetActive(true);
        }
        else
        {
            m_Stars[2].SetActive(false);
        }
        if(time<rankData.Elements[m_SelectNumber-1].BetterTime)
        {
            m_Stars[1].SetActive(true);
        }
        else
        {
            m_Stars[1].SetActive(false);
        }
        if(time < 5999)
        {
            m_Stars[0].SetActive(true);
        }
        else
        {
            m_Stars[0].SetActive(false);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int hash = m_BookAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (!(hash == Animator.StringToHash("R_L") || hash == Animator.StringToHash("R_L")))
        {
            UpdateTexture();
        }

        InputAxis();
        InputButtonA();
        
    }

    private void Update()
    {
        if(MyInputManager.GetButtonDown(MyInputManager.Button.B))
        {
            UtilsSound.SE_Cancel();
            m_mySceneManager.SceneLoad("Title");
        }
    }

    public void UpdateTexture()
    {
        int hash = m_BookAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        ChangeTexture(0, pages[m_SelectNumber - 1]);
        if (m_SelectNumber >= MaxStage) return;
        ChangeTexture(1, pages[m_SelectNumber]);
    }

    void ChangeTexture(int index, Texture2D tex)
    {
        m_Materials[index].mainTexture = tex;

        if (index == 0)
            m_PageMaterials[1].mainTexture = tex;
        else
            m_PageMaterials[0].mainTexture = tex;
    }

    private void InputAxis()
    {
        float v = MyInputManager.GetAxis(MyInputManager.Axis.LeftStick).x;
        if (v == 0)
        {
            isInputAxis = false;
            return;
        }
        if (isInputAxis) return;
        isInputAxis = true;

        int hash = m_BookAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (hash != Animator.StringToHash("Idele")) return;
            if (v > 0)
        {
            if (m_SelectNumber == 1) return;
            m_BookAnim.Play("L_R", 0);

            if(m_SelectNumber!=1) UtilsSound.SE_MenuPage();

            m_SelectNumber = (int)Mathf.Max(1, (float)m_SelectNumber - 1);
            UtilsSound.SE_MenuPage();
            BestTimeUpdate();
            UpdateTexture();
        }
        else
        {
            if (m_SelectNumber == MaxStage) return;
            m_BookAnim.Play("R_L", 0, 0.0f);

            if (m_SelectNumber != MaxStage) UtilsSound.SE_MenuPage();

            m_SelectNumber = (int)Mathf.Min((float)MaxStage, (float)m_SelectNumber + 1);
            BestTimeUpdate();
        }


    }

    private void InputButtonA()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        if (isNext) return;
        isNext = true;
        UtilsSound.SE_Decision();
        PlayData.StageNum = m_SelectNumber;
        PlayerPrefs.SetInt("StageNum",m_SelectNumber);
        PlayerPrefs.Save();
        CameraObject.transform.DOMove(new Vector3(-0.1f,0.22f,-0.03f),1.0f);
        StartCoroutine(Transition());
        //SceneManager.LoadSceneAsync("Loading");       
    }

    IEnumerator Transition()
    {
        float t = 0.0f;
        UnityStandardAssets.ImageEffects.DepthOfField dof = CameraObject.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
        TransitionManager.I.FadeOut();
        while (true)
        {
            t += Time.deltaTime;
            float p = Mathf.Pow(t, 10);
            dof.focalSize = 2 - (p*2);
            if (t > 1.0f)
                break;
            yield return null;
        }
        SceneManager.LoadSceneAsync("Loading");
    }
}
