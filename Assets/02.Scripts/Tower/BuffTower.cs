using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : Tower
{
    [SerializeField] private float[] range;
    public float Range { get { return range[curLevel]; } }

    [SerializeField] private int[] damageBuff;
    public int DamageBuff { get { return damageBuff.Length > 0 ? damageBuff[curLevel] : 0; } }

    [SerializeField] private float[] cooldownBuff;
    public float CooldownBuff { get { return cooldownBuff.Length > 0 ? cooldownBuff[curLevel] : 0; } }

    [SerializeField] private float[] rangeBuff;
    public float RangeBuff { get { return rangeBuff.Length > 0 ? rangeBuff[curLevel] : 0; } }

    public GameObject rangeDisplay;

    private List<int> buffedTowerIds = new List<int>();

    public override void Init(Tile tile)
    {
        instanceId = InstanceId++;
        SetTile(tile);
        rangeDisplay.transform.localScale = Vector3.one * Range;
    }

    public override void Perform()
    {
        if (CurState == State.Landed)
        {
            for (int i = 0; i < TowerManager.Towers.Count; i++)
            {
                if(TowerManager.Towers[i].GetType() == typeof(AttackTower))
                {
                    AttackTower tower = (AttackTower)TowerManager.Towers[i];
                    if ((transform.position - tower.transform.position).sqrMagnitude < range[curLevel] * range[curLevel])
                    {
                        if(!buffedTowerIds.Contains(tower.instanceId))
                        {
                            tower.AddBuff(CooldownBuff, DamageBuff, RangeBuff);
                            buffedTowerIds.Add(tower.instanceId);
                        }
                    }
                    else if(buffedTowerIds.Contains(tower.instanceId))
                    {
                        tower.RemoveBuff(CooldownBuff, DamageBuff, RangeBuff);
                        buffedTowerIds.Remove(tower.instanceId);
                    }
                }
            }
        }
    }    

    public override void ShowInfo(bool show)
    {
        rangeDisplay.SetActive(show);
    }
}
