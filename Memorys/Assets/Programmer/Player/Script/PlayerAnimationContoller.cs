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
        {
            ChangeAnimation("TopToGround", 0.1f);
            controller.currentState = PlayerState.Land;
        }

        //なぜか回転するのでなおす
        unitychan.eulerAngles = transform.eulerAngles;
    }

    public void ChangeAnimation(string name,float transitionDuration, bool check = true)
    {
        if (check && !CheckAnimationName(name))
        {
            animator.CrossFade(name, transitionDuration, 0);
        }
    }

    bool IsNearGround()
    {
        //落ちている方向にRayを飛ばす
       // Vector3 direction = transform.position - controller.oldPosition;
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag != "Floor") return false;

            //床が近いならtrueを返す
            return hit.distance < nearDistance;
        }

        return false;
    }

    bool IsNearGround(int i)
    {
        //落ちている方向
        Vector3 direction = transform.position - controller.oldPosition;
        Ray ray = new Ray(transform.position, direction);
        float radius = 0.1f;
        RaycastHit hit;

        if (Physics.SphereCast(ray,radius,out hit))
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
                    if (underObj.distance < nearDistance) return true;
                    return false;
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
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer." + name);
    }
}
