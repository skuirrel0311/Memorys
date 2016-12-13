using UnityEngine;
using System.Collections;

public class Golem : MonoBehaviour
{
    [SerializeField]
    EnemyAttack leftArm = null, rightArm = null;

    //警戒度
    public float alertnessTime = 0.0f;
    [SerializeField]
    float maxAlertnessTime = 3.0f;

    public bool isSeePlayer = false, isHearPlayer = false;

    public bool IsAttacking { get { return leftArm.IsAttacking; } }

    void Update()
    {
        if(isSeePlayer) alertnessTime += Time.deltaTime * 1.5f;

        if (isHearPlayer) alertnessTime += Time.deltaTime;

        if(!isSeePlayer && !isHearPlayer) alertnessTime -= Time.deltaTime;

        alertnessTime = Mathf.Clamp(alertnessTime, 0, maxAlertnessTime);
    }

    public void Attack(float attackInterval)
    {
        leftArm.Attack(attackInterval);
        rightArm.Attack(attackInterval);
    }
}
