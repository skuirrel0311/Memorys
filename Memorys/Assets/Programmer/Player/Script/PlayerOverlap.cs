using UnityEngine;
using UnityEngine.UI;
using System.Collections;


//プレイヤーの接触判定用クラス
public class PlayerOverlap : MonoBehaviour {

    const int maxHP=10;

    [SerializeField]
    Slider m_slider;

    int HP;

	// Use this for initialization
	void Start ()
    {
        HP = maxHP;
        m_slider.value = HP;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_slider.value = HP;
	}

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag=="Enemy")
        {
            HP--;
        }
    }
}
