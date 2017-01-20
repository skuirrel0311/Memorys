using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using DG.Tweening;
using UnityEngine.PostProcessing;

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

    GameObject MainCamera;

    // Use this for initialization
    void Start()
    {
        HP = maxHP;
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(n => n.GetComponent<BehaviorTree>()).ToArray();
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
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
        SoundManager.PlaySound("Totem_Shot_impact");
        HP -= point;
        pointGauge.Value = HP;
        MainCamera.GetComponent<CameraContoller>().IsWork = false;
        MainCamera.GetComponent<PostProcessingBehaviour>().profile.vignette.enabled = true;
        MainCamera.transform.DOShakePosition(0.1f,0.5f,100,90,false,false).OnKill(() => {
            MainCamera.GetComponent<CameraContoller>().IsWork = true;
            MainCamera.GetComponent<PostProcessingBehaviour>().profile.vignette.enabled = false;
        });
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
