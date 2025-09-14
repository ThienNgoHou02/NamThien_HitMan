using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{
    [SerializeField] private GameEnities _projectile;
    [SerializeField] private AudioSource _audioSource;

    public override void EnableWeapon()
    {
        base.EnableWeapon();
        Projectile prj = MasterPool.Pop<Projectile>(_projectile, transform.position, Quaternion.identity);
        prj.Fire(_owner.forward, _owner, _damage);
        _audioSource.Play();
        _gameobject.SetActive(false);
    }
    public override void DisableWeapon()
    {
        base.DisableWeapon();
        _audioSource.Stop();
        _gameobject.SetActive(true);
    }
}
