using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform cube;
    public Transform target;

    public float moveTowards;
    public float lerp;
    public float duration;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Lerp());
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(MoveTowards());
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(LerpPosition());
        }
    }

    private IEnumerator MoveTowards()
    {
        cube.position = Vector3.zero;
        Vector3 startPos = cube.position;

        while (true)
        {
            if ((cube.position - target.position).sqrMagnitude < 0.01f)
            {
                cube.position = target.position;
                break;
            }

            cube.position = Vector3.MoveTowards(cube.position, target.position, moveTowards * Time.deltaTime);

            Debug.Log("MoveTowards");

            yield return null;
        }
    }

    IEnumerator LerpPosition()
    {
        float time = 0;
        cube.position = Vector3.zero;
        Vector3 startPosition = cube.position;        

        while (time < duration)
        {
            cube.position = Vector3.Lerp(cube.position, target.position, time / duration);
            time += Time.deltaTime;

            if((cube.position - target.position).sqrMagnitude < 0.01f)
            {
                cube.position = target.position;
                break;
            }

            Debug.Log("Lerp2");

            yield return null;
        }
        cube.position = target.position;
    }

    private IEnumerator Lerp()
    {
        cube.position = Vector3.zero;

        while (true)
        {
            if ((cube.position - target.position).sqrMagnitude < 0.01f)
            {
                cube.position = target.position;
                break;
            }

            cube.position = Vector3.Lerp(cube.position, target.position, lerp * Time.deltaTime);

            Debug.Log("Lerp");

            yield return null;
        }
    }
}
