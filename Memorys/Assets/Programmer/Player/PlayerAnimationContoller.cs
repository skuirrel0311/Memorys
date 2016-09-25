using UnityEngine;
using System.Collections;

public class PlayerAnimationContoller : MonoBehaviour
{
    Animator animator;
    PlayerController controller;

    [SerializeField]
    Transform unitychan = null;

    [SerializeField]
    float nearDistance = 0.9f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        animator.SetBool("IsMove", controller.currentState == PlayerState.Move);

        if (controller.currentState == PlayerState.Fall && IsNearGround())
            animator.CrossFade("TopToGround", 0.1f, 0);

        //なぜか回転するのでなおす
        unitychan.eulerAngles = new Vector3(0, 90, 0);
    }

    public void JumpAnimation()
    {
        animator.CrossFade("JumpToTop", 0.1f, 0);
    }

    public void LandingAnimation()
    {
        animator.CrossFade("TopToGround", 0.1f, 0);
    }

    bool IsNearGround()
    {
        //落ちている方向
        Vector3 direction = transform.position - controller.oldPosition;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag != "Floor") return false;
            //当たった地点の法線が上を向いていなかったら
            if (hit.normal != Vector3.up)
            {
                //真下も確認
                Ray underRay = new Ray(transform.position, Vector3.down);
                RaycastHit underObj;
                if (Physics.Raycast(underRay, out underObj))
                {
                    if (underObj.transform.gameObject.tag != "Floor") return false;
                    return true;
                }
                return false;
            }
            //地面が近かった
            if (hit.distance < nearDistance) return true;
        }

        return false;
    }

    //一致していたらtrueを返す
    public bool CheckAnimationName(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer." + name);
    }
}
