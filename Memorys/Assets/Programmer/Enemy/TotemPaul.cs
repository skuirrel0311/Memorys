using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class TotemPaul : MonoBehaviour
{
    ParticleSystem chargeEffect;
    GameObject chargeEndEffect;
    [SerializeField]
    GameObject hitEffect = null;

    LineRenderer lineRenderer;
    public float range = 50.0f;
    [SerializeField]
    Vector3 targetOffset = new Vector3(0, 1.5f, 0);

    Transform player;
    Vector3 targetPosition;

    public bool IsAttacking { get; private set; }
    
    float chargeTime, intervalTime;
    float lostTime;

    Vector3 startPosition, underPosition;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        chargeEffect = transform.FindChild("Sphere/Charge/ball").GetComponent<ParticleSystem>();
        chargeEndEffect = transform.FindChild("Sphere/ChargeEnd").gameObject;
        player = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Hips/Spine/Spine1/Spine2/Neck");
        lineRenderer.enabled = false;
        IsAttacking = false;
        chargeTime = intervalTime = 0;

        GetComponent<BehaviorTree>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);

        startPosition = transform.position;
        underPosition = new Vector3(transform.position.x, -transform.position.y - 15.0f, transform.position.z);
    }

    void Update()
    {
        if (!GetComponent<BehaviorTree>().enabled) transform.position = underPosition;
    }

    //攻撃
    public void Attack(float intervalTime, float chargeTime)
    {
        if (IsAttacking) return;
        this.intervalTime = intervalTime;
        this.chargeTime = chargeTime;
        targetPosition = player.position;
        GetComponent<BehaviorTree>().SetVariable("TargetPosition", (SharedVector3)targetPosition);
        StartCoroutine("Attacking");
    }

    IEnumerator Attacking()
    {
        //チャージ開始
        IsAttacking = true;

        chargeEffect.gameObject.SetActive(true);
        chargeEffect.Play(true);

        lineRenderer.enabled = true;
        lineRenderer.SetWidth(0.1f, 0.1f);

        lostTime = 0.0f;
        float time = 0.0f;
        while (true)
        {
            time += Time.deltaTime;
            if (time > chargeTime) break;
            //チャージしつつプレイヤーの方に向く
            Charge(GetTargetPosition());
            yield return null;
        }

        //チャージ終了
        lineRenderer.SetWidth(0.5f, 0.6f);
        //chargeEffect.gameObject.SetActive(false);
        //chargeEndEffect.SetActive(true);

        while(true)
        {
            if (RotateTowards()) break;
            yield return null;
        }

        //発射
        Shot(GetTargetPosition());

        yield return new WaitForSeconds(intervalTime);

        //終了処理
        lineRenderer.enabled = false;
        chargeEffect.gameObject.SetActive(false);
        //chargeEndEffect.SetActive(false);
        IsAttacking = false;
    }

    void Charge(Vector3 target)
    {
        Ray shotRay;
        if (RotateTowards())
            shotRay = GetToPlayerRay(target);
        else
            shotRay = GetFrontRay(target);

        RaycastHit hit;
        lineRenderer.SetPosition(0, shotRay.origin);

        if (Physics.Raycast(shotRay, out hit, range))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, shotRay.origin + shotRay.direction * range);
        }
    }

    Ray GetFrontRay(Vector3 target)
    {
        Ray shotRay = new Ray();
        shotRay.origin = chargeEffect.transform.position;
        shotRay.direction = (target + targetOffset) - shotRay.origin;

        float y = shotRay.direction.y;
        shotRay.direction = transform.forward * shotRay.direction.magnitude;
        shotRay.direction = SetY(shotRay.direction, y);

        return shotRay;
    }

    Ray GetToPlayerRay(Vector3 target)
    {
        return new Ray(chargeEffect.transform.position, (target + targetOffset) - chargeEffect.transform.position);
    }

    void Shot(Vector3 target)
    {
        Ray shotRay = new Ray(chargeEffect.transform.position, (target + targetOffset) - chargeEffect.transform.position);
        RaycastHit hit;

        lineRenderer.SetPosition(0, shotRay.origin);

        if (Physics.Raycast(shotRay, out hit, range))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.transform.gameObject.tag == "Player")
            {
                //hit
                hit.transform.GetComponent<PlayerOverlap>().Damage(1);
                Quaternion temp = Quaternion.Euler(new Vector3(-90.0f, Mathf.Atan2(shotRay.direction.x, shotRay.direction.y) * Mathf.Rad2Deg, 0.0f));
                Instantiate(hitEffect, hit.point, temp);
            }
        }
        else
        {
            //no hit
            lineRenderer.SetPosition(1, shotRay.origin + shotRay.direction * range);
        }
    }

    Vector3 GetTargetPosition()
    {
        if ((bool)GetComponent<BehaviorTree>().GetVariable("IsSeePlayer").GetValue())
        {
            //見えているときだけtargetPositionを更新する
            targetPosition = player.position;
        }
        else
        {
            lostTime += Time.deltaTime;
            //１秒以内なら壁をすり抜けてプレイヤーを見つけ出す
            if(lostTime < 1)
            {
                targetPosition = player.position;
            }
        }
        GetComponent<BehaviorTree>().SetVariable("TargetPosition", (SharedVector3)targetPosition);
        return targetPosition;
    }

    bool RotateTowards()
    {
        Vector3 vec = targetPosition - transform.position;
        vec.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.5f);

        if(Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            return true;
        return false;
    }

    //起動
    public void StartUp()
    {
        StartCoroutine("Rising");

        GetComponent<BehaviorTree>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    IEnumerator Rising()
    {
        float time = 0.0f;
        while (true)
        {
            time += Time.deltaTime * 0.5f;
            transform.position = Vector3.Lerp(underPosition, startPosition, time * time);
            if (transform.position.y > startPosition.y) break;
            yield return null;
        }
    }

    public void QuickStartUp()
    {
        transform.position = startPosition;
        GetComponent<BehaviorTree>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    //警報
    public void Alarm()
    {
        int count = 0;
        BehaviorTree[] enemies = GameManager.I.enemies;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (name == enemies[i].name) continue;
            if (!enemies[i].enabled) continue;
            if ((bool)enemies[i].GetVariable("IsCalled").GetValue()) continue;

            enemies[i].SetVariable("IsCalled", (SharedBool)true);
            count++;
        }

        //友達はいない。
        if (count == 0)
        {
            GetComponent<BehaviorTree>().SetVariable("IsCalled", (SharedBool)true);
            return;
        }
        GetComponent<BehaviorTree>().SetVariable("IsCalled", (SharedBool)false);
        //めっちゃ回る
        StartCoroutine("CallFriends");
    }

    //仲間を呼ぶ
    IEnumerator CallFriends()
    {
        float time = 0.0f;
        float rotationY = transform.eulerAngles.y;
        while (true)
        {
            time += Time.deltaTime;

            if (time > 3.0f) break;

            rotationY += 4.0f;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
            yield return null;
        }
    }

    Vector3 SetY(Vector3 vec,float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }
}
