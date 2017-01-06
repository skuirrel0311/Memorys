using UnityEngine;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class Bullet : MonoBehaviour
{
    Vector3 velocity;
    [SerializeField]
    GameObject playerHitEffect = null;
    [SerializeField]
    GameObject objectHitEffect = null;
    [SerializeField]
    float speed = 4.0f;

    Ray ray;
    RaycastHit hit;
    [SerializeField]
    LayerMask mask;

    public void SetUp(Vector3 velocity)
    {
        this.velocity = velocity * speed;
        ray = new Ray();
        ray.direction = velocity;
    }

    void Update()
    {
        ray.origin = transform.position;
        transform.Translate(velocity * Time.deltaTime, Space.World);

        if (Physics.Raycast(ray, out hit, (velocity * Time.deltaTime).magnitude, mask))
        {
            if (objectHitEffect != null)
            {
                //float rotateY = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180.0f;
                //float rotateX = MovementUtility.GetAngleY(Vector3.zero, hit.normal);
                //Quaternion temp = Quaternion.Euler(new Vector3(-rotateX, rotateY, 0.0f));
                //Instantiate(objectHitEffect, hit.point + (ray.direction * -0.1f), temp);
                Instantiate(objectHitEffect, hit.point + (ray.direction * -0.1f), Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
            PlayerHit(col.gameObject);
    }

    void PlayerHit(GameObject playerObj)
    {
        //float rotateY = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg + 180.0f;
        //Quaternion temp = Quaternion.Euler(new Vector3(0.0f, rotateY, 0.0f));
        //Destroy(Instantiate(playerHitEffect, transform.position, temp),1.5f);
        Destroy(Instantiate(playerHitEffect, transform.position, Quaternion.identity), 1.5f);
        playerObj.GetComponent<PlayerOverlap>().Damage(1);
        Destroy(gameObject);
    }
}
