using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 基于Unity内置对象池API的游戏对象池
/// </summary>
public class UnityObjectPool
{
    private readonly IObjectPool<GameObject> _pool;
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="prefab">要池化的预制体</param>
    /// <param name="parent">对象回收后的父节点</param>
    /// <param name="defaultCapacity">默认容量</param>
    /// <param name="maxSize">最大容量</param>
    public UnityObjectPool(GameObject prefab, Transform parent = null, 
                          int defaultCapacity = 10, int maxSize = 1000)
    {
        _prefab = prefab;
        _parent = parent;

        // 创建对象池
        _pool = new ObjectPool<GameObject>(
            createFunc: CreateObject,      // 创建新对象的方法
            actionOnGet: OnGet,            // 从池获取对象时的操作
            actionOnRelease: OnRelease,    // 释放对象回池时的操作
            actionOnDestroy: OnDestroy,    // 销毁对象时的操作
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    public GameObject Get()
    {
        return _pool.Get();
    }

    /// <summary>
    /// 释放对象回池
    /// </summary>
    public void Release(GameObject obj)
    {
        _pool.Release(obj);
    }

    /// <summary>
    /// 清理对象池
    /// </summary>
    public void Clear()
    {
        _pool.Clear();
    }

    // 创建新对象
    private GameObject CreateObject()
    {
        GameObject obj = Object.Instantiate(_prefab, _parent);
        obj.name = _prefab.name; // 保持名称一致
        return obj;
    }

    // 获取对象时的操作
    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    // 释放对象时的操作
    private void OnRelease(GameObject obj)
    {
        obj.SetActive(false);
        
        // 如果指定了父节点，回收时重置父节点
        if (_parent != null)
            obj.transform.SetParent(_parent);
    }

    // 销毁对象时的操作
    private void OnDestroy(GameObject obj)
    {
        Object.Destroy(obj);
    }
}
