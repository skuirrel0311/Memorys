using UnityEngine;
using GamepadInput;

public class MyInputManager : BaseManager<MyInputManager>
{
    public enum Button { A, B, Y, X, RightShoulder, LeftShoulder, RightStick, LeftStick, Back, Start }
    public enum StickDirection { LeftStickRight, LeftStickLeft, LeftStickUp, LeftStickDown, RightStickRight, RightStickLeft, RightStickUp, RightStickDown }
    public enum Trigger { LeftTrigger, RightTrigger }
    public enum Axis { LeftStick, RightStick, Dpad }

    private static GamepadState[] currentState = new GamepadState[4];
    private static GamepadState[] oldState = new GamepadState[4];

    public void Awake()
    {
        //見つけてきたInstanceが自身でない場合はManagerが２つ存在している
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        for (int i = 0; i < currentState.Length; i++)
        {
            currentState[i] = GamePad.GetState((GamePad.Index)i);
        }
        oldState = currentState;
    }

    public void Update()
    {
        oldState = currentState;
        for (int i = 0; i < currentState.Length; i++)
        {
            currentState[i] = GamePad.GetState((GamePad.Index)i);
        }
    }

    public static bool GetButton(Button button,GamePad.Index index = GamePad.Index.One)
    {
        return GamePad.GetButton((GamePad.Button)button, index);
    }

    public static bool GetButtonDown(Button button,GamePad.Index index = GamePad.Index.One)
    {
        return GamePad.GetButtonDown((GamePad.Button)button, index);
    }

    public static bool GetButtonUp(Button button, GamePad.Index index = GamePad.Index.One)
    {
        
        return GamePad.GetButtonUp((GamePad.Button)button, index);
    }

    public static Vector2 GetAxis(Axis axis, GamePad.Index index = GamePad.Index.One)
    {
        return GamePad.GetAxis((GamePad.Axis)axis, index);
    }

    public static float GetTrigger(Trigger trigger, GamePad.Index index = GamePad.Index.One)
    {
        return GamePad.GetTrigger((GamePad.Trigger)trigger, index);
    }

    public static bool IsStickDown(StickDirection direction, GamePad.Index index = GamePad.Index.One)
    {
        Vector2 stick;
        if (direction >= (StickDirection)4)
            stick = GamePad.GetAxis(GamePad.Axis.RightStick, index);
        else
            stick = GamePad.GetAxis(GamePad.Axis.LeftStick, index);
        float dead = 0.3f;
        switch(direction)
        {
            case StickDirection.LeftStickRight:
            case StickDirection.RightStickRight:
                return stick.x > dead;
            case StickDirection.LeftStickLeft:
            case StickDirection.RightStickLeft:
                return stick.x < -dead;
            case StickDirection.LeftStickUp:
            case StickDirection.RightStickUp:
                return stick.y > dead;
            case StickDirection.LeftStickDown:
            case StickDirection.RightStickDown:
                return stick.y < -dead;
        }
        return false;
    }

    public static bool IsJustStickDown(StickDirection direction, GamePad.Index index = GamePad.Index.One)
    {
        Vector2 stick,oldStick;
        float dead = 0.3f;

        if (direction >= StickDirection.RightStickRight)
        {
            stick = currentState[(int)index].rightStickAxis;
            oldStick = oldState[(int)index].rightStickAxis;
        }
        else
        {
            stick = currentState[(int)index].LeftStickAxis;
            oldStick = oldState[(int)index].LeftStickAxis;
        }
        
        switch (direction)
        {
            case StickDirection.LeftStickRight:
            case StickDirection.RightStickRight:
                return stick.x > dead && oldStick.x <= dead;
            case StickDirection.LeftStickLeft:
            case StickDirection.RightStickLeft:
                return stick.x < -dead && oldStick.x >= -dead;
            case StickDirection.LeftStickUp:
            case StickDirection.RightStickUp:
                return stick.y > dead && oldStick.y <= dead;
            case StickDirection.LeftStickDown:
            case StickDirection.RightStickDown:
                return stick.y < -dead && oldStick.y >= -dead;
        }
        return false;
    }

    public static bool IsTriggerDown(Trigger trigger, GamePad.Index index = GamePad.Index.One)
    {
        float t = GamePad.GetTrigger((GamePad.Trigger)trigger, index);
        float dead = 0.05f;

        return t > dead;
    }

    public static bool IsJustTriggerDown(Trigger trigger, GamePad.Index index = GamePad.Index.One)
    {
        float t,oldT;
        float dead = 0.05f;
        
        if (trigger == Trigger.LeftTrigger)
        {
            t = currentState[(int)index].LeftTrigger;
            oldT = oldState[(int)index].LeftTrigger;
        }
        else
        {
            t = currentState[(int)index].RightTrigger;
            oldT = oldState[(int)index].RightTrigger;
        }
        
        return t > dead && oldT <= dead;
    }

}