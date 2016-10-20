using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectManager : MonoBehaviour
{
    [SerializeField]
    int MaxStage = 1;
    [SerializeField]
    Animator m_BookAnim;

    int m_SelectNumber = 1;
    // Use this for initialization
    void Start ()
    {
        m_SelectNumber = 1;
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
            return;
        }
        if (v > 0)
        {
            m_SelectNumber = (int)Mathf.Max(1, (float)m_SelectNumber - 1);
            m_BookAnim.CrossFade("L_R",0.1f,0);
        }
        else
        {
            m_SelectNumber = (int)Mathf.Min((float)MaxStage, (float)m_SelectNumber + 1);
            m_BookAnim.CrossFade("R_L", 0.1f, 0);
        }
    }

    private void InputButtonA()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        SceneManager.LoadSceneAsync("Stage"+(m_SelectNumber));
    }
}
