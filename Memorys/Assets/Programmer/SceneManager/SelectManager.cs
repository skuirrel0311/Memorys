using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectManager : MonoBehaviour
{
    [SerializeField]
    int MaxStage = 1;
    [SerializeField]
    Animator m_BookAnim;

    private FusenSpawner m_FusenSpawner;
    public int m_SelectNumber = 1;
    bool isInputAxis;
    // Use this for initialization
    void Start ()
    {
        m_SelectNumber = 1;
        isInputAxis = false;
        m_FusenSpawner = GetComponent<FusenSpawner>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        InputAxis();
        InputButtonA();
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

        if (v > 0)
        {

            m_SelectNumber = (int)Mathf.Max(1, (float)m_SelectNumber - 1);
            m_FusenSpawner.SetAnimationRoot(m_SelectNumber - 1);
            m_BookAnim.Play("L_R",0);
        }
        else
        {
            m_SelectNumber = (int)Mathf.Min((float)MaxStage, (float)m_SelectNumber + 1);
            m_FusenSpawner.SetAnimationRoot(m_SelectNumber-1);
            m_BookAnim.Play("R_L", 0);

        }
    }

    private void InputButtonA()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        SceneManager.LoadSceneAsync("Stage"+(m_SelectNumber));
    }
}
