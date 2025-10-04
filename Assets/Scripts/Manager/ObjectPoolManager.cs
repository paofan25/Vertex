using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对象池管理器，用于统一管理多个预制体的对象池
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    // 存储所有对象池的字典，key为预制体，value为对应的对象池
    private Dictionary<GameObject, UnityObjectPool> _pools = new Dictionary<GameObject, UnityObjectPool>();

    private void Awake()
    {
        // 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化指定预制体的对象池
    /// </summary>
    public void InitPool(GameObject prefab, Transform parent = null, 
                        int defaultCapacity = 10, int maxSize = 1000)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new UnityObjectPool(prefab, parent, defaultCapacity, maxSize);
        }
    }

    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    public GameObject GetObject(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab))
        {
            // 如果对象池未初始化，则自动初始化
            InitPool(prefab);
        }

        return _pools[prefab].Get();
    }

    /// <summary>
    /// 释放对象回对应的对象池
    /// </summary>
    public void ReleaseObject(GameObject prefab, GameObject obj)
    {
        if (_pools.ContainsKey(prefab))
        {
            _pools[prefab].Release(obj);
        }
        else
        {
            // 如果没有对应的对象池，直接销毁
            Object.Destroy(obj);
        }
    }

    /// <summary>
    /// 清理指定预制体的对象池
    /// </summary>
    public void ClearPool(GameObject prefab)
    {
        if (_pools.ContainsKey(prefab))
        {
            _pools[prefab].Clear();
            _pools.Remove(prefab);
        }
    }

    /// <summary>
    /// 清理所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Clear();
        }
        _pools.Clear();
    }
}

