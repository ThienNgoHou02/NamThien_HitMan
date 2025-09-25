using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterPool
{
    private static Dictionary<GameEnities, Pool> _PoolMaster = new Dictionary<GameEnities, Pool>();

    public static void Preload(Transform parent, GameEntity prefab, int amount)
    {
        if (!prefab)
        {
            return;
        }
        if (_PoolMaster.ContainsKey(prefab._gameEntity))
        {
            if (_PoolMaster[prefab._gameEntity] != null)
            {
                return;
            }
        }
        Pool pool = new Pool();
        pool.PreLoad(parent, prefab, amount);
        _PoolMaster[prefab._gameEntity] = pool;
    }
    public static void Push(GameEntity enity)
    {
        if (!_PoolMaster.ContainsKey(enity._gameEntity))
        {
            return;
        }
        _PoolMaster[enity._gameEntity].Push(enity);
    }
    public static T Pop<T>(GameEnities tag, Vector3 position, Quaternion rotation) where T : GameEntity
    {
        if (!_PoolMaster.ContainsKey(tag))
        {
            return null;
        }
        return _PoolMaster[tag].Pop(position, rotation) as T;
    }
    public static void Collect(GameEnities poolType)
    {
        if (!_PoolMaster.ContainsKey(poolType))
        {
            Debug.LogError($"{poolType} IS NOT PRELOAD!!");
        }
        _PoolMaster[poolType].Collect();
    }
    public static void CollectAll()
    {
        foreach (var pool in _PoolMaster.Values)
        {
            pool.Collect();
        }
    }
    public static void Release(GameEnities poolType)
    {
        if (!_PoolMaster.ContainsKey(poolType))
        {
            Debug.LogError($"{poolType} IS NOT PRELOAD!!");
        }
        _PoolMaster[poolType].Release();
    }
    public static void ReleaseAll()
    {
        foreach (var pool in _PoolMaster.Values)
        {
            pool.Release();
        }
    }
}
