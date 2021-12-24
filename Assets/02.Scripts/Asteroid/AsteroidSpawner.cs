using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public int spawnAmount;
    public float spawnRadius;

    public static List<Asteroid> Asteroids = new List<Asteroid>();

    private void Start()
    {
        Spawn(spawnAmount);
    }

    public void Spawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Asteroid asteroid = PoolingManager.Instance.Get("Asteroid", transform).GetComponent<Asteroid>();
            asteroid.Init(Random.onUnitSphere * spawnRadius, Destroyed);
            Asteroids.Add(asteroid);
        }
    }    

    public void Destroyed(Asteroid asteroid)
    {
        Asteroids.Remove(asteroid);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(Vector3.zero, spawnRadius);
    }
}
