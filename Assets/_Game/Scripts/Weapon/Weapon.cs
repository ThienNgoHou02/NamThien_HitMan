using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject _gameobject;
    public Transform _owner;
    public GameEnities _hitVFX;
    public int _side;
    public bool _isEnabled;
    public float _force;

    protected float _damage;

    public virtual void EnableWeapon() 
    { 
        _isEnabled = true;
    }
    public virtual void DisableWeapon() 
    { 
        _isEnabled = false;
    }
    public void Damage(Collider other, Color color)
    {
        Vector3 closetPoint = other.ClosestPoint(transform.position);
        MasterPool.Pop<MeleeVFX>(_hitVFX, closetPoint, Quaternion.identity);
        MasterPool.Pop<PopupText>(GameEnities.PopupText, new Vector3(closetPoint.x, closetPoint.y + 1f, closetPoint.z), Quaternion.identity).SetText(DamageText(_damage), color);
        Character character = Cache.GetCharacterByCollider(other);
        character?.Damaged(_owner, _side, _force, _damage);
        CameraController.Instance.CameraShake(6f);
    }
    public string DamageText(float damage)
    {
        return $"-{(int)Random.Range(damage - 5f, damage + 5f)}";
    }
    public float DAMAGE
    {
        get { return _damage; }
        set { _damage = value; }
    }
}
