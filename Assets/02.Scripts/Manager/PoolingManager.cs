using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string key;
        public int amount;
        public GameObject prefab;
        private Transform holder;
        private Queue<GameObject> queue;

        public Pool(string key, int amount, GameObject prefab, Transform poolHolder)
        {
            this.key = key;
            this.amount = amount;
            this.prefab = prefab;

            Init(poolHolder);
        }

        public void Init(Transform poolHolder)
        {
            queue = new Queue<GameObject>();
            if (holder == null)
            {
                holder = new GameObject(key + "Holder").transform;
                holder.SetParent(poolHolder);
            }

            for (int i = 0; i < amount; i++)
            {
                GameObject go = Instantiate(prefab, holder);                
                go.name = key;
                go.SetActive(false);
                queue.Enqueue(go);
            }
        }

        private void Expand()
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject go = Instantiate(prefab, holder);                
                go.name = key;
                go.SetActive(false);
                queue.Enqueue(go);
            }
        }

        public GameObject Get()
        {
            if (queue.Count <= 0) Expand();

            GameObject go = queue.Dequeue();
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject go)
        {
            go.transform.SetParent(holder);
            go.SetActive(false);
            queue.Enqueue(go);
        }
    }

    public static PoolingManager Instance;

    private Transform poolHolder;
    public Pool[] pools;
    private Dictionary<string, Pool> poolDic = new Dictionary<string, Pool>();    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Debug.LogError("More than one PoolingManagers are in Scene");
            return;
        }

        poolHolder = new GameObject("PoolHolder").transform;
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i].Init(poolHolder);
            poolDic.Add(pools[i].key, pools[i]);
        }
    }

    public void CreatePool(string key, int amount, GameObject prefab)
    {
        Pool pool = new Pool(key, amount, prefab, poolHolder);
        poolDic.Add(key, pool);
    }

    #region Get
    public GameObject Get(string key)
    {
        if(!poolDic.ContainsKey(key))
        {
            Debug.LogError("Wrong key");
            return null;
        }

        GameObject go = poolDic[key].Get();
        go.transform.SetParent(null);
        return go;
    }

    public GameObject Get(string key, Transform parent)
    {
        if (!poolDic.ContainsKey(key))
        {
            Debug.LogError("Wrong key");
            return null;
        }

        GameObject go = poolDic[key].Get();
        go.transform.SetParent(parent);
        return go;
    }

    public GameObject Get(string key, Vector3 pos)
    {
        if (!poolDic.ContainsKey(key))
        {
            Debug.LogError("Wrong key");
            return null;
        }

        GameObject go = poolDic[key].Get();
        go.transform.SetParent(null);
        go.transform.position = pos;
        return go;
    }

    public GameObject Get(string key, Vector3 pos, Quaternion rot)
    {
        if(!poolDic.ContainsKey(key))
        {
            Debug.LogError("Wrong key");
            return null;
        }

        GameObject go = poolDic[key].Get();
        go.transform.SetParent(null);
        go.transform.position = pos;
        go.transform.rotation = rot;
        return go;
    }

    public GameObject Get(string key, Transform parent, Vector3 pos)
    {
        if (!poolDic.ContainsKey(key))
        {
            Debug.LogError("Wrong key");
            return null;
        }

        GameObject go = poolDic[key].Get();
        go.transform.SetParent(parent);
        go.transform.position = pos;
        return go;
    }

    public GameObject Get(string key, Transform parent, Vector3 pos, Quaternion rot)
    {
        if (!poolDic.ContainsKey(key))
        {
            Debug.LogError("Wrong key");
            return null;
        }

        GameObject go = poolDic[key].Get();
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.rotation = rot;
        return go;
    }
    #endregion

    #region Return
    public void Return(GameObject go)
    {
        if (!poolDic.ContainsKey(go.name))
        {
            Debug.LogError("Wrong key : " + go.name);
            return;
        }
        else poolDic[go.name].Return(go);
    }
    #endregion
}
