using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private Transform _parentTF;
    private GameEntity _prefab;

    private Queue<GameEntity> _Pool = new Queue<GameEntity>();
    List<GameEntity> _Alive = new List<GameEntity>();
    public void PreLoad(Transform parent, GameEntity prefab, int amount)
    {
        _parentTF = parent;
        _prefab = prefab;

        for (int i = 0; i < amount; i++)
        {
            Push(Pop(UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity));
        }
    }
    public void Push(GameEntity entity)
    {
        if (entity)
        {
            _Pool.Enqueue(entity);
            _Alive.Remove(entity);
            entity.gameObject.SetActive(false);
        }
    }
    public GameEntity Pop(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation)
    {
        GameEntity entity;
        if (_Pool.Count <= 0)
        {
            entity = GameObject.Instantiate(_prefab, _parentTF);
        }
        else
        {
            entity = _Pool.Dequeue();
        }
        _Alive.Add(entity);
        entity.TF.SetPositionAndRotation(position, rotation);
        entity.gameObject.SetActive(true);
        return entity;
    }
    public void Collect()
    {
        while (_Alive.Count > 0)
        {
            Push(_Alive[0]);
        }
    }
    public void Release()
    {
        Collect();
        while (_Pool.Count > 0)
            GameObject.Destroy(_Pool.Dequeue().gameObject);
        _Pool.Clear();
    }
}
