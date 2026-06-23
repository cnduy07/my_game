using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    static readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetSceneState()
    {
        pools.Clear();
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        GameObject instance = null;
        if (pools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            while (pool.Count > 0 && instance == null)
                instance = pool.Dequeue();
        }

        if (instance == null)
        {
            instance = Object.Instantiate(prefab);
            var pooled = instance.GetComponent<PooledObject>();
            if (pooled == null) pooled = instance.AddComponent<PooledObject>();
            pooled.prefabSource = prefab;
        }

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    public static void Despawn(GameObject instance)
    {
        if (instance == null) return;

        var pooled = instance.GetComponent<PooledObject>();
        if (pooled == null || pooled.prefabSource == null)
        {
            Object.Destroy(instance);
            return;
        }

        if (!pools.TryGetValue(pooled.prefabSource, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools.Add(pooled.prefabSource, pool);
        }

        instance.SetActive(false);
        pool.Enqueue(instance);
    }

    public static void Clear()
    {
        foreach (var pair in pools)
        {
            while (pair.Value.Count > 0)
            {
                GameObject instance = pair.Value.Dequeue();
                if (instance != null)
                    Object.Destroy(instance);
            }
        }

        pools.Clear();
    }
}
