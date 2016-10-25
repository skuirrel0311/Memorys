using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public int AttackPoint = 1;
    public bool IsAttacking = false;

    void Start()
    {
    }

    void Update()
    {

    }

    public virtual void Attack()
    {
        IsAttacking = true;
    }

    public virtual void EndAttack()
    {
        IsAttacking = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (!IsAttacking) return;

        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerOverlap>().Damage(AttackPoint);
        }
    }
}
