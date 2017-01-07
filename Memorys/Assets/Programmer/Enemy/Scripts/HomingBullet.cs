using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : Bullet
{
    Vector3 startPosition;
    Transform target;
    Timer updateTimer;
    PlayerController player;

    bool isHoming = false;

    protected void Start()
    {
        updateTimer = new Timer();
        updateTimer.TimerStart(0.01f);
        player = PlayerController.I;
    }

    public void SetUp(Transform target,ParticleSystem playerHitEffect, ParticleSystem objectHitEffect)
    {
        this.playerHitEffect = playerHitEffect;
        this.objectHitEffect = objectHitEffect;
        this.target = target;
        startPosition = transform.position;
        isHoming = true;
    }

    protected override void Update()
    {
        if(!isHoming)
        {
            base.Update();
            return;
        }
        
        if(IsPassing())
        {
            isHoming = false;
            base.Update();
            return;
        }

        updateTimer.Update();

        //一定間隔で移動量の更新を行う
        if (updateTimer.IsLimitTime)
        {
            updateTimer.Reset();
            //移動量を更新
            velocity = (GetTargetPosition() - transform.position).normalized * speed;
            transform.LookAt(target.position);
        }

        base.Update();
    }

    Vector3 GetTargetPosition()
    {
        Vector3 movement = player.movement;

        float futureRate = (transform.position - target.position).magnitude;

        return target.position + (movement * futureRate);
    }

    //通り過ぎたか？
    bool IsPassing()
    {
        return Vector3.Distance(startPosition, transform.position) > Vector3.Distance(startPosition, target.position);
    }
}
