using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemEffect : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        PoolingManager.Instance.Return(gameObject);
    }
}
