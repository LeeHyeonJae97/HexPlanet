using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Planet : MonoBehaviour
{
	[Header("Hexsphere Properties")]
	public HexsphereProperties properties;

	[Header("Terrain")]
	public NoiseSettings noiseSettings;
	private NoiseFilter noiseFilter;
	public Tile.TileType[] tileTypes;

	[Header("Planet Properties")]
	public float hp;
	public static int Gold { get; private set; }
	public static int Wood { get; private set; }

	private void Awake()
	{
		noiseFilter = new NoiseFilter(noiseSettings);
	}

	private void Start()
	{
		// Generate Hexsphere
		HexsphereGenerator.Generate(properties);
		transform.localScale = Vector3.one * properties.scale;

		// Generate random terrain
		GenerateTerrain();
	}

	private void GenerateTerrain()
	{
		// Set shader properties
		tileTypes[0].material.SetFloat("_radius", properties.sphereRadius * properties.scale);
		tileTypes[1].material.SetFloat("_radius", properties.sphereRadius * properties.scale);

		// Set type by noise value
		for (int i = 0; i < properties.pointsOnSphere.Count; i++)
		{
			float elevation = noiseFilter.Evaluate(properties.pointsOnSphere[i]);
			properties.tiles[i].SetType(tileTypes[Mathf.Min((int)elevation, 4)]);
		}
	}

	public void Damaged(float damage)
    {
		hp -= damage;
		if(hp <= 0)
        {
			// Explosion effect

			gameObject.SetActive(false);
			Debug.Log("Game over");
        }
    }
}
