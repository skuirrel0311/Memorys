using System.Collections;
using UnityEngine;

public class AggloTotemPaul : TotemPaul
{
    [SerializeField]
    GameObject firingEffect = null;

    //攻撃
    public void Attack(float intervalTime)
    {
        if (IsAttacking) return;
        targetPosition = playerNeck.position;
        StartCoroutine("Attacking",intervalTime);
    }

    IEnumerator Attacking(float intervalTime)
    {
        IsAttacking = true;
        Destroy(Instantiate(firingEffect, chargeEffect.transform.position, Quaternion.identity),2.0f);
        //発射
        Shot(GetTargetPosition());
        float time = 0.0f;
        while(true)
        {
            time += Time.deltaTime;
            if (time > intervalTime) break;

            yield return null;
        }
        yield return new WaitForSeconds(intervalTime);

        //終了処理
        IsAttacking = false;
    }
}
