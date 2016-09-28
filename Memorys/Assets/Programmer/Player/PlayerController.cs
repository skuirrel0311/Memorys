﻿using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Idle,
    Move,
    Jump,
    Fall,
    Land    //着地
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

    GameObject cameraObject;
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
        cameraObject = GameObject.Find("MainCamera");
    }

    void FixedUpdate()
    {
        if (recorder.m_RecordState == RecordState.PLAY) return;
        Vector3 movement = GetInputVector();

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            movement,          //入力の角度まで
            360 * Time.deltaTime / Vector3.Angle(transform.forward, movement)
            );
        transform.LookAt(transform.position + forward);

        transform.Translate(movement, Space.World);
        //body.AddForce(movement);

        switch (currentState)
        {
            case PlayerState.Fall:
                if(IsOnGround())
                {
                    currentState = PlayerState.Land;
                }
                else
                {
                    //Debug.Log("No Ground Hit");
                }
                break;
        }

        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 movement = transform.position - oldPosition;

        if (Input.GetButtonDown("Fire2")) GetComponentInChildren<Animator>().CrossFade("punching", 0.1f);

        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                if (Input.GetButtonDown("Fire1")) Jumpping();
                break;
            case PlayerState.Jump:
                if (movement.y < 0)
                {
                    currentState = PlayerState.Fall;
                }
                break;
            case PlayerState.Land:
                animationContoller.LandingAnimation();
                currentState = movement == Vector3.zero ? PlayerState.Idle : PlayerState.Move;
                break;
        }
    }

    Vector3 GetInputVector()
    {
        Vector3 movement = Vector3.zero;

        movement.z = Input.GetAxis("Vertical");
        movement.x = Input.GetAxis("Horizontal");

        Quaternion cameraRotation = cameraObject.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        movement = cameraRotation * movement;

        if (currentState != PlayerState.Jump && currentState != PlayerState.Fall)
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
        if (recorder.m_RecordState == RecordState.PLAY) return;

        animationContoller.JumpAnimation();
        currentState = PlayerState.Jump;
        body.AddForce(Vector3.up * jumpPower);
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

        //落ちていない
        if (IsOnGround()) return;

        //落ちたであろう
    }

}
