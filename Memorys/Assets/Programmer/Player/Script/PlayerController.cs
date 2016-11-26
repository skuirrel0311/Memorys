using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Idle,
    Move,
    Jump,
    Fall,
    Land,   //着地
    Attack,
    Damage,
    Clamber
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController I;
    public delegate void OnAttack();
    public OnAttack AttackCallBack = null;

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
    private bool isStaminaDepletion;

    GameObject cameraObject;
    Rigidbody body = null;
    RecordOfAction recorder = null;
    PlayerAnimationContoller animationContoller = null;
    public Vector3 oldPosition { get; private set; }

    [SerializeField]
    GameObject[] underCollider; //着地判定用

    bool isSquat;

    private float jumpTime = 0;

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
        isSquat = false;
    }

    void FixedUpdate()
    {
        Vector3 movement = GetInputVector();
        if (isSquat) movement *= 0.5f;
        //スタミナ
        stamina = Mathf.Min(MaxStamina, stamina + Time.deltaTime);
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
                if (IsOnGround())
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
        jumpTime += Time.deltaTime;
        //Debug.Log("currentState:"+currentState);
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                if (!NoJumping && (MyInputManager.GetButtonDown(MyInputManager.Button.A) || Input.GetKeyDown(KeyCode.Space))) Jumpping();
                if ((MyInputManager.GetButtonDown(MyInputManager.Button.B) || Input.GetKeyDown(KeyCode.B))) ChangeSquat(!isSquat);
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
        if (currentState == PlayerState.Jump) return Vector3.zero;
        if (currentState == PlayerState.Clamber) return Vector3.zero;
        Vector2 leftStick = MyInputManager.GetAxis(MyInputManager.Axis.LeftStick);

        if (leftStick == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.A)) leftStick.x = -1;
            if (Input.GetKey(KeyCode.D)) leftStick.x = 1;
            if (Input.GetKey(KeyCode.W)) leftStick.y = 1;
            if (Input.GetKey(KeyCode.S)) leftStick.y = -1;
        }

        Vector3 movement = new Vector3(leftStick.x, 0, leftStick.y);
        if (!isStaminaDepletion && MyInputManager.GetButton(MyInputManager.Button.LeftShoulder))
        {
            movement *= 2.0f;
            stamina = Mathf.Max(0.0f, stamina - (Time.deltaTime * 2.0f));
            if (stamina == 0.0f)
                isStaminaDepletion = true;
        }

        if (isStaminaDepletion)
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
        if (currentState != PlayerState.Move) return;

        GameObject go = GetForwardObject();

        if (go == null)
        {
            jumpTime = 0.0f;
            ChangeSquat(false);
            currentState = PlayerState.Jump;
        }
        else
        {
            ChangeSquat(false);
            currentState = PlayerState.Clamber;
            GetComponent<Animator>().CrossFadeInFixedTime("Clamber",0.1f);

            Debug.Log("TransitionClamber");
        }
    }

    void ChangeSquat(bool issquat)
    {
        isSquat = issquat;
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        //しゃがみ
        if (isSquat)
        {
            cap.center = new Vector3(0.0f, 0.4f, 0.0f);
            cap.height = 0.8f;
        }
        //立ち
        else
        {
            cap.center = new Vector3(0.0f, 0.8f, 0.0f);
            cap.height = 1.6f;
        }

        GetComponent<Animator>().SetBool("isSquat", isSquat);
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

        Ray underRay = new Ray(transform.position + (Vector3.up * 0.2f), Vector3.down);

        if (Physics.Raycast(underRay, out hit, 0.3f))
        {
            return true;
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

    void OnCollisionEnter(Collision col)
    {
        if (currentState == PlayerState.Jump)
        {
            if (IsOnGround())
            {
                currentState = PlayerState.Idle;
                return;
            }
        }
    }


    GameObject GetForwardObject()
    {
        RaycastHit hit;

        Ray underRay = new Ray(transform.position + (Vector3.up * 0.2f), transform.forward);

        if (!Physics.Raycast(underRay, out hit, 1.0f))
        {
            return null;
        }

        if(hit.transform.tag == "Player")
        {
            return null;
        }
        return hit.transform.gameObject;
    }

}
