using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public int AttackPoint = 1;
    Timer timer;

    public bool IsAttacking { get { return timer.IsWorking; } }

    void Start()
    {
        timer = new Timer();
    }

    void Update()
    {
        timer.Update();

        if(timer.IsLimitTime)
        {
            timer.Stop(true);
        }
    }

    public virtual void Attack(float interval)
    {
        timer.TimerStart(interval);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!timer.IsWorking) return;

        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerOverlap>().Damage(AttackPoint);
        }
    }
}
