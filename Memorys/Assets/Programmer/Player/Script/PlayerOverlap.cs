using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

//プレイヤーの接触判定用クラス
public class PlayerOverlap : MonoBehaviour
{
    const int maxHP = 6;

    //[SerializeField]
    //Slider m_slider;
    [SerializeField]
    PointGauge pointGauge = null;
    BehaviorTree[] enemies;

    public int HP;

    bool isFound = false;

    // Use this for initialization
    void Start()
    {
        HP = maxHP;
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(n => n.GetComponent<BehaviorTree>()).ToArray();
    }

    // Update is called once per frame
    void Update()
    {


        isFound = false;

        foreach (BehaviorTree enemy in enemies)
        {
            if (!(bool)enemy.GetVariable("IsWarning").GetValue()) continue;
            //1匹でも見ていたらtrueにする。
            isFound = true;
        }
       
    }

    void OnCollisionEnter(Collision col)
    {
        //if (PlayerController.I.currentState == PlayerState.Attack) return;
        //if (col.gameObject.tag == "Enemy")
        //{
        //    Damage(1);
        //}
        //if (col.gameObject.tag == "Bullet")
        //{
        //    Damage(3);
        //}
    }

    public void Death()
    {
        GameManager.I.m_GameEnd.GameOver();
    }

    public void Damage(int point)
    {
        if (GameManager.I.IsPlayStop) return;
        HP -= point;
        pointGauge.Value = HP;
        PlayerController.I.currentState = PlayerState.Damage;
        if (HP <= 0)
        {
            Death();
        }
    }

    public bool IsAlive()
    {
        return HP > 0;
    }

    void DisableEnemy()
    {
        List<GameObject> enemyList = new List<GameObject>();
        enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        foreach (GameObject g in enemyList)
        {
            BehaviorTree tree = g.GetComponent<BehaviorTree>();
            if (tree != null) tree.DisableBehavior();
        }
    }

    public void Recovery(int point)
    {
        HP += point;
        HP = Mathf.Clamp(HP, 0, maxHP);
    }
}
