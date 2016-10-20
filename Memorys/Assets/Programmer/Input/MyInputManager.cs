using UnityEngine;
using GamepadInput;

public class MyInputManager : BaseManager<MyInputManager>
{
    public enum Button { A, B, Y, X, RightShoulder, LeftShoulder, RightStick, LeftStick, Back, Start }
    public enum StickDirection { LeftStickRight, LeftStickLeft, LeftStickUp, LeftStickDown, RightStickRight, RightStickLeft, RightStickUp, RightStickDown }
    public enum Trigger { LeftTrigger, RightTrigger }
    public enum Axis { LeftStick, RightStick, Dpad }

    private static GamepadState currentState;
    private static GamepadState oldState;

    public void Awake()
    {
        //見つけてきたInstanceが自身でない場合はManagerが２つ存在している
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        currentState = GamePad.GetState(GamePad.Index.One);
        oldState = currentState;
    }

    public void Update()
    {
        oldState = currentState;
        currentState = GamePad.GetState(GamePad.Index.One);
    }

    public static bool GetButton(Button button)
    {
        
        return GamePad.GetButton((GamePad.Button)button, GamePad.Index.One);
    }

    public static bool GetButtonDown(Button button)
    {
        return GamePad.GetButtonDown((GamePad.Button)button, GamePad.Index.One);
    }

    public static bool GetButtonUp(Button button)
    {
        
        return GamePad.GetButtonUp((GamePad.Button)button, GamePad.Index.One);
    }

    public static Vector2 GetAxis(Axis axis)
    {
        return GamePad.GetAxis((GamePad.Axis)axis, GamePad.Index.One);
    }

    public static float GetTrigger(Trigger trigger)
    {
        return GamePad.GetTrigger((GamePad.Trigger)trigger, GamePad.Index.One);
    }

    public static bool IsStickDown(StickDirection direction)
    {
        Vector2 stick;
        if (direction >= (StickDirection)4)
            stick = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One);
        else
            stick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
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

    public static bool IsJustStickDown(StickDirection direction)
    {
        Vector2 stick,oldStick;
        float dead = 0.3f;

        if (direction >= (StickDirection)4)
        {
            stick = currentState.rightStickAxis;
            oldStick = oldState.rightStickAxis;
        }
        else
        {
            stick = currentState.LeftStickAxis;
            oldStick = oldState.LeftStickAxis;
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

    public static bool IsTriggerDown(Trigger trigger)
    {
        float t = GamePad.GetTrigger((GamePad.Trigger)trigger, GamePad.Index.One);
        float dead = 0.05f;

        return t > dead;
    }

    public static bool IsJustTriggerDown(Trigger trigger)
    {
        float t,oldT;
        float dead = 0.05f;
        
        if (trigger == Trigger.LeftTrigger)
        {
            t = currentState.LeftTrigger;
            oldT = oldState.LeftTrigger;
        }
        else
        {
            t = currentState.RightTrigger;
            oldT = oldState.RightTrigger;
        }
        
        return t > dead && oldT <= dead;
    }

}