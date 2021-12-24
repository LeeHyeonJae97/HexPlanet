using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTower : Tower
{
    public override void Init(Tile tile)
    {
        instanceId = InstanceId++;
        SetTile(tile);
    }

    public override void Perform()
    {
        // 단순 데미지 감소용 타워
        // 일회용
    }

    public override void ShowInfo(bool show) { }
}
