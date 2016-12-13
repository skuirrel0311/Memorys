using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    Vector3 velocity;
    [SerializeField]
    GameObject hitEffect = null;
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
            Destroy(this);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
            PlayerHit(col.gameObject);
    }

    void PlayerHit(GameObject playerObj)
    {
        Quaternion temp = Quaternion.Euler(new Vector3(90.0f, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg, 0.0f));
        Instantiate(hitEffect, transform.position, temp);
        playerObj.GetComponent<PlayerOverlap>().Damage(1);
        Destroy(this);
    }
}
