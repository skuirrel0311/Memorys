using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;

    public void Shot(float speed)
    {
        velocity = transform.forward * speed;
    }

    void Update()
    {
        //if (transform == null) return;
        

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy") return;
        if (col.gameObject.name == "Bullet(Clone)") return;

        gameObject.SetActive(false);
    }
}
