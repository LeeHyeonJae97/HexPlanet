using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITween : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down }

    public Direction direction;
    public float tweenSpeed;

    private GameObject canvas;

    private Vector2 activePos, inActivePos;
    private Coroutine corShow, corHide;

    private void Awake()
    {
        // Get position when activated and inactivated
        RectTransform tr = (RectTransform)transform;

        inActivePos = tr.position;

        float width = tr.sizeDelta.x;
        float height = tr.sizeDelta.y;
        switch (direction)
        {
            case Direction.Left:
                activePos = new Vector2(inActivePos.x - width, inActivePos.y);
                break;
            case Direction.Right:
                activePos = new Vector2(inActivePos.x + width, inActivePos.y);
                break;
            case Direction.Up:
                activePos = new Vector2(inActivePos.x, inActivePos.y + height);
                break;
            case Direction.Down:
                activePos = new Vector2(inActivePos.x + width, inActivePos.y - height);
                break;
        }

        canvas = GetComponentInParent<Canvas>().gameObject;
        canvas.SetActive(false);
    }

    public void Show(bool show)
    {
        if (show && corShow == null)
        {
            if (corHide != null)
            {
                StopCoroutine(corHide);
                corHide = null;
            }
            canvas.SetActive(true);
            corShow = StartCoroutine(Tween(true));
        }
        else if (!show && corHide == null && corShow == null && gameObject.activeInHierarchy)
        {
            corHide = StartCoroutine(Tween(false));
        }
    }

    private IEnumerator Tween(bool show)
    {
        if (show)
        {
            yield return null;
            yield return StartCoroutine(Tweener.Move(transform, activePos, tweenSpeed));

            corShow = null;
        }
        else
        {
            yield return null;
            yield return StartCoroutine(Tweener.Move(transform, inActivePos, tweenSpeed));

            canvas.SetActive(false);
            corHide = null;
        }
    }
}
