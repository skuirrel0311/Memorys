using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Idle,
    Move,
    Jump
}

public class PlayerController : MonoBehaviour
{
    /*パラメータ*/
    [SerializeField]
    float moveSpeed = 10;
    [SerializeField]
    float jumpPower = 250;
    [SerializeField]
    float airMoveSpeed = 5;
    public PlayerState currentState;    //現在のステート
    
    Rigidbody body = null;
    RecordOfAction recorder = null;
    PlayerAnimationContoller animationContoller = null;
    public Vector3 oldPosition { get; private set; }

    [SerializeField]
    GameObject[] underCollider = new GameObject[3]; //着地判定用
    //private enum ColliderPlace { Center, Left, Right, Back, Front }
    
    void Start()
    {
        body = GetComponent<Rigidbody>();
        recorder = GetComponent<RecordOfAction>();
        animationContoller = GetComponent<PlayerAnimationContoller>();
        oldPosition = transform.position;
        currentState = PlayerState.Idle;
    }

    void FixedUpdate()
    {
        if (recorder.m_RecordState==RecordState.PLAY) return;
        Vector3 movement = GetInputVector();
        
        transform.Translate(movement);
        //body.AddForce(movement);

        switch(currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                if (Input.GetKeyDown(KeyCode.Space)) Jumpping();
                break;
            case PlayerState.Jump:
                if (IsLanding())
                {
                    Debug.Log("Landing");
                    animationContoller.LandingAnimation();
                    currentState = movement == Vector3.zero ? PlayerState.Idle : PlayerState.Move;
                }

                break;
        }

        oldPosition = transform.position;
    }

    Vector3 GetInputVector()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;

        if(currentState != PlayerState.Jump)
        {
            //移動しているかしていないか
            currentState = movement == Vector3.zero ? PlayerState.Idle : PlayerState.Move;
        }

        if (currentState == PlayerState.Jump)
            return movement * airMoveSpeed;
        else
            return movement * moveSpeed;
    }

    void Jumpping()
    {
        if (recorder.m_RecordState == RecordState.PLAY) return;

        animationContoller.JumpAnimation();
        currentState = PlayerState.Jump;
        body.AddForce(Vector3.up * jumpPower);
    }

    bool IsLanding()
    {
        //落下中ではない
        if (transform.position.y - oldPosition.y > 0) return false;

        return IsOnGround();
    }

    //UnderColliderのどれかが地面に触れていれば地面に接しているはず。
    bool IsOnGround()
    {
        RaycastHit hit;

        foreach (GameObject g in underCollider)
        {
            if (Physics.Linecast(transform.position, g.transform.position, out hit))
            {
                if (hit.transform.tag == "Floor") return true;
            }

        }
        return false;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Floor") return;

        Debug.Log("Called:OnCollisionExit");

        //落ちていない
        if (IsLanding()) return;

        //落ちたであろう
    }

}
