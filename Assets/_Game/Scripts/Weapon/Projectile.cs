using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Projectile : GameEntity
{
    public GameEnities _hitVFX;
    public float _speed;
    public float _duration;
    public float _force;

    private bool _isFired;
    private float _flyTimer;
    private float _damage;
    private Vector3 _direction;
    private Transform _owner;
    public void Fire(Vector3 direction, Transform owner, float damage)
    {
        _isFired = true;
        _direction = direction;
        _owner = owner;
        _damage = damage;
        _flyTimer = 0;
    }
    private void Update()
    {
        if (_isFired)
        {
            _flyTimer += Time.deltaTime;
            if (_flyTimer >= _duration)
            {
                DestroyProjectile();
            }
            TF.position += _direction * _speed * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 closetPoint = other.ClosestPoint(transform.position);
        MasterPool.Pop<MeleeVFX>(_hitVFX, closetPoint, Quaternion.identity);
        MasterPool.Pop<PopupText>(GameEnities.PopupText, new Vector3(closetPoint.x, closetPoint.y + 1f, closetPoint.z), Quaternion.identity).SetText(DamageText(_damage), Color.red);
        Character character = Cache.GetCharacterByCollider(other);
        character?.Damaged(_owner, 0, _force, _damage);
        CameraController.Instance.CameraShake(6f);
        DestroyProjectile();
    }
    private string DamageText(float damage)
    {
        return $"-{(int)Random.Range(damage - 5f, damage + 5f)}";
    }
    private void DestroyProjectile()
    {
        _isFired = false;
        MasterPool.Push(this);
    }
}
