using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class SniperTotemPaul : TotemPaul
{
    [SerializeField]
    GameObject hitEffect = null;

    GameObject chargeEndEffect;

    LineRenderer lineRenderer;
    public float range = 50.0f;
    

    public override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    //攻撃
    public void Attack(float intervalTime, float chargeTime)
    {
        targetPosition = playerNeck.position;
        StartCoroutine(Attacking(intervalTime,chargeTime));
    }

    IEnumerator Attacking(float intervalTime, float chargeTime)
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
        Vector3 velocity;
        GameObject bullet = Instantiate(shotEffect, chargeEffect.transform.position, chargeEffect.transform.rotation);
        Shot(GetTargetPosition(),out velocity);

        int t = 2;
        velocity = velocity / t;
        for (int i = 0; i < t; i++)
        {
            yield return null;

            //tフレーム後に到達する。
            bullet.transform.position += velocity;

        }
        Destroy(bullet, 1.5f);
        yield return new WaitForSeconds(intervalTime);

        //終了処理

        chargeEffect.gameObject.SetActive(false);
    }

    void Charge(Vector3 target)
    {
        Ray shotRay = GetToPlayerRay(target);

        RaycastHit hit;
        lineRenderer.SetPosition(0, shotRay.origin);

        if (Physics.Raycast(shotRay, out hit, range))
        {
            lineRenderer.SetPosition(1, hit.point);
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
        shotRay.direction = target  - shotRay.origin;

        float y = shotRay.direction.y;
        shotRay.direction = transform.forward * shotRay.direction.magnitude;
        shotRay.direction = SetY(shotRay.direction, y);

        return shotRay;
    }

    Ray GetToPlayerRay(Vector3 target)
    {
        return new Ray(chargeEffect.transform.position, target - chargeEffect.transform.position);
    }

    protected void Shot(Vector3 target,out Vector3 velocity)
    {
        Ray ray = new Ray(chargeEffect.transform.position, (target - chargeEffect.transform.position));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit,range))
        {
            //当たった場所にエフェクトを出す
            velocity = hit.point - ray.origin;

            Quaternion temp = Quaternion.Euler(new Vector3(90.0f, Mathf.Atan2(velocity.normalized.y, velocity.normalized.x) * Mathf.Rad2Deg, 0.0f));
            Instantiate(hitEffect, hit.point, temp);

            if(hit.transform.tag == "Player")
                hit.transform.GetComponent<PlayerOverlap>().Damage(1);
        }
        else
        {
            velocity = ray.origin + (ray.direction * range);
            velocity = velocity - ray.origin;
        }
    }

    protected override Vector3 GetTargetPosition()
    {
        if (IsWarning)
        {
            targetPosition = playerNeck.position;
        }
        return targetPosition;
    }

    ////警報
    //public void Alarm()
    //{
    //    int count = 0;
    //    BehaviorTree[] enemies = GameManager.I.enemies;
    //    for (int i = 0; i < enemies.Length; i++)
    //    {
    //        if (name == enemies[i].name) continue;
    //        if (!enemies[i].enabled) continue;
    //        if ((bool)enemies[i].GetVariable("IsCalled").GetValue()) continue;

    //        enemies[i].SetVariable("IsCalled", (SharedBool)true);
    //        count++;
    //    }

    //    //友達はいない。
    //    if (count == 0)
    //    {
    //        GetComponent<BehaviorTree>().SetVariable("IsCalled", (SharedBool)true);
    //        return;
    //    }
    //    GetComponent<BehaviorTree>().SetVariable("IsCalled", (SharedBool)false);
    //    //めっちゃ回る
    //    StartCoroutine("CallFriends");
    //}

    ////仲間を呼ぶ
    //IEnumerator CallFriends()
    //{
    //    float time = 0.0f;
    //    float rotationY = transform.eulerAngles.y;
    //    while (true)
    //    {
    //        time += Time.deltaTime;

    //        if (time > 3.0f) break;

    //        rotationY += 4.0f;
    //        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    //        yield return null;
    //    }
    //}

    Vector3 SetY(Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }
}
