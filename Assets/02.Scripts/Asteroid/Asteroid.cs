using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    [Header("Hexsphere Properties")]
    public HexsphereProperties properties;

    [Header("Terrain")]
    public Tile.TileType[] tileTypes;

    [Header("Asteroid Properties")]
    public float hp;
    private float flySpeed;
    private float damage;
    private float damageRadius;

    private Tile[] beingDamagedTiles;

    public UnityAction notifyDestroyedToMissile;
    public UnityAction<Asteroid> notifyDestroyedToSpawner;

    private void Update()
    {
        // Move
        //transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, flySpeed * Time.deltaTime);

        // Set shader properties
        tileTypes[0].material.SetVector("_core", transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlanetTile")) Collide(other.gameObject);
    }

    public void Init(Vector3 pos, UnityAction<Asteroid> notifyDestroyedToSpawner)
    {
        // Set settings
        properties.scale = Random.Range(0.5f, 1f);
        hp = Random.Range(100, 1000);       // 스테이지, 난이도를 고려해 랜덤한 범위를 설정한다. (생짜 랜덤 X)
        flySpeed = Random.Range(1f, 3f);
        damage = Random.Range(1f, 10f);      // settings.scale과 flySpeed를 적절히 계산해서 설정한다.
        damageRadius = Random.Range(0.5f, 1f); // settings.scale과 flySpeed를 적절히 계산해서 설정한다.        

        // Generate hexsphere and set position & scale
        HexsphereGenerator.Generate(properties);
        transform.position = pos;
        transform.localScale = Vector3.one * properties.scale;

        // Generate terrain
        GenerateTerrain();

        // Get being damaged Tiles
        if (Physics.Raycast(transform.position, -transform.position, out RaycastHit hit, float.MaxValue, 1 << LayerMask.NameToLayer("Tile")))
        {
            Collider[] colls = Physics.OverlapSphere(hit.point, damageRadius, 1 << LayerMask.NameToLayer("Tile"));
            beingDamagedTiles = new Tile[colls.Length];
            for (int i = 0; i < colls.Length; i++)
                beingDamagedTiles[i] = colls[i].gameObject.GetComponent<Tile>();
        }

        // Initialize delegate onDestroyed
        notifyDestroyedToMissile = null;
        if (this.notifyDestroyedToSpawner == null) this.notifyDestroyedToSpawner = notifyDestroyedToSpawner;
    }

    private void GenerateTerrain()
    {
        tileTypes[0].material = Instantiate(tileTypes[0].material);
        tileTypes[1].material = tileTypes[0].material;
        tileTypes[2].material = tileTypes[0].material;

        tileTypes[0].material.SetFloat("_radius", properties.sphereRadius * properties.scale);

        List<Tile> tiles = properties.tiles;
        for (int i = 0; i < tiles.Count; i++)
        {
            int random = Random.Range(0, 100);
            if (random < 20)
            {
                tiles[i].SetType(tileTypes[0]);
            }
            else if (random < 80)
            {
                tiles[i].SetType(tileTypes[1]);
            }
            else
            {
                tiles[i].SetType(tileTypes[2]);
            }
        }
    }

    // Collide with planet
    private void Collide(GameObject planet)
    {
        if (planet.activeInHierarchy)
        {
            // Damage planet
            beingDamagedTiles[0].GetComponentInParent<Planet>().Damaged(damage);

            // Damage planet's tiles
            for (int i = 0; i < beingDamagedTiles.Length; i++)
            {
                beingDamagedTiles[i].Damaged();
            }
        }

        // InActivate Asteroid
        Destroyed();

        // Play particle system effect
        PoolingManager.Instance.Get("AsteroidCollisionEffect", transform.position).GetComponent<ParticleSystem>().Play();

        // Shake camera
        CameraShake.Instance.Shake(0.5f);
    }

    // Damaged by tower
    public void Damaged(float amount)
    {
        hp -= amount;
        if (hp <= 0) Destroyed();
    }

    // Collide with planet or Totally destroyed by tower
    public void Destroyed()
    {
        if (notifyDestroyedToMissile != null) notifyDestroyedToMissile();
        if (notifyDestroyedToSpawner != null) notifyDestroyedToSpawner(this);

        PoolingManager.Instance.Return(gameObject);
    }

    public void ShowDamageRange(bool value, Material warningMat = null)
    {
        for (int i = 0; i < beingDamagedTiles.Length; i++)
            beingDamagedTiles[i].Warned(value, warningMat);
    }
}
