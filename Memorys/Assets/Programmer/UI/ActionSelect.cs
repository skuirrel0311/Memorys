using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionSelect : MonoBehaviour {
    public static ActionSelect I;
    public enum ActionSelected
    {
        Move,Attack
    }
    public ActionSelected selected;

    Image Attack;
    Image Move;
    public bool isActive;

	// Use this for initialization
	void Start ()
    {
        selected = ActionSelected.Move;
        Attack = transform.GetChild(0).GetComponent<Image>();
        Move = transform.GetChild(1).GetComponent<Image>();
        isActive = false;
        ContentUnEnabled();
        I = this;
    }
   
    public bool IsActionStint(ActionSelected actionSelected)
    {
        return RecordOfAction.I.m_RecordState == RecordState.RECORD && actionSelected != selected;
    }

    void ContentEnebled()
    {
        Attack.gameObject.SetActive(true);
        Move.gameObject.SetActive(true);
        ColorUpdate();
    }

    void ContentUnEnabled()
    {
        Attack.gameObject.SetActive(false);
        Move.gameObject.SetActive(false);
    }

    void ColorUpdate()
    {
        if (selected == ActionSelected.Attack)
        {
            Move.color = new Color(1, 1, 1, 0.5f);
            Attack.color = Color.white;
        }
        else
        {
            Attack.color = new Color(1, 1, 1, 0.5f);
            Move.color = Color.white;
        }
    }

	
	// Update is called once per frame
	void Update ()
    {
        if (MyInputManager.GetButtonDown(MyInputManager.Button.LeftShoulder) && RecordOfAction.I.m_RecordState != RecordState.RECORD)
        {
            ContentEnebled();
            Time.timeScale = 0;
            isActive = true;
        }

        if (isActive)
        {
            float vert = MyInputManager.GetAxis(MyInputManager.Axis.LeftStick).y;
            if(vert>0)
            {
                selected = (ActionSelected)Mathf.Max(0,(float)(selected)-1);
            }
            if(vert<0)
            {
                selected = (ActionSelected)Mathf.Min(1, (float)(selected) + 1);
            }
            ColorUpdate();
            if(MyInputManager.GetButtonDown(MyInputManager.Button.A))
            {
                Time.timeScale = 1;
                isActive = false;
                ContentUnEnabled();
                RecordOfAction.I.RecordStart();
            }
        }
	
	}
}
