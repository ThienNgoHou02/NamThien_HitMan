using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T Ins;
    public static T Instance
    {
        get
        {
            if (Ins == null)
            {
                Ins = GameObject.FindObjectOfType<T>();
                if (Ins == null)
                {
                    Ins = new GameObject(nameof(T)).AddComponent<T>();
                }
            }
            return Ins;
        }
    }
}
