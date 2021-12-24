using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Tweener
{
    // Move towards target position
    public static IEnumerator Move(this Transform tr, Vector3 targetPos, float speed, UnityAction onFinished = null)
    {
        while (true)
        {
            if ((tr.position - targetPos).sqrMagnitude < 0.0001f)
            {
                tr.position = targetPos;
                break;
            }

            tr.position = Vector3.MoveTowards(tr.position, targetPos, speed * Time.deltaTime);

            yield return null;
        }

        if (onFinished != null) onFinished();
    }
    
    // Move towards target
    public static IEnumerator Move(this Transform tr, Transform target, float speed, UnityAction onFinished = null)
    {
        while (true)
        {
            if ((tr.position - target.position).sqrMagnitude < 0.0001f)
            {
                tr.position = target.position;
                break;
            }

            tr.position = Vector3.MoveTowards(tr.position, target.position, speed * Time.deltaTime);

            yield return null;
        }

        if (onFinished != null) onFinished();
    }

    // Move lerp towards target position
    public static IEnumerator MoveLerp(this Transform tr, Vector3 targetPos, float speed, UnityAction onFinished = null)
    {
        while (true)
        {
            if ((tr.position - targetPos).sqrMagnitude < 0.0001f)
            {
                tr.position = targetPos;
                break;
            }

            tr.position = Vector3.Lerp(tr.position, targetPos, speed);

            yield return null;
        }

        if (onFinished != null) onFinished();
    }

    // Move lerp towards target
    public static IEnumerator MoveLerp(this Transform tr, Transform target, float speed, UnityAction onFinished = null)
    {
        while (true)
        {
            if ((tr.position - target.position).sqrMagnitude < 0.0001f)
            {
                tr.position = target.position;
                break;
            }

            tr.position = Vector3.Lerp(tr.position, target.position, speed);

            yield return null;
        }

        if (onFinished != null) onFinished();
    }

    public static IEnumerator Rotate(this Transform tr, Quaternion targetRotation, float speed, UnityAction onFinished = null)
    {
        while (true)
        {
            if (Quaternion.Angle(tr.rotation, targetRotation) < 0.01f) break;

            tr.rotation = Quaternion.RotateTowards(tr.rotation, targetRotation, speed * Time.deltaTime);

            yield return null;
        }

        if (onFinished != null) onFinished();
    }    

    public static IEnumerator RotateLerp(this Transform tr, Quaternion targetRotation, float speed, UnityAction onFinished = null)
    {
        while (true)
        {
            if (Quaternion.Angle(tr.rotation, targetRotation) < 0.01f) break;

            tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, speed);

            yield return null;
        }

        if (onFinished != null) onFinished();
    }

    public static IEnumerator RotateSmoothly(this Transform tr, Quaternion targetRotation, float towardsSpeed, float lerpSpeed, UnityAction onFinished = null)
    {
        float rotateTowardsAngleThreshold = Quaternion.Angle(tr.rotation, targetRotation) * 0.2f;

        while (true)
        {
            float angle = Quaternion.Angle(tr.rotation, targetRotation);
            if (angle < 0.01f)
            {
                break;
            }
            else if (angle < rotateTowardsAngleThreshold)
            {
                tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, lerpSpeed);
            }
            else
            {
                tr.rotation = Quaternion.RotateTowards(tr.rotation, targetRotation, towardsSpeed * Time.deltaTime);
            }

            yield return null;
        }

        if (onFinished != null) onFinished();
    }
}
