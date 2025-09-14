using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Melee : Weapon
{
    private void OnTriggerEnter(Collider other)
    {
        Damage(other, Color.yellow);
    }
    public override void EnableWeapon()
    {
        base.EnableWeapon();
        _gameobject.SetActive(true);
    }
    public override void DisableWeapon()
    {
        base.DisableWeapon();
        _gameobject.SetActive(false);
    }
}
