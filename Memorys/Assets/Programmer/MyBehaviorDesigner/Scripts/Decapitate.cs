using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tactical;

public class Decapitate : MonoBehaviour,IAttackAgent
{
    private enum MyAnimationName { Idle, Walk, Attack}
    public float attackDistance = 5.0f;
    public float attackAngle = 5.0f;
    Animation myAnimation = null;
    Timer timer;
    float interval = 5.0f;

    [SerializeField]
    string[] animationNames = null;

    public void Start()
    {
        myAnimation = GetComponentInChildren<Animation>();
        ChangeAnimation(MyAnimationName.Idle);
        timer = new Timer();
    }

    public void Update()
    {
        timer.Update();

        if(timer.IsLimitTime)
        {
            timer.Stop(true);
        }
    }

    public float AttackDistance()
    {
        return attackDistance;
    }

    public float AttackAngle()
    {
        return attackAngle;
    }

    public bool CanAttack()
    {
        return !timer.IsWorking;
    }

    public void Attack(Vector3 targetPosition)
    {
        ChangeAnimation(MyAnimationName.Attack);
        timer.TimerStart(interval);
        GetComponentInChildren<EnemyAttack>().Attack(interval);
    }

    private void ChangeAnimation(MyAnimationName name)
    {
        myAnimation.Play(animationNames[(int)name]);
    }

}
