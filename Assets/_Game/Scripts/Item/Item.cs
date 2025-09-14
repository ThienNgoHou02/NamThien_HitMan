using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : GameEntity
{
    private readonly int LOOT = Animator.StringToHash("Loot");

    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _itemValue;
    public Action<int> _lootAction;

    Coroutine _crtHideItem;
    private void OnEnable()
    {
        if (!_collider.enabled)
        {
            _collider.enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        _collider.enabled = false;
        int value = GetValue(_itemValue);
        _lootAction?.Invoke(value);
        _animator.SetTrigger(LOOT);
        if (_crtHideItem != null )
        {
            StopCoroutine( _crtHideItem );
        }
        _crtHideItem = StartCoroutine(HideItem(1.1f));
    }
    private int GetValue(float value)
    {
        return (int)Random.Range(value - 3f, value + 3f);
    }
    IEnumerator HideItem(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        _lootAction = null;
        MasterPool.Push(this);
    }
}
