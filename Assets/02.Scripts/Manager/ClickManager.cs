using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public enum State { Idle, AsteroidInfo, TowerInfo, BuildTower, LandTower }

    public static State CurState;

    public TowerManager towerManager;
    public AsteroidExplainer asteroidExplainer;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                string tag = hit.collider.tag;
                switch (tag)
                {
                    case "Asteroid":
                        ClickAsteroid(hit.collider.gameObject.GetComponent<Asteroid>());
                        break;
                    case "PlanetTile":
                        ClickPlanetTile(hit.collider.gameObject.GetComponent<Tile>());
                        break;
                    case "Tower":
                        ClickTower(hit.collider.gameObject.GetComponentInParent<Tower>());
                        break;
                }
            }
            else
            {
                ClickNothing();
            }
        }
    }

    private void ClickTower(Tower clicked)
    {
        towerManager.ShowInfo(clicked);
        if (CurState == State.BuildTower) towerManager.Cancel();
        if (CurState == State.AsteroidInfo) asteroidExplainer.HideInfo();

        CurState = State.TowerInfo;
    }

    private void ClickPlanetTile(Tile clicked)
    {
        if (CurState == State.LandTower)
        {
            towerManager.LandTower(clicked);

            CurState = State.TowerInfo;
        }
        else
        {
            towerManager.SelectTile(clicked);
            if (CurState == State.TowerInfo) towerManager.HideInfo();
            if (CurState == State.AsteroidInfo) asteroidExplainer.HideInfo();

            CurState = State.BuildTower;
        }
    }

    private void ClickAsteroid(Asteroid clicked)
    {
        asteroidExplainer.ShowInfo(clicked);
        if (CurState == State.TowerInfo || CurState == State.LandTower) towerManager.HideInfo();
        if (CurState == State.BuildTower) towerManager.Cancel();

        CurState = State.AsteroidInfo;
    }

    private void ClickNothing()
    {
        if (CurState == State.TowerInfo || CurState == State.LandTower) towerManager.HideInfo();
        if (CurState == State.BuildTower) towerManager.Cancel();
        if (CurState == State.AsteroidInfo) asteroidExplainer.HideInfo();

        CurState = State.Idle;
    }
}
