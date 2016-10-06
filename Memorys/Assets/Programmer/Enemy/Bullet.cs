using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    Vector3 velocity;

    void Start()
    {
        velocity = Vector3.zero;
    }

    public void Shot(Transform startPosition, float speed)
    {
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
        velocity = transform.forward * speed;
    }

    void Update()
    {
        if (transform == null) return;
        

        transform.Translate(velocity, Space.World);
    }

    void OnCollisionEnter(Collision col)
    {
        gameObject.SetActive(false);
    }
}
