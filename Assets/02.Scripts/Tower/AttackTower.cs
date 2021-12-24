using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTower : Tower
{
    public enum FindTargetType { Closest, Farthest, MaxHp, MinHp }
    public delegate Asteroid FindTarget(Vector3 position, float range);

    public int[] damage;
    public float[] range;

    public int buffedDamage;
    public int Damage { get { return damage[curLevel] + buffedDamage; } }
    public float buffedRange;
    public float Range { get { return range[curLevel] + buffedRange; } }

    private FindTargetType findTargetType;
    private FindTarget findTarget;

    public GameObject rangeDisplay;

    public override void Init(Tile tile)
    {
        instanceId = InstanceId++;

        findTarget = FindClosestTarget;
        findTargetType = FindTargetType.Closest;

        SetTile(tile);
    }

    public override void Perform()
    {
        if (CurState == State.Landed)
        {
            Asteroid target = findTarget(transform.position, Range);
            if (target != null)
            {
                // 미사일 발사 위치 약간 이격 필요

                GameObject go = PoolingManager.Instance.Get("Missile", transform.position, Quaternion.LookRotation(transform.forward, transform.up));
                go.GetComponent<Missile>().Init(target, Damage);
            }
        }
    }

    public void SetFindTargetType(FindTargetType type)
    {
        switch (type)
        {
            case FindTargetType.Closest:
                findTarget = FindClosestTarget;
                break;
            case FindTargetType.Farthest:
                findTarget = FindFarthestTarget;
                break;
            case FindTargetType.MaxHp:
                findTarget = FindMaxHpTarget;
                break;
            case FindTargetType.MinHp:
                findTarget = FindMinHpTarget;
                break;
        }

        findTargetType = type;
    }

    public void AddBuff(float cooldownBuff, int damageBuff, float rangeBuff)
    {
        buffedCooldown += cooldownBuff;
        buffedDamage += damageBuff;
        buffedRange += rangeBuff;
    }

    public void RemoveBuff(float cooldownBuff, int damageBuff, float rangeBuff)
    {
        buffedCooldown -= cooldownBuff;
        buffedDamage -= damageBuff;
        buffedRange -= rangeBuff;
    }

    public override void ShowInfo(bool show)
    {
        rangeDisplay.transform.localScale = Vector3.one * Range;
        rangeDisplay.SetActive(show);
    }

    private Asteroid FindClosestTarget(Vector3 towerPos, float range)
    {
        float minSqrDst = float.MaxValue;
        Asteroid target = null;

        for (int i = 0; i < AsteroidSpawner.Asteroids.Count; i++)
        {
            Asteroid asteroid = AsteroidSpawner.Asteroids[i];
            float sqrDst = (towerPos - asteroid.transform.position).sqrMagnitude;
            float dstThreshold = range;// + asteroid.properties.sphereRadius;

            if (sqrDst < dstThreshold * dstThreshold && sqrDst < minSqrDst)
            {
                minSqrDst = sqrDst;
                target = asteroid;
            }
        }

        return target;
    }

    private Asteroid FindFarthestTarget(Vector3 towerPos, float range)
    {
        float maxSqrDst = float.MinValue;
        Asteroid target = null;

        for (int i = 0; i < AsteroidSpawner.Asteroids.Count; i++)
        {
            Asteroid asteroid = AsteroidSpawner.Asteroids[i];
            float sqrDst = (towerPos - asteroid.transform.position).sqrMagnitude;
            float dstThreshold = range;// + asteroid.properties.sphereRadius;

            if (sqrDst < dstThreshold * dstThreshold && sqrDst > maxSqrDst)
            {
                maxSqrDst = sqrDst;
                target = asteroid;
            }
        }

        return target;
    }

    private Asteroid FindMaxHpTarget(Vector3 towerPos, float range)
    {
        float maxHp = float.MinValue;
        Asteroid target = null;

        for (int i = 0; i < AsteroidSpawner.Asteroids.Count; i++)
        {
            Asteroid asteroid = AsteroidSpawner.Asteroids[i];
            float sqrDst = (towerPos - asteroid.transform.position).sqrMagnitude;
            float dstThreshold = range + asteroid.properties.sphereRadius;

            if (sqrDst < dstThreshold * dstThreshold && asteroid.hp > maxHp)
            {
                maxHp = asteroid.hp;
                target = asteroid;
            }
        }

        return target;
    }

    private Asteroid FindMinHpTarget(Vector3 towerPos, float range)
    {
        float minHp = float.MaxValue;
        Asteroid target = null;

        for (int i = 0; i < AsteroidSpawner.Asteroids.Count; i++)
        {
            Asteroid asteroid = AsteroidSpawner.Asteroids[i];
            float sqrDst = (towerPos - asteroid.transform.position).sqrMagnitude;
            float dstThreshold = range + asteroid.properties.sphereRadius;

            if (sqrDst < dstThreshold * dstThreshold && asteroid.hp < minHp)
            {
                minHp = asteroid.hp;
                target = asteroid;
            }
        }

        return target;
    }
}
