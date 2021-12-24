using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmBuildTowerButton : MonoBehaviour
{
    public void Show(Transform tr)
    {
        transform.SetParent(tr);
        transform.localPosition = Vector2.zero;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (gameObject.activeInHierarchy) gameObject.SetActive(false);
    }
}
