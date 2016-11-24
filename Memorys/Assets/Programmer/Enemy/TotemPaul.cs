using UnityEngine;
using System.Collections;

public class TotemPaul : MonoBehaviour
{
    [SerializeField]
    GameObject beamParticle = null;
    
    LineRenderer lineRenderer;
    Ray shotRay;
    RaycastHit hit;
    public float range = 20.0f;
    [SerializeField]
    Vector3 offset = new Vector3(0,5,0);
    [SerializeField]
    Vector3 targetOffset = new Vector3(0,1.5f,0);

    GameObject player;
    
    Timer timer;

    public bool IsAttacking { get { return timer.IsWorking; } }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        timer = new Timer();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        timer.Update();

        if(timer.IsLimitTime)
        {
            lineRenderer.enabled = false;
            timer.Stop(true);
        }
    }

    public void Attack(float interval)
    {
        if (IsAttacking) return;
        timer.TimerStart(interval);

        shotRay.origin = transform.position + offset;
        shotRay.direction = (player.transform.position + targetOffset) - shotRay.origin;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shotRay.origin);


        if (Physics.Raycast(shotRay, out hit, range))
        {
            //todo:damage
        }
        lineRenderer.SetPosition(1,shotRay.origin + shotRay.direction * range);
    }
}
