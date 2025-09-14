using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : Weapon
{
    [SerializeField] private Collider _collider;
    [SerializeField] private TrailController _trailController;
    [SerializeField] private AudioSource _audioSource;
    private void OnTriggerEnter(Collider other)
    {
        Damage(other, Color.red);
    }
    public override void EnableWeapon()
    {
        base.EnableWeapon();
        _collider.enabled = true;
        _trailController.EnableTrail();
        _audioSource.Play();
    }
    public override void DisableWeapon()
    {
        base.DisableWeapon();
        _collider.enabled = false;
        _trailController.DisableTrail();
        _audioSource.Stop();
    }
}
