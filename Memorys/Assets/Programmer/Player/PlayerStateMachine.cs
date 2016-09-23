using UnityEngine;
using System.Collections;

public class PlayerStateMachine : MonoBehaviour
{

    //現在のステータス
    PlayerState currentState;

    void Start()
    {
        currentState = null;
    }

    public void ChangeState(PlayerState state)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }

        currentState = state;
        currentState.Enter();
    }

    void Update()
    {
        if(currentState != null)
        {
            currentState.Update();
        }
    }
}
