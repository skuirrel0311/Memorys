﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class SniperTotemPaul : TotemPaul
{
    [SerializeField]
    float intervalTime = 1.0f;
    [SerializeField]
    float chargeTime = 4.0f;

    LineRenderer lineRenderer;
    public float range = 50.0f;
    GameObject bullet;
    WaitForSeconds intervalWait;
    [SerializeField]
    LayerMask layerMask;
    Transform playerHips;

    public override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        ParticleSystem thunderEffect = chargeEffect.transform.GetChild(0).GetComponent<ParticleSystem>();

        float effectTime = chargeTime;
        effectTime = 3.5f / effectTime;

        var module = chargeEffect.main;
        module.simulationSpeed = effectTime;
        module = thunderEffect.main;
        module.simulationSpeed = effectTime;

        playerHitEffect = ShotManager.Instance.GetParticle("sniper_hit(Clone)");
        objectHitEffect = ShotManager.Instance.GetParticle("sniper_landing(Clone)");
        bullet = Instantiate(shotEffect);
        bullet.SetActive(false);
        intervalWait = new WaitForSeconds(intervalTime);
        playerHips = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
    }

    //攻撃
    public void Attack()
    {
        targetPosition = playerNeck.position;
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        chargeEffect.gameObject.SetActive(true);

        chargeEffect.Play(true);
        lineRenderer.enabled = true;

        //チャージ開始
        float time = 0.0f;
        while (true)
        {
            time += Time.deltaTime;
            if (time > chargeTime) break;

            //チャージしつつプレイヤーの方に向く
            Charge(GetTargetPosition());
            yield return null;
        }

        //チャージ終了
        lineRenderer.enabled = false;

        //発射
        StartCoroutine(Shot(GetTargetPosition(), 1));
        yield return intervalWait;

        //終了処理
        bullet.SetActive(false);
        chargeEffect.Stop(true);
        chargeEffect.gameObject.SetActive(false);
    }

    void Charge(Vector3 target)
    {
        Ray shotRay = GetToPlayerRay(target);

        RaycastHit hit;
        lineRenderer.SetPosition(0, shotRay.origin);

        if (Physics.Raycast(shotRay, out hit, range))
        {
            lineRenderer.SetPosition(1, hit.point + shotRay.direction * 0.2f);
        }
        else
        {
            lineRenderer.SetPosition(1, shotRay.origin + shotRay.direction * range);
        }
    }

    Ray GetFrontRay(Vector3 target)
    {
        Ray shotRay = new Ray();
        shotRay.origin = chargeEffect.transform.position;
        shotRay.direction = target - shotRay.origin;

        float y = shotRay.direction.y;
        shotRay.direction = transform.forward * shotRay.direction.magnitude;
        shotRay.direction = SetY(shotRay.direction, y);

        return shotRay;
    }

    Ray GetToPlayerRay(Vector3 target)
    {
        return new Ray(chargeEffect.transform.position, target - chargeEffect.transform.position);
    }

    protected IEnumerator Shot(Vector3 targetPosition, int arrivalFlame)
    {
        bullet.transform.position = chargeEffect.transform.position;
        bullet.transform.LookAt(targetPosition);
        bullet.SetActive(true);

        Vector3 velocity = targetPosition - bullet.transform.position;
        Vector3 nextPosition;
        RaycastHit hit;

        //tフレーム後に到着する
        velocity = velocity / arrivalFlame;
        //１フレーム多く計算する
        arrivalFlame += 1;

        for (int i = 0; i < arrivalFlame; i++)
        {
            yield return null;
            nextPosition = bullet.transform.position + velocity;

            if (Physics.Linecast(bullet.transform.position, nextPosition, out hit, layerMask))
            {
                bullet.transform.position = hit.point + velocity.normalized * 0.2f;
                if (hit.transform.tag == "Player")
                {
                    hit.transform.GetComponent<PlayerOverlap>().Damage(2);
                    playerHitEffect.transform.parent.position = hit.point;
                    playerHitEffect.Play(true);
                    break;
                }
                else
                {
                    objectHitEffect.transform.parent.position = hit.point;
                    objectHitEffect.Play(true);
                    break;
                }
            }

            //LineCastで当たらなかった
            bullet.transform.position = nextPosition;
        }
    }

    protected override Vector3 GetTargetPosition()
    {
        if (IsWarning)
        {
            targetPosition = Vector3.Lerp(playerNeck.position, playerHips.position, 0.5f);
        }
        return targetPosition;
    }

    Vector3 SetY(Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }
}
