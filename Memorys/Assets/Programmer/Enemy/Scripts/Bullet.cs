using UnityEngine;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class Bullet : MonoBehaviour
{
    protected Vector3 velocity;
    protected ParticleSystem playerHitEffect = null;
    protected ParticleSystem objectHitEffect = null;
    [SerializeField]
    protected float speed = 50.0f;

    Ray ray;
    RaycastHit hit;

    public void SetUp(Vector3 velocity,ParticleSystem playerHitEffect, ParticleSystem objectHitEffect)
    {
        this.playerHitEffect = playerHitEffect;
        this.objectHitEffect = objectHitEffect;
        this.velocity = velocity * speed;
        ray = new Ray();
        ray.direction = velocity;
    }

    protected virtual void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
            PlayerHit(col.gameObject);
        else
            ObjectHit(col.gameObject);

        Destroy(gameObject);
    }

    void PlayerHit(GameObject playerObj)
    {
        playerHitEffect.transform.parent.position = transform.position;
        playerHitEffect.Play(true);

        playerObj.GetComponent<PlayerOverlap>().Damage(1);
    }

    void ObjectHit(GameObject otherObj)
    {
        if(otherObj.tag != "Enemy" && otherObj.tag != "FieldObject")
        objectHitEffect.transform.parent.position = transform.position;
        objectHitEffect.Play(true);
    }
}
