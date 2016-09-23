using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10;
    [SerializeField]
    float jumpPower = 250;
    [SerializeField]
    float airMoveSpeed = 5;

    Rigidbody body = null;
    RecordOfAction recorder = null;
    public Vector3 oldPosition { get; private set; }

    public bool IsOnGround = false;
    public bool IsJump = false;
    public bool IsFall = true;
    public bool IsMove = false;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        recorder = GetComponent<RecordOfAction>();
        oldPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (recorder.m_RecordState==RecordState.PLAY) return;
        Vector3 movement = GetInputVector();

        body.AddForce(movement);

        if (Input.GetKeyDown(KeyCode.Space)) Jumpping();
        oldPosition = transform.position;
    }

    void Jumpping()
    {
        if (recorder.m_RecordState == RecordState.PLAY) return;
        if (IsJump) return;

        GetComponent<PlayerAnimationContoller>().JumpAnimation();
        IsJump = true;
        IsOnGround = false;
        body.AddForce(Vector3.up * jumpPower);
    }

    void Update()
    {
        //ジャンプ中に落ちているか？
        if (!IsOnGround && oldPosition.y >= transform.position.y) IsFall = true;

        if (IsJump)
        {
            //ジャンプ中にUnderColliderとPlayerとの間にオブジェクトがあったら着地しているだろう。
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Floor") return;
        //上昇中には着地はできない
        if (IsOnGround) return;
        if (!IsFall) return;

        IsJump = false;
        IsFall = false;
        IsOnGround = true;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Floor") return;

        Ray underRay = new Ray(transform.position, Vector3.down);
        RaycastHit underObj;
        if (Physics.Raycast(underRay, out underObj))
        {
            if (underObj.transform.gameObject.tag == "Floor") return;
        }

        IsOnGround = false;
    }

    Vector3 GetInputVector()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;
        IsMove = movement != Vector3.zero;

        if (IsJump)
            return movement * airMoveSpeed;
        else
            return movement * moveSpeed;
    }
}
