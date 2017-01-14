using System.Collections;
using UnityEngine;

public class AggloTotemPaul : TotemPaul
{
    ParticleSystem firingEffect;

    public override void Start()
    {
        base.Start();

        playerHitEffect = ShotManager.Instance.GetParticle("shot_hit(Clone)");
        objectHitEffect = ShotManager.Instance.GetParticle("shot_landing(Clone)");
        firingEffect = ShotManager.Instance.GetParticle("shot_charge(Clone)");
    }

    protected override IEnumerator Attacking()
    {
        IsAttacking = true;
        //発射エフェクト
        firingEffect.transform.parent.position = chargeEffect.transform.position;
        firingEffect.Play(true);

        //発射
        Shot(GetTargetPosition());
        yield return intervalWait;

        //終了処理
        IsAttacking = false;
    }
}
