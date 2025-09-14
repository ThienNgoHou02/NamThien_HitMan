using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.OpenUI<CanvasMenu>();
    }
}
