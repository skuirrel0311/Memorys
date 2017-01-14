using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class ShotManager : BaseManager<ShotManager>
{
    private struct EffectInstance
    {
        public GameObject particle;
        public string name;
    }

    public GameObject[] effects;
    List<EffectInstance> effectList = new List<EffectInstance>();
    
    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        //エフェクトのインスタンスを作成
        for(int i = 0;i< effects.Length;i++)
        {
            EffectInstance e;
            e.particle = Instantiate(effects[i],transform).gameObject;
            e.particle.transform.position = transform.position;
            e.name = e.particle.name;
            effectList.Add(e);
        }
    }

    public ParticleSystem GetParticle(string name)
    {
        for(int i = 0;i< effectList.Count;i++)
        {
            if (effectList[i].name == name)
            {
                return effectList[i].particle.transform.GetChild(0).GetComponent<ParticleSystem>();
            }
        }

        return null;
    }
}
