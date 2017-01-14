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
    public PlayerState currentState;  //現在のステート

    [SerializeField]
    private float MaxStamina;
    public float stamina;
    private bool isStaminaDepletion;

    GameObject cameraObject;
    Rigidbody body = null;
    PlayerAnimationContoller animationContoller = null;
    public Vector3 movement { get; private set; }
    public Vector3 oldPosition { get; private set; }

    bool isSquat;

    private float jumpTime = 0;
    private float m_accel;
    private bool isJump;
    private bool isClumbHint;


    void Awake()
    {
        I = this;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        animationContoller = GetComponent<PlayerAnimationContoller>();
        oldPosition = transform.position;
        currentState = PlayerState.Idle;
        cameraObject = GameObject.Find("MainCamera");
        stamina = MaxStamina;
        isStaminaDepletion = false;
        isSquat = false;
        m_accel = 0;
        isJump = false;
        isClumbHint = false;
    }

    void FixedUpdate()
    {
        if (GameManager.I.IsPlayStop&&!GameEnd.isGameClear) return;
        if (isJump) return;
        Vector3 movement = GetInputVector();
        if (isSquat) movement *= 0.5f;
        if (GameEnd.isGameClear)
        {
            movement = -Vector3.forward * 0.15f;
            currentState = PlayerState.Move;
            isSquat = false;
        }

        if (movement == Vector3.zero)
        {
            m_accel = 0.0f;
        }
        else
        {
            m_accel = movement.magnitude * 6.666f;
        }
        GetComponent<Animator>().SetFloat("RunSpeed", m_accel);
        //スタミナ
        stamina = Mathf.Min(MaxStamina, stamina + Time.deltaTime);
        if (stamina == MaxStamina)
            isStaminaDepletion = false;

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            movement,          //入力の角度まで
            700 * Time.deltaTime / Vector3.Angle(transform.forward, movement)
            );
        transform.LookAt(transform.position + forward);

        transform.Translate(movement, Space.World);
        //body.AddForce(movement*50.0f,ForceMode.VelocityChange);
        this.movement = movement;
        oldPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.I.IsPlayStop) return;
        Vector3 movement = transform.position - oldPosition;
        jumpTime += Time.deltaTime;
        //Debug.Log("currentState:"+currentState);
        bool isButtonDown = (MyInputManager.GetButtonDown(MyInputManager.Button.A) || Input.GetKeyDown(KeyCode.Space));

        if (isButtonDown) Jumpping();
        //(isSquat || m_accel < 0.5f) &&
        if (MyInputManager.GetButtonDown(MyInputManager.Button.B) && currentState != PlayerState.Clamber && currentState != PlayerState.Jump) ChangeSquat(!isSquat);
        CheckFall();
    }

    void CheckFall()
    {
        if (currentState == PlayerState.Jump) return;
        if (currentState == PlayerState.Clamber) return;
        bool isonGround = IsOnGround();
        if (isonGround || body.velocity.y > -1.5f)
        {
            if (currentState == PlayerState.Fall)
            {
                currentState = PlayerState.Idle;
                AkSoundEngine.PostEvent("Player_Landing", gameObject);
            }
            if (isonGround)
            {
                if (isJump)
                    Debug.Log("landing");
                isJump = false;
            }
            return;
        }
        currentState = PlayerState.Fall;

        //自動よじ登り
        if (isJump)
        {
            Clamb(GetForwardObject());
        }
    }

    public void JumpCheckFall()
    {
        JumpClamb(GetForwardObject());
        if (IsOnGround(1.0f))
        {
            currentState = PlayerState.Idle;
            return;
        }
        currentState = PlayerState.Fall;
    }

    Vector3 GetInputVector()
    {
        if (currentState == PlayerState.Jump) return Vector3.zero;
        if (currentState == PlayerState.Clamber) return Vector3.zero;
        int hash = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (hash == Animator.StringToHash("Damage") || hash == Animator.StringToHash("LongJump") || hash == Animator.StringToHash("Clamber") || hash == Animator.StringToHash("ClamberFailed"))
        {
            return Vector3.zero;
        }

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
            movement *= 1.3f;
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

        if (go == null &&m_accel > 0.9f)
        {
            if (isSquat) return;
            ChangeSquat(false);
            if (currentState == PlayerState.Fall)
                GetComponent<Animator>().CrossFadeInFixedTime("LongJump", 0.1f);
            currentState = PlayerState.Jump;
            if (!isJump)
            {
                AkSoundEngine.PostEvent("Player_Jump", gameObject);
                GetComponent<Rigidbody>().velocity = Vector3.up * 3.0f;
                jumpTime = 0.0f;
            }

            isJump = true;
        }
        else if(go != null)
        {
            Clamb(go);
            Debug.Log("TransitionClamber");
        }
    }

    void Clamb(GameObject go)
    {
        if (currentState == PlayerState.Clamber) return;
        if (go == null) return;
        //当たったオブジェクトの高さの差が小さければよじ登り
        float dis = Mathf.Abs(transform.position.y - go.transform.position.y);

        if (dis <= 2.1f && dis >= 1.8f)
        {
            currentState = PlayerState.Clamber;
            transform.Translate(0.0f, 0.2f, 0.0f);
            GetComponent<Animator>().CrossFadeInFixedTime("Clamber", 0.1f);
            AkSoundEngine.PostEvent("Player_Climb", gameObject);
            isJump = false;
        }
        //よじ登り失敗
        else
        {
            currentState = PlayerState.Clamber;
            GetComponent<Animator>().CrossFadeInFixedTime("ClamberFailed", 0.1f);
        }
    }

    void JumpClamb(GameObject go)
    {
        if (currentState == PlayerState.Clamber) return;
        if (go == null) return;
        //当たったオブジェクトの高さの差が小さければよじ登り
        float dis = Mathf.Abs(transform.position.y - go.transform.position.y);

        if (dis <= 2.1f&&dis>=1.8f)
        {
            currentState = PlayerState.Clamber;
            transform.Translate(0.0f, 0.2f, 0.0f);
            GetComponent<Animator>().CrossFadeInFixedTime("Clamber", 0.1f);
            AkSoundEngine.PostEvent("Player_Climb", gameObject);
            isJump = false;
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

    bool IsOnGround(float distance = 0.3f)
    {
        RaycastHit hit;

        Ray underRay = new Ray(transform.position + (Vector3.up * 0.2f), Vector3.down);

        if (Physics.Raycast(underRay, out hit, distance))
        {
            gameObject.transform.SetParent(hit.collider.transform);
            return true;
        }
        gameObject.transform.parent = null;
        return false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isClumbHint && m_accel>0.5f&&GetForwardObject() != null)
        {
            isClumbHint = true;
            GetComponent<Animator>().CrossFadeInFixedTime("ClamberFailed", 0.1f);
        }

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

        Ray underRay = new Ray(transform.position + Vector3.up * 1.5f, transform.forward);

        if (!Physics.Raycast(underRay, out hit, 1.0f))
        {
            return null;
        }

        if (hit.transform.tag == "Player")
        {
            return null;
        }

        return hit.collider.gameObject;
    }

    public void FowardForce()
    {
        StartCoroutine(FowardForceCourtine());
    }

    IEnumerator FowardForceCourtine()
    {
        float t = 0;
        while(true)
        {
            t += Time.deltaTime;
            transform.GetComponent<Rigidbody>().AddForce(transform.forward * 3000.0f*Time.deltaTime, ForceMode.Acceleration);
            if (t > 0.2f) break;
            yield return null;
        }
    }

}
