using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float flySpeed = 5f;
    public float curHeadingSpeed = 1f;
    public float maxHeadingSpeed = 2.5f;
    public float explosionRadius;
    private float damage;

    private Transform target;
    private bool isTargetDestroyed;

    private void Update()
    {
        if (target != null)
        {
            curHeadingSpeed = Mathf.Lerp(curHeadingSpeed, maxHeadingSpeed, 0.4f);
            transform.up = Vector3.MoveTowards(transform.up, target.position - transform.position, curHeadingSpeed * Time.deltaTime);

            transform.Translate(transform.up * flySpeed * Time.deltaTime, Space.World);
        }

        if (isTargetDestroyed && (transform.position - target.position).sqrMagnitude < 1f)
        {
            // Play particle system effect
            PoolingManager.Instance.Get("MissileExplosionEffect", transform.position).GetComponent<ParticleSystem>().Play();

            // Shake camera
            CameraShake.Instance.Shake(0.1f);

            // Return to pool
            PoolingManager.Instance.Return(gameObject);
        }
    }

    public void Init(Asteroid target, float damage)
    {
        target.notifyDestroyedToMissile += NotifiedTargetDestroyed;

        curHeadingSpeed = 1f;
        isTargetDestroyed = false;
        this.target = target.transform;
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AsteroidTile")) Collide(other.GetComponentInParent<Asteroid>());
    }

    private void Collide(Asteroid asteroid)
    {
        asteroid.Damaged(damage);

        //Collider[] colls = Physics.OverlapSphere(transform.position, explosionRadius, 1 << LayerMask.NameToLayer("Tile"));
        //for (int i = 0; i < colls.Length; i++)
        //{
        //    Tile tile = colls[i].gameObject.GetComponent<Tile>();
        //    tile.Damaged();
        //}

        PoolingManager.Instance.Get("MissileExplosionEffect", transform.position).GetComponent<ParticleSystem>().Play();

        CameraShake.Instance.Shake(0.1f);

        target.GetComponent<Asteroid>().notifyDestroyedToMissile -= NotifiedTargetDestroyed;

        PoolingManager.Instance.Return(gameObject);
    }

    private void NotifiedTargetDestroyed()
    {
        isTargetDestroyed = true;
    }
}
