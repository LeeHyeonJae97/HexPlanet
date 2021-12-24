using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Tower : MonoBehaviour
{
    public enum State { Landed, Floating }
    public static int InstanceId = 0;

    public int id;
    public int instanceId;
    public string towerName;
    public int priceGold;
    public int priceCoal;
    public int maxLevel;
    public float[] cooldown;
    public float towardsSpeed = 10f;
    public float lerpSpeed = 0.05f;
    public float floatingPosOffset = 1.2f;

    public float buffedCooldown;
    public float Cooldown { get { return cooldown[curLevel] - buffedCooldown; } }

    public State CurState { get; private set; }
    protected int curLevel;
    private float curCooldown;

    private Tile tile;

    private Coroutine corLiftOff, corMove, corLand;

    public abstract void Init(Tile tile);    
    public abstract void Perform();
    public abstract void ShowInfo(bool show);

    public void SetTile(Tile tile)
    {
        this.tile = tile;
        tile.BuildTower();
    }

    public bool CheckCooldown()
    {
        if (curCooldown <= 0)
        {
            curCooldown = Cooldown;
            return true;
        }
        else
        {
            curCooldown -= Time.deltaTime;
            return false;
        }
    }

    public virtual void LiftOff()
    {
        if (CurState == State.Landed && corLiftOff == null && corMove == null && corLand == null)
            corLiftOff = StartCoroutine(Tweener.MoveLerp(transform, transform.position * floatingPosOffset, lerpSpeed, () => corLiftOff = null));

        tile.RemoveTower();
        tile = null;
        CurState = State.Floating;
    }

    public void Land(Vector3 targetPos, UnityAction onLanded)
    {
        if (CurState == State.Floating && corLiftOff == null && corLand == null)
        {
            if (corMove != null)
            {
                StopCoroutine(corMove);
                corMove = null;
            }

            StartCoroutine(CorLand(targetPos, towardsSpeed, lerpSpeed, onLanded));
        }
    }

    private IEnumerator CorLand(Vector3 targetPos, float towardsSpeed, float lerpSpeed, UnityAction onLanded)
    {
        // Rotate towards target tile
        Vector3 up = Vector3.Cross(transform.position, targetPos);
        Transform orgParent = transform.parent;
        Transform rotator = PoolingManager.Instance.Get("TowerRotator").transform;
        rotator.rotation = Quaternion.LookRotation(transform.position, up);
        transform.SetParent(rotator);

        Quaternion targetRotation = Quaternion.LookRotation(targetPos, up);
        yield return corMove = StartCoroutine(Tweener.RotateSmoothly(rotator, targetRotation, towardsSpeed, lerpSpeed));

        transform.SetParent(orgParent);
        PoolingManager.Instance.Return(rotator.gameObject);
        corMove = null;

        // Land on target tile
        yield return corLand = StartCoroutine(Tweener.MoveLerp(transform, targetPos, lerpSpeed));

        CurState = State.Landed;
        corLand = null;

        onLanded();
    }
}
