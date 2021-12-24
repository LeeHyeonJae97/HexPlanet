using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AsteroidExplainer : MonoBehaviour
{
    public InfoCanvas infoCanvas;
    public Transform ringEffect;
    public LineRenderer lineToCollisionPoint;

    public Material warningMat;

    private bool isShowing;
    private Asteroid selectedAsteroid;

    private void Update()
    {
        if (isShowing)
        {
            // Ring effect should look at camera
            ringEffect.rotation = CameraController.Rotation;

            // Set lineRenderer's start point by current position
            lineToCollisionPoint.SetPosition(0, selectedAsteroid.transform.position);
        }
    }

    public void ShowInfo(Asteroid asteroid)
    {        
        if (selectedAsteroid != null && selectedAsteroid != asteroid) selectedAsteroid.ShowDamageRange(false);
        asteroid.ShowDamageRange(true, warningMat);
        selectedAsteroid = asteroid;

        ringEffect.SetParent(selectedAsteroid.transform);
        ringEffect.localPosition = Vector3.zero;
        ringEffect.localScale = Vector3.one;
        ringEffect.gameObject.SetActive(true);

        lineToCollisionPoint.gameObject.SetActive(true);

        infoCanvas.ShowAsteroidInfo(selectedAsteroid);

        isShowing = true;
    }

    public void HideInfo()
    {
        if(isShowing)
        {
            selectedAsteroid.ShowDamageRange(false);
            selectedAsteroid = null;

            ringEffect.gameObject.SetActive(false);

            lineToCollisionPoint.gameObject.SetActive(false);

            infoCanvas.Hide();

            isShowing = false;
        }
    }
}
