using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollectingTower : Tower
{
    public enum ResourceType { Gold, Coal, Both }

    public ResourceType resourceType;
    public int collectingAmount;

    private ResourceManager resourceManager;

    private void Awake()
    {
        if (resourceManager == null) resourceManager = FindObjectOfType<ResourceManager>();
    }

    public override void Init(Tile tile)
    {
        SetTile(tile);
    }

    public override void Perform()
    {
        if (CurState == State.Landed)
        {
            switch (resourceType)
            {
                case ResourceType.Gold:
                    resourceManager.CollectGold(collectingAmount);
                    break;
                case ResourceType.Coal:
                    resourceManager.CollectCoal(collectingAmount);
                    break;
                case ResourceType.Both:
                    resourceManager.CollectGold(collectingAmount);
                    resourceManager.CollectCoal(collectingAmount);
                    break;
            }
        }
    }

    public override void ShowInfo(bool show) { }
}
