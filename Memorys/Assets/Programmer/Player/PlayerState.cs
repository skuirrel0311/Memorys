using UnityEngine;
using System.Collections;

public abstract class PlayerState
{
    public abstract void Enter();

    public abstract void Update();

    public abstract void Exit();
}
