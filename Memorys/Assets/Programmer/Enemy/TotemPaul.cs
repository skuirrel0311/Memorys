using UnityEngine;
using System.Collections;

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

    GameObject player;

    public bool IsAttacking { get; private set; }

    float chargeTime, intervalTime;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        lineRenderer.enabled = false;
        IsAttacking = false;
        chargeTime = intervalTime = 0;
    }

    void Update()
    {
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

        yield return new WaitForSeconds(chargeTime);

        shotRay.origin = chargeEffect.transform.position;
        shotRay.direction = (player.transform.position + targetOffset) - shotRay.origin;

        lineRenderer.enabled = true;
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

}
