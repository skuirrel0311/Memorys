﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

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
        if (PlayerController.I.currentState == PlayerState.Attack) return;
        if(col.gameObject.tag=="Enemy")
        {
            Damage(1);
        }
        if(col.gameObject.tag == "Bullet")
        {
            Damage(1);
        }
    }

    public void Damage(int point)
    {
        HP -= point;
        PlayerController.I.currentState = PlayerState.Damage;
        if (HP <= 0)
        {
            List<GameObject> enemyList = new List<GameObject>();
            enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

            foreach(GameObject g in enemyList)
            {
                g.GetComponent<BehaviorTree>().DisableBehavior();
            }
            //ゲーム終了イベントへ飛ばす（セーブポイントへ戻す？）
            SceneManager.LoadSceneAsync("Title");
        }
    }
}
