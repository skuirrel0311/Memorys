using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectManager : MonoBehaviour
{
    public Animator m_BookAnim;
    [SerializeField]
    int MaxStage = 1;
    [SerializeField]
    Texture2D[] pages;
    [SerializeField]
    Renderer m_BookModel;
    [SerializeField]
    Renderer m_PageModel;

    private Material[] m_Materials;
    //ページのマテリアルはアサインの順番が逆
    private Material[] m_PageMaterials;
    private SelectManager m_selectManager;
    public int m_SelectNumber = 1;
    bool isInputAxis;

    [SerializeField]
    bool isR_L;
    // Use this for initialization
    void Start()
    {
        m_SelectNumber = 1;
        isInputAxis = false;
        m_Materials = m_BookModel.materials;
        m_PageMaterials = m_PageModel.materials;
        isR_L = false;
        UpdateTexture();
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
            m_SelectNumber = (int)Mathf.Max(1, (float)m_SelectNumber - 1);
            UpdateTexture();
        }
        else
        {
            if (m_SelectNumber == MaxStage) return;
            m_BookAnim.Play("R_L", 0, 0.0f);
            m_SelectNumber = (int)Mathf.Min((float)MaxStage, (float)m_SelectNumber + 1);
        }


    }

    private void InputButtonA()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        SceneManager.LoadSceneAsync("Stage" + (m_SelectNumber));
    }
}
