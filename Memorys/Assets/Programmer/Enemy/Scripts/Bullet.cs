using UnityEngine;

public class Bullet : MonoBehaviour
{
    //弾を撃ったやつ
    protected GameObject owner;
    protected Vector3 velocity;
    protected ParticleSystem playerHitEffect = null;
    protected ParticleSystem objectHitEffect = null;
    [SerializeField]
    protected float speed = 50.0f;

    public void SetUp(Vector3 velocity,ParticleSystem playerHitEffect, ParticleSystem objectHitEffect,GameObject owner)
    {
        this.playerHitEffect = playerHitEffect;
        this.objectHitEffect = objectHitEffect;
        this.velocity = velocity * speed;
        this.owner = owner;
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
    }

    void PlayerHit(GameObject playerObj)
    {
        //todo:SE

        playerHitEffect.transform.parent.position = transform.position;
        playerHitEffect.Play(true);

        playerObj.GetComponent<PlayerOverlap>().Damage(1);
        Destroy(gameObject);
    }

    void ObjectHit(GameObject otherObj)
    {
        //todo:SE

        string layerName = LayerMask.LayerToName(otherObj.layer);
        if (layerName != "Enemy" && layerName != "Floor") return;

        if (otherObj.Equals(owner))
        {
            return;
        }
        objectHitEffect.transform.parent.position = transform.position;
        objectHitEffect.Play(true);
        Destroy(gameObject);
    }
}
