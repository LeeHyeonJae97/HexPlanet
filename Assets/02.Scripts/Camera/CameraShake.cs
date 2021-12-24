using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [System.Serializable]
    public class CameraShakeSettings
    {
        public float startAngle;
        public float duration;
        [Range(0, 1)] public float noise;
        [Range(0, 1)] public float damping;
    }

    public static CameraShake Instance;

    public CameraShakeSettings settings;
    private Coroutine corShake;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Shake(float strength)
    {
        if (corShake != null) StopCoroutine(corShake);
        corShake = StartCoroutine(CorShake(strength));
    }

    private IEnumerator CorShake(float strength)
    {
        float progress = 0;

        float noiseAngle = 0;
        float targetAngle = settings.startAngle * Mathf.Deg2Rad + noiseAngle;
        Vector3 targetPos = new Vector3(Mathf.Cos(targetAngle), Mathf.Sin(targetAngle)) * strength;
        targetPos.z = transform.localPosition.z;

        // Shake        
        while (true)
        {
            // Move to target position
            while (true)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 0.01f);
                if ((transform.localPosition - targetPos).sqrMagnitude < 0.01f)
                {
                    transform.localPosition = targetPos;
                    break;
                }
            }

            // Break when its position is similar to Vector3.zero
            if (transform.localPosition.sqrMagnitude < 0.01f)
            {
                transform.localPosition = Vector3.zero;
                break;
            }

            progress += Time.deltaTime / settings.duration;

            // Set next pos
            noiseAngle = Random.Range(-1f, 1f) * Mathf.PI * settings.noise;
            targetAngle += Mathf.PI + noiseAngle;
            targetPos = new Vector3(Mathf.Cos(targetAngle), Mathf.Sin(targetAngle)) * strength * DampingCurve(progress, settings.damping);
            targetPos.z = transform.localPosition.z;

            yield return null;
        }
    }

    // | ***
    // |    **
    // |      *
    // |      *
    // |      *
    // |      *
    // |       *
    // |        **
    // |          ***
    // ㅡㅡㅡㅡㅡㅡㅡㅡ
    private float DampingCurve(float x, float dampingPercent)
    {
        x = Mathf.Clamp01(x);
        float a = Mathf.Lerp(2, .25f, dampingPercent);
        float b = 1 - Mathf.Pow(x, a);
        return b * b * b;
    }
}
