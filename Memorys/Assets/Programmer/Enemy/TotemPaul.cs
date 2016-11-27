using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class TotemPaul : MonoBehaviour
{
    [SerializeField]
    ParticleSystem chargeEffect = null;
    
    LineRenderer lineRenderer;
    Ray shotRay;
    RaycastHit hit;
    public float range = 20.0f;
    [SerializeField]
    Vector3 offset = new Vector3(0,5,0);
    [SerializeField]
    Vector3 targetOffset = new Vector3(0,1.5f,0);

    Transform player;

    public bool IsAttacking { get; private set; }
    bool IsCharge = false;

    float chargeTime, intervalTime;

    bool isActive = false;

    Vector3 startPosition,underPosition;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Hips/Spine/Spine1/Spine2/Neck");
        lineRenderer.enabled = false;
        IsAttacking = false;
        chargeTime = intervalTime = 0;

        GetComponent<BehaviorTree>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);

        startPosition = transform.position;
        underPosition = new Vector3(transform.position.x, -transform.position.y - 10, transform.position.z);
    }

    void Update()
    {
        if (!isActive) transform.position = underPosition;
        if (IsCharge)
        {
            shotRay.origin = chargeEffect.transform.position;
            shotRay.direction = (player.position + targetOffset) - shotRay.origin;
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
    }

    public void Attack(float chargeTime,float intervalTime)
    {
        if (IsAttacking) return;
        this.chargeTime = chargeTime;
        this.intervalTime = intervalTime;

        StartCoroutine("Shot");
    }

    IEnumerator Shot()
    {
        IsAttacking = true;
        chargeEffect.gameObject.SetActive(true);
        chargeEffect.Play(true);
        IsCharge = true;
        lineRenderer.enabled = true;
        lineRenderer.SetWidth(0.1f, 0.1f);

        yield return new WaitForSeconds(chargeTime);

        shotRay.origin = chargeEffect.transform.position;
        shotRay.direction = (player.position + targetOffset) - shotRay.origin;
        lineRenderer.SetWidth(0.5f, 0.6f);

        IsCharge = false;
        lineRenderer.SetPosition(0, shotRay.origin);

        if (Physics.Raycast(shotRay, out hit, range))
        {
            Debug.Log(hit.transform.name);
            lineRenderer.SetPosition(1, hit.point);

            if(hit.transform.gameObject.tag == "Player")
            {
                hit.transform.GetComponent<PlayerOverlap>().Damage(1);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, shotRay.origin + shotRay.direction * range);
        }

        yield return new WaitForSeconds(intervalTime);

        lineRenderer.enabled = false;
        chargeEffect.gameObject.SetActive(false);
        IsAttacking = false;
    }

    //起動
    public void StartUp()
    {
        StartCoroutine("Rising");

        GetComponent<BehaviorTree>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);
        isActive = true;
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
        isActive = true;
    }

}
