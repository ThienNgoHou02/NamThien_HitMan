using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
    void OnEnter(T owner);
    void OnExecute(T owner);
    void OnExit(T owner);
}
public struct Combo
{
    public string _comboName;
    public Weapon _weapon;

    public Combo(string comboName, Weapon weapon)
    {
        _comboName = comboName;
        _weapon = weapon;
        _weapon.DisableWeapon();
    }
}
