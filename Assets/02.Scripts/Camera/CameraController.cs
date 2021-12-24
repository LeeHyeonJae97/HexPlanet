using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private struct CameraView
    {
        public Quaternion Rotation { get; private set; }
        public float Fov { get; private set; }

        public void Save(Quaternion rotation, float fov)
        {
            Rotation = rotation;
            Fov = fov;
        }
    }

    public static Quaternion Rotation;

    [Header("Properties")]
    public float rotateByKeyboardSpeed = 50f;
    public float rotateByMouseSpeed = 150f;
    public float zoomSensitivity = 5f;
    public float lookTowardsSpeed = 0.15f;

    private Camera mainCam;
    private CameraView[] savedViews = new CameraView[3];
    private Coroutine corLookTowards;

    public float dragThreshold;
    private bool isDragging;
    private Vector3 mousePos;

    public float minFov = 5, maxFov = 100;

    [Space(10)]
    public TowerManager towerManager;

    private void Awake()
    {
        mainCam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        // Rotate by keyboard
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 || v != 0)
        {
            if (corLookTowards != null)
            {
                StopCoroutine(corLookTowards);
                corLookTowards = null;
            }

            transform.Rotate(new Vector3(-v, h, 0), rotateByKeyboardSpeed * Time.deltaTime, Space.Self);

            // Save rotation
            Rotation = transform.rotation;
        }

        // Rotate by mouse
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            mousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            if (corLookTowards != null)
            {
                StopCoroutine(corLookTowards);
                corLookTowards = null;
            }

            Vector3 delta = Input.mousePosition - mousePos;
            if (-dragThreshold <= delta.x && delta.x <= dragThreshold) delta.x = 0;
            if (-dragThreshold <= delta.y && delta.y <= dragThreshold) delta.y = 0;
            if (delta != Vector3.zero)
            {
                transform.Rotate(new Vector3(-delta.y, delta.x, 0), rotateByMouseSpeed * Time.deltaTime, Space.Self);
                mousePos = Input.mousePosition;
            }

            // Save rotation
            Rotation = transform.rotation;
        }

        // Zoom by mouse scroll
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            if (corLookTowards != null)
            {
                StopCoroutine(corLookTowards);
                corLookTowards = null;
            }

            mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView + (scrollDelta > 0 ? -1 : +1) * zoomSensitivity, minFov, maxFov);
        }

        // Zoom in selected tile or tower by input 'F'
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 targetForward = Vector3.zero;
            if (towerManager.GetSelectedTileCenter(ref targetForward) || towerManager.GetSelectedTowerPosition(ref targetForward))
            {
                Quaternion targetRotation = Quaternion.LookRotation(-targetForward, transform.up);
                ZoomInSelectedTile(targetRotation);
            }
        }

        // Save view
        for (int i = 0; i < 3; i++)
        {
            KeyCode check = KeyCode.Alpha1 + i;
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(check))
                savedViews[i].Save(transform.rotation, mainCam.fieldOfView);
        }

        // Load saved view
        for (int i = 0; i < 3; i++)
        {
            KeyCode check = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(check) && savedViews[i].Fov != 0) LoadSavedView(savedViews[i]);
        }
    }

    private void LoadSavedView(CameraView view)
    {
        if (corLookTowards != null) StopCoroutine(corLookTowards);
        corLookTowards = StartCoroutine(LookTowards(view.Rotation, view.Fov));
    }

    private void ZoomInSelectedTile(Quaternion targetRotation)
    {
        float targetFov = 10;

        if (corLookTowards != null) StopCoroutine(corLookTowards);
        corLookTowards = StartCoroutine(LookTowards(targetRotation, targetFov));
    }

    // Change View to saved view
    private IEnumerator LookTowards(Quaternion targetRotation, float targetFov)
    {
        bool isRotateDone = false, isZoomDone = false;
        while (true)
        {
            if (!isRotateDone)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookTowardsSpeed);
                if (transform.rotation == targetRotation) isRotateDone = true;
            }

            if (!isZoomDone)
            {
                mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFov, lookTowardsSpeed);
                if (mainCam.fieldOfView == targetFov) isZoomDone = true;
            }

            if (isRotateDone && isZoomDone) break;

            yield return null;
        }

        // Save Rotation
        Rotation = transform.rotation;

        corLookTowards = null;
    }
}
