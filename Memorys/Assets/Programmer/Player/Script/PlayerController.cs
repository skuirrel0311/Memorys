﻿using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Idle,
    Move,
    Jump,
    Fall,
    Land,   //着地
    Attack,
    Damage
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController I;
    public  delegate void OnAttack();
    public OnAttack AttackCallBack=null;

    /*パラメータ*/
    [SerializeField]
    float moveSpeed = 10;
    [SerializeField]
    float jumpPower = 250;
    [SerializeField]
    float airMoveSpeed = 5;
    public PlayerState currentState;    //現在のステート
    [SerializeField]
    bool NoJumping = false;
    [SerializeField]
    private float MaxStamina;
    public float stamina;
    private bool  isStaminaDepletion;

    GameObject cameraObject;
    Rigidbody body = null;
    RecordOfAction recorder = null;
    PlayerAnimationContoller animationContoller = null;
    public Vector3 oldPosition { get; private set; }

    [SerializeField]
    GameObject[] underCollider; //着地判定用

    //private enum ColliderPlace { Center, Left, Right, Back, Front }

     void Awake()
    {
        I = this;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        recorder = GetComponent<RecordOfAction>();
        animationContoller = GetComponent<PlayerAnimationContoller>();
        oldPosition = transform.position;
        currentState = PlayerState.Idle;
        cameraObject = GameObject.Find("MainCamera");
        stamina = MaxStamina;
        isStaminaDepletion = false;
    }

    void FixedUpdate()
    {
        Vector3 movement = GetInputVector();
        //スタミナ
        stamina = Mathf.Min(MaxStamina,stamina+Time.deltaTime);
        if (stamina == MaxStamina)
            isStaminaDepletion = false;

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            movement,          //入力の角度まで
            360 * Time.deltaTime / Vector3.Angle(transform.forward, movement)
            );
        transform.LookAt(transform.position + forward);

        transform.Translate(movement, Space.World);
        //body.AddForce(movement*50.0f,ForceMode.VelocityChange);

        switch (currentState)
        {
            case PlayerState.Land:
                if(IsOnGround())
                {
                    currentState = movement == Vector3.zero ? PlayerState.Idle : PlayerState.Move;
                }
                break;
        }

        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 movement = transform.position - oldPosition;
        //Debug.Log("currentState:"+currentState);
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                if (!NoJumping && MyInputManager.GetButtonDown(MyInputManager.Button.A)) Jumpping();
                //if (MyInputManager.GetButtonDown(MyInputManager.Button.X) ) Attack();
                break;
            case PlayerState.Jump:
                if (movement.y < 0)
                {
                   // currentState = PlayerState.Fall;
                }
                break;
            case PlayerState.Attack:
                //アニメーションの再生が終わったら戻る

                break;
        }
    }

    Vector3 GetInputVector()
    {
        if (currentState == PlayerState.Attack)return Vector3.zero;

        Vector2 leftStick = MyInputManager.GetAxis(MyInputManager.Axis.LeftStick);

        if(leftStick == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.A)) leftStick.x = -1;
            if (Input.GetKey(KeyCode.D)) leftStick.x = 1;
            if (Input.GetKey(KeyCode.W)) leftStick.y = 1;
            if (Input.GetKey(KeyCode.S)) leftStick.y = -1;
        }

        Vector3 movement = new Vector3(leftStick.x, 0, leftStick.y);
        if(!isStaminaDepletion&&MyInputManager.GetButton(MyInputManager.Button.LeftShoulder))
        {
            movement *= 2.0f;
            stamina = Mathf.Max(0.0f,stamina-(Time.deltaTime*2.0f));
            if (stamina == 0.0f)
                isStaminaDepletion = true;
        }

        if(isStaminaDepletion)
        {
            movement *= 0.0f;
        }

        Quaternion cameraRotation = cameraObject.transform.rotation;
        cameraRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        movement = cameraRotation * movement;

        if (currentState == PlayerState.Idle || currentState == PlayerState.Move)
        {
            //移動しているかしていないか
            currentState = movement == Vector3.zero ? PlayerState.Idle : PlayerState.Move;
        }

        if (currentState == PlayerState.Jump || currentState == PlayerState.Fall)
            return movement * airMoveSpeed;
        else
            return movement * moveSpeed;
    }

    void Jumpping()
    {
        if (recorder.IsPlaying) return;

        //animationContoller.ChangeAnimation("JumpToTop", 0.1f);
        currentState = PlayerState.Jump;
        body.AddForce(Vector3.up * jumpPower);
    }

    public void Attack()
    {
        //animationContoller.ChangeAnimation("Attack", 0.1f);
        currentState = PlayerState.Attack;
        if (AttackCallBack != null) AttackCallBack();

    }
    
    //UnderColliderのどれかが地面に触れていれば地面に接しているはず。
    bool IsOnGround()
    {
        RaycastHit hit;

        foreach (GameObject g in underCollider)
        {
            Ray underRay = new Ray(g.transform.position + (Vector3.up * 0.2f), Vector3.down);

            if (Physics.Raycast(underRay, out hit, 0.3f))
            {
                if (hit.transform.tag == "Floor") return true;
            }
        }
        return false;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Floor") return;

        //落ちていない
        if (IsOnGround()) return;

        //落ちたであろう
    }

}
