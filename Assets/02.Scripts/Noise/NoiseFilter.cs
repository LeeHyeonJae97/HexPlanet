using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    private NoiseSettings settings;
    private Noise noise = new Noise();

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float totalNoiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float noiseValue = noise.Evaluate(point * frequency + settings.centre);
            totalNoiseValue += (noiseValue + 1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        totalNoiseValue = Mathf.Max(0, totalNoiseValue - settings.minValue);
        return totalNoiseValue * settings.strength;
    }
}
