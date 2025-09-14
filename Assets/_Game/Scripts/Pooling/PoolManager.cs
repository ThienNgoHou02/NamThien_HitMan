using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolInfo[] _poolInfos;

    private void Awake()
    {
        for (int i = 0; i < _poolInfos.Length; i++)
        {
            MasterPool.Preload(_poolInfos[i]._parentTF, _poolInfos[i]._prefab, _poolInfos[i]._amount);
        }
    }
}
[System.Serializable]
public class PoolInfo
{
    public Transform _parentTF;
    public GameEntity _prefab;
    public int _amount;
}
