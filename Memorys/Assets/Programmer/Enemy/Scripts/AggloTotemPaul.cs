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

        //todo:SE
        yield return intervalWait;

        //終了処理
        IsAttacking = false;
    }

    protected override IEnumerator SetColor(bool atStart = false)
    {
        Color startColor = Color.black;
        Color endColor = new Color(1.0f, 0.8977686f, 0.4705881f);
        Color overColor = new Color(3.0f, 2.693306f, 1.411764f);

        Coroutine coroutine = StartCoroutine(SetEmissionColor(startColor, overColor, 1.0f));
        if (atStart) activateCoroutineList.Add(coroutine);
        yield return coroutine;

        coroutine = StartCoroutine(SetEmissionColor(overColor, endColor, 1.5f));
        if (atStart) activateCoroutineList.Add(coroutine);
        yield return coroutine;
    }
}
