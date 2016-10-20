using UnityEngine;
using System.Collections;

public class AttackState : StateMachineBehaviour
{
    //攻撃しているかの通知
    private static bool isAttack = false;
    [SerializeField]
    float PushDiray = 0.1f;

    bool isNext;
    float Timer = 0;

    public static void AttackNotice()
    {
        isAttack = true;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isNext = false;
        Timer = 0.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isNext) return;
        if (MyInputManager.GetButtonDown(MyInputManager.Button.X))
        {
            if (Timer >= PushDiray)
            {
                PlayerController.I.Attack();
                isNext = true;
            }
        }
        if (isAttack)
        {
            isNext = true;
            isAttack = false;
        }
        Timer += Time.deltaTime;

      if(stateInfo.normalizedTime>0.8)
        {
            if (isNext)
            {
                PlayerController.I.currentState = PlayerState.Attack;
            }
            else
            {
                PlayerController.I.currentState = PlayerState.Idle;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
 
        if (!isNext)
        {
            animator.speed = 1;
        }
    }


    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
