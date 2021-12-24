using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static List<Tower> Towers = new List<Tower>();

    public InfoCanvas infoCanvas;
    public UITween buildTowerCanvasUITween;
    public ConfirmBuildTowerButton confirmBuildTowerButton;

    private Tower selectedTower;
    private bool isShowing;

    private Tile selectedTile;
    private bool isSelectingTower;

    public GameObject[] previewTowers;
    private Dictionary<int, GameObject> previewTowerDic = new Dictionary<int, GameObject>();
    private int selectedPreviewTowerId;
    private GameObject selectedPreviewTower;

    public int SetAttackTowerFindTargetType
    {
        set
        {
            if (selectedTower != null) ((AttackTower)selectedTower).SetFindTargetType((AttackTower.FindTargetType)value);
        }
    }

    private void Awake()
    {
        for (int i = 0; i < previewTowers.Length; i++)
            previewTowerDic.Add(i, previewTowers[i]);
    }

    private void Update()
    {
        // Check tower can attack
        for (int i = 0; i < Towers.Count; i++)
        {
            if (Towers[i].CheckCooldown()) Towers[i].Perform();
        }
    }

    public bool GetSelectedTowerPosition(ref Vector3 pos)
    {
        if (selectedTower != null)
        {
            pos = selectedTower.transform.position;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetSelectedTileCenter(ref Vector3 center)
    {
        if (isSelectingTower)
        {
            center = selectedTile.Center;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LiftOffSelectedTower()
    {
        if (selectedTower != null)
        {
            selectedTower.LiftOff();
            infoCanvas.ButtonsWhenTowerLiftedOff();
        }
    }

    // Called when click land button
    public void SearchTileForLandTower()
    {
        ClickManager.CurState = ClickManager.State.LandTower;
    }

    public void LandTower(Tile tile)
    {
        Tower tower = selectedTower;

        if (tower != null) tower.Land(tile.Center, () =>
        {
            tower.SetTile(tile);
            infoCanvas.ButtonsWhenTowerLanded();
        });
    }

    public void ShowInfo(Tower tower)
    {
        if (selectedTower != null && selectedTower != tower) selectedTower.ShowInfo(false);
        tower.ShowInfo(true);
        selectedTower = tower;

        infoCanvas.ShowTowerInfo(tower);

        isShowing = true;
    }

    public void HideInfo()
    {
        if (isShowing)
        {
            selectedTower.ShowInfo(false);
            selectedTower = null;

            infoCanvas.Hide();

            isShowing = false;
        }
    }

    public void SelectTile(Tile tile)
    {
        if (!tile.IsEmpty)
        {
            Cancel();
            return;
        }

        if (selectedPreviewTower != null) selectedPreviewTower.SetActive(false);
        confirmBuildTowerButton.Hide();

        selectedTile = tile;
        isSelectingTower = true;

        buildTowerCanvasUITween.Show(true);
    }

    public void SelectTower(int id)
    {
        if (!isSelectingTower)
        {
            Debug.LogError("Error");
            return;
        }

        if (selectedPreviewTower != null) selectedPreviewTower.SetActive(false);

        GameObject previewTower = previewTowerDic[id];
        previewTower.transform.position = selectedTile.Center;
        previewTower.transform.up = selectedTile.Center;
        previewTower.SetActive(true);

        selectedPreviewTowerId = id;
        selectedPreviewTower = previewTower;
    }

    public void BuildTower()
    {
        Transform tr = PoolingManager.Instance.Get(selectedPreviewTowerId.ToString()).transform;
        tr.position = selectedTile.Center;
        tr.up = selectedTile.Center;

        Tower tower = tr.GetComponent<Tower>();
        Towers.Add(tower);
        tower.Init(selectedTile);

        Cancel();
    }

    public void Cancel()
    {
        if (isSelectingTower)
        {
            if (selectedPreviewTower != null)
            {
                selectedPreviewTower.SetActive(false);
                selectedPreviewTower = null;
            }

            selectedTile = null;
            isSelectingTower = false;

            confirmBuildTowerButton.Hide();
            buildTowerCanvasUITween.Show(false);
        }
    }
}
